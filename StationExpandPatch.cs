using System;
using HarmonyLib;

namespace BigTower
{
    /// <summary>
    /// 新建运输塔时、打开运输塔窗口前，把格子扩到 TargetSlots。
    /// 只加长 storage 与 priorityLocks 数组，不改存档格式，扩容结果由原版 Export/Import 原样读写。
    /// </summary>
    [HarmonyPatch]
    public static class StationExpandPatch
    {
        internal static int TargetSlots = 9;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.Init))]
        public static void StationComponent_Init_Postfix(StationComponent __instance)
        {
            Expand(__instance);
        }

        internal static void Expand(StationComponent station)
        {
            if (station.isCollector || station.isVeinCollector || station.storage == null || station.storage.Length >= TargetSlots)
            {
                return;
            }
            lock (station.storage)
            {
                StationStore[] storage = new StationStore[TargetSlots];
                Array.Copy(station.storage, storage, station.storage.Length);
                StationPriorityLock[] locks = new StationPriorityLock[TargetSlots];
                if (station.priorityLocks != null)
                {
                    Array.Copy(station.priorityLocks, locks, Math.Min(station.priorityLocks.Length, TargetSlots));
                }
                station.priorityLocks = locks;
                station.storage = storage;
            }
        }
    }
}
