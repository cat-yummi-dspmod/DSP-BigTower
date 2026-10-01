using System;
using HarmonyLib;

namespace BigTower
{
    /// <summary>
    /// 新建运输塔时、打开运输塔窗口前，按 ExpansionMode 扩容 storage / priorityLocks。
    /// safe：一律 6 格；max：行星内 8 格、星际 9 格（避免行星内 9 格与原版冲突）。
    /// </summary>
    [HarmonyPatch]
    public static class StationExpandPatch
    {
        internal const int SafeSlots = 6;
        internal const int PlanetaryMaxSlots = 8;
        internal const int StellarMaxSlots = 9;

        internal static bool UseSafeMode;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.Init))]
        public static void StationComponent_Init_Postfix(StationComponent __instance)
        {
            Expand(__instance);
        }

        internal static int GetTargetSlots(StationComponent station)
        {
            if (UseSafeMode)
            {
                return SafeSlots;
            }
            return station.isStellar ? StellarMaxSlots : PlanetaryMaxSlots;
        }

        internal static void Expand(StationComponent station)
        {
            if (station.isCollector || station.isVeinCollector || station.storage == null)
            {
                return;
            }
            int target = GetTargetSlots(station);
            if (station.storage.Length >= target)
            {
                return;
            }
            lock (station.storage)
            {
                StationStore[] storage = new StationStore[target];
                Array.Copy(station.storage, storage, station.storage.Length);
                StationPriorityLock[] locks = new StationPriorityLock[target];
                if (station.priorityLocks != null)
                {
                    Array.Copy(station.priorityLocks, locks, Math.Min(station.priorityLocks.Length, target));
                }
                station.priorityLocks = locks;
                station.storage = storage;
            }
        }
    }
}
