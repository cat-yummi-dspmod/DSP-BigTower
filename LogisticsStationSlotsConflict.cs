using System;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace BigTower
{
    /// <summary>
    /// 与 LogisticsStationSlots 功能重叠且易双重补丁，二者不可同时启用。
    /// </summary>
    internal static class LogisticsStationSlotsConflict
    {
        internal const string LogisticsStationSlotsGuid = "local.logisticsstationslots";

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

        internal const string ErrorLog =
            "LogisticsStationSlots and BigTower are incompatible. BigTower has been disabled; uninstall one of them.\n\n" +
            "LogisticsStationSlots 与 BigTower 存在明确的不兼容。BigTower 已停用，请卸载其中一个 mod。";

        internal static void RegisterMainMenuWarning(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(VFPreload), nameof(VFPreload.InvokeOnLoadWorkEnded)),
                postfix: new HarmonyMethod(typeof(LogisticsStationSlotsConflict), nameof(OnMainMenuOpen)));
        }

        private static bool messageShown;

        public static void OnMainMenuOpen()
        {
            if (messageShown)
            {
                return;
            }
            messageShown = true;
            UIMessageBox.Show(
                "BigTower",
                ErrorLog,
                "确定".Translate(),
                UIMessageBox.ERROR);
        }
    }
}
