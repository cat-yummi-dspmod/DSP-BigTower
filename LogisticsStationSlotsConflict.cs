using System;
using BepInEx.Bootstrap;

namespace BigTower
{
    internal static class LogisticsStationSlotsConflict
    {
        internal const string LogisticsStationSlotsGuid = "local.logisticsstationslots";

        private static bool messageShown;

        internal static bool IsPresent()
        {
            if (Chainloader.PluginInfos.ContainsKey(LogisticsStationSlotsGuid))
            {
                return true;
            }
            foreach (BepInEx.PluginInfo info in Chainloader.PluginInfos.Values)
            {
                if (info.Metadata.Name.Equals("LogisticsStationSlots", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (info.Metadata.GUID.IndexOf("logisticsstationslots", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
                Type instanceType = info.Instance?.GetType();
                if (instanceType != null && instanceType.Namespace == "LogisticsStationSlots")
                {
                    return true;
                }
            }
            return false;
        }

        internal static string GetErrorMessage()
        {
            string label = $"BigTower {BigTower.PLUGIN_VERSION} | 运输塔扩容 {BigTower.PLUGIN_VERSION}";
            return
                $"Could not load [{label}] because it is incompatible with LogisticsStationSlots ({LogisticsStationSlotsGuid}). BigTower has been disabled; uninstall one of them.\n\n" +
                $"无法加载 [{label}]，因其与 LogisticsStationSlots（{LogisticsStationSlotsGuid}）不兼容。BigTower 已停用，请卸载其中一个 mod。";
        }

        internal static void ShowConflictDialog()
        {
            if (messageShown)
            {
                return;
            }
            messageShown = true;
            BigTower.Log?.LogError(GetErrorMessage());
            UIMessageBox.Show(
                "BigTower | 运输塔扩容",
                GetErrorMessage(),
                "确定".Translate(),
                UIMessageBox.ERROR);
        }
    }
}
