using System.Collections;
using BepInEx;

namespace BigTower
{
    /// <summary>
    /// 等 Chainloader 跑完再判定互斥，避免 BigTower 比 LogisticsStationSlots 更早 Awake 时漏检。
    /// </summary>
    internal static class ConflictStartup
    {
        internal static bool Blocked { get; private set; }
        internal static bool ScanComplete { get; private set; }

        internal static void BeginScan(BaseUnityPlugin host)
        {
            host.StartCoroutine(ScanAfterChainloader());
        }

        private static IEnumerator ScanAfterChainloader()
        {
            yield return null;
            ScanComplete = true;
            Blocked = LogisticsStationSlotsConflict.IsPresent();
            if (Blocked)
            {
                BigTower.Log?.LogError(LogisticsStationSlotsConflict.GetErrorMessage());
            }
        }

        internal static void OnMainMenuReady()
        {
            if (!ScanComplete)
            {
                Blocked = LogisticsStationSlotsConflict.IsPresent();
                ScanComplete = true;
            }

            if (Blocked || LogisticsStationSlotsConflict.IsPresent())
            {
                Blocked = true;
                // 弹窗交给 YummiHarness 统一提示（已在 Awake 中登记互斥），这里只写日志
                BigTower.Log?.LogError(LogisticsStationSlotsConflict.GetErrorMessage());
                return;
            }

            BigTowerPlugin.EnableFeatures();
        }
    }
}
