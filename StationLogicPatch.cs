using System;
using HarmonyLib;

namespace BigTower
{
    /// <summary>
    /// 原版运输塔逻辑有多处手写展开到固定格数，这里把它们补到全部格子：
    /// - needs：原版为 [格0..格4, 翘曲器]；格子多于 5 时改为 [格0..格N-1, 翘曲器]，needs[i] 始终对应 storage[i]。
    /// - 传送带入塔、分拣器取货只认 needs 前 6 位：原版取不到时，把 needs[6..] 每 6 个一组再试一次。
    /// - 无人机/运输船卸货（AddItem）只到第 6 格，沙盒保持数量（UpdateKeepMode）只到第 5 格。
    /// </summary>
    [HarmonyPatch]
    public static class StationLogicPatch
    {
        private const int VanillaNeedsLength = 6;
        private const int WarperItemId = 1210;

        [ThreadStatic]
        private static int[] needsChunk;

        private static int[] GetNeedsChunk(int[] needs, int start)
        {
            int[] chunk = needsChunk ?? (needsChunk = new int[VanillaNeedsLength]);
            bool any = false;
            for (int k = 0; k < VanillaNeedsLength; k++)
            {
                int idx = start + k;
                chunk[k] = idx < needs.Length ? needs[idx] : 0;
                any |= chunk[k] > 0;
            }
            return any ? chunk : null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.UpdateNeeds))]
        [YummiHarness.Guard("87833a246c679fc8")]
        public static void UpdateNeeds_Postfix(StationComponent __instance)
        {
            StationStore[] storage = __instance.storage;
            int len = storage.Length;
            if (len <= VanillaNeedsLength - 1)
            {
                return;
            }
            lock (storage)
            {
                int[] needs = __instance.needs;
                if (needs == null || needs.Length != len + 1)
                {
                    needs = new int[len + 1];
                }
                bool anyFront = false;
                bool anyBack = false;
                for (int i = 0; i < len; i++)
                {
                    needs[i] = storage[i].count < storage[i].max ? storage[i].itemId : 0;
                }
                needs[len] = __instance.isStellar && __instance.warperCount < __instance.warperMaxCount ? WarperItemId : 0;
                for (int i = 0; i < needs.Length; i++)
                {
                    if (needs[i] == 0)
                    {
                        continue;
                    }
                    if (i < VanillaNeedsLength)
                    {
                        anyFront = true;
                    }
                    else
                    {
                        anyBack = true;
                    }
                }
                // 分拣器在 needs 前 6 位全为 0 时直接空闲，不会去取后面的格子；放一个不对应任何物品的非零值唤醒它。
                if (!anyFront && anyBack)
                {
                    needs[0] = -1;
                }
                __instance.needs = needs;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.UpdateInputSlots))]
        [YummiHarness.Guard("0f49824d6f59bbd9")]
        public static void UpdateInputSlots_Prefix(StationComponent __instance, out long __state)
        {
            __state = 0L;
            if (__instance.needs == null || __instance.needs.Length <= VanillaNeedsLength)
            {
                return;
            }
            SlotData[] slots = __instance.slots;
            for (int i = 0; i < slots.Length && i < 64; i++)
            {
                if (slots[i].dir == IODir.Input && slots[i].counter == 0 && slots[i].beltId != 0)
                {
                    __state |= 1L << i;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.UpdateInputSlots))]
        [YummiHarness.Guard("0f49824d6f59bbd9")]
        public static void UpdateInputSlots_Postfix(StationComponent __instance, CargoTraffic traffic, long __state)
        {
            if (__state == 0L)
            {
                return;
            }
            int[] needs = __instance.needs;
            SlotData[] slots = __instance.slots;
            BeltComponent[] beltPool = traffic.beltPool;
            for (int i = 0; i < slots.Length && i < 64; i++)
            {
                if ((__state & (1L << i)) == 0L)
                {
                    continue;
                }
                ref SlotData slot = ref slots[i];
                if (slot.counter != 0 || slot.beltId == 0)
                {
                    continue;
                }
                CargoPath cargoPath = traffic.GetCargoPath(beltPool[slot.beltId].segPathId);
                if (cargoPath == null)
                {
                    continue;
                }
                for (int start = VanillaNeedsLength; start < needs.Length; start += VanillaNeedsLength)
                {
                    int[] chunk = GetNeedsChunk(needs, start);
                    if (chunk == null)
                    {
                        continue;
                    }
                    int itemId = cargoPath.TryPickItemAtRear(chunk, out int chunkIdx, out byte stack, out byte inc);
                    if (chunkIdx >= 0)
                    {
                        __instance.InputItem(itemId, start + chunkIdx, stack, inc);
                        slot.storageIdx = start + chunkIdx + 1;
                        slot.counter = 1;
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.PickFrom),
            new[] { typeof(int), typeof(int), typeof(int), typeof(int[]), typeof(byte), typeof(byte) },
            new[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
        [YummiHarness.Guard("c5ca122186e845f3")]
        public static void PickFrom_Entity_Postfix(PlanetFactory __instance, ref int __result, int entityId, int offset, int filter, int[] needs, ref byte stack, ref byte inc)
        {
            if (__result != 0 || needs == null || needs.Length <= VanillaNeedsLength)
            {
                return;
            }
            for (int start = VanillaNeedsLength; start < needs.Length; start += VanillaNeedsLength)
            {
                int[] chunk = GetNeedsChunk(needs, start);
                if (chunk == null)
                {
                    continue;
                }
                __result = __instance.PickFrom(entityId, offset, filter, chunk, out stack, out inc);
                if (__result != 0)
                {
                    return;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.PickFrom),
            new[] { typeof(uint), typeof(int), typeof(int), typeof(int[]), typeof(byte), typeof(byte) },
            new[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
        [YummiHarness.Guard("fc3da46f7f47bcc2")]
        public static void PickFrom_Typed_Postfix(PlanetFactory __instance, ref int __result, uint ioTargetTypedId, int offset, int filter, int[] needs, ref byte stack, ref byte inc)
        {
            if (__result != 0 || needs == null || needs.Length <= VanillaNeedsLength)
            {
                return;
            }
            for (int start = VanillaNeedsLength; start < needs.Length; start += VanillaNeedsLength)
            {
                int[] chunk = GetNeedsChunk(needs, start);
                if (chunk == null)
                {
                    continue;
                }
                __result = __instance.PickFrom(ioTargetTypedId, offset, filter, chunk, out stack, out inc);
                if (__result != 0)
                {
                    return;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.AddItem))]
        [YummiHarness.Guard("806f415ada529868")]
        public static void AddItem_Postfix(StationComponent __instance, ref int __result, int itemId, int count, int inc)
        {
            const int vanillaSlots = 6;
            StationStore[] storage = __instance.storage;
            if (__result != 0 || itemId <= 0 || storage.Length <= vanillaSlots)
            {
                return;
            }
            lock (storage)
            {
                for (int i = vanillaSlots; i < storage.Length; i++)
                {
                    if (storage[i].itemId == itemId)
                    {
                        storage[i].count += count;
                        storage[i].inc += inc;
                        __result = count;
                        return;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.UpdateKeepMode))]
        [YummiHarness.Guard("368147d80661e649")]
        public static void UpdateKeepMode_Postfix(StationComponent __instance)
        {
            const int vanillaSlots = 5;
            StationStore[] storage = __instance.storage;
            if (storage.Length <= vanillaSlots)
            {
                return;
            }
            lock (storage)
            {
                for (int i = vanillaSlots; i < storage.Length; i++)
                {
                    ref StationStore store = ref storage[i];
                    if (store.keepMode > 0 && store.itemId > 0)
                    {
                        store.count = store.keepMode <= 2 ? store.max / store.keepMode : 0;
                        store.inc = store.keepMode <= 2 ? (int)(store.count * store.keepIncRatio + 1E-05f) : 0;
                    }
                }
            }
        }
    }
}
