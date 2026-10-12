using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace BigTower
{
    /// <summary>
    /// 运输塔新建时、打开窗口时自动扩容；ExpansionMode 为 max 或 safe。
    /// 不改存档格式，卸载 mod 回到原版后扩容仍然保留。
    /// </summary>
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInIncompatibility(LogisticsStationSlotsConflict.LogisticsStationSlotsGuid)]
    public class BigTower : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.yummi.dsp.bigtower";
        public const string PLUGIN_NAME =
            "More station slots (max 8/9, safe 6), kept after uninstall. | 运输塔扩容（max 8/9，safe 6），卸载mod后依旧保留6格";
        public const string PLUGIN_VERSION = "1.2.2";

        private static readonly Harmony harmony = new Harmony(PLUGIN_GUID);
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            if (LogisticsStationSlotsConflict.IsPresent())
            {
                Logger.LogError(LogisticsStationSlotsConflict.ErrorLog);
                LogisticsStationSlotsConflict.RegisterMainMenuWarning(harmony);
                enabled = false;
                return;
            }

            ConfigEntry<string> expansionMode = Config.Bind("General", "ExpansionMode", "max", new ConfigDescription(
                "max: planetary stations 8 slots, interstellar 9. safe: all stations 6 slots (best vanilla compatibility after uninstall).\n" +
                "max：行星内运输塔 8 格、星际 9 格。safe：全部 6 格（卸载后原版兼容性最好）。",
                new AcceptableValueList<string>("max", "safe")));
            StationExpandPatch.UseSafeMode = expansionMode.Value.Equals("safe", StringComparison.OrdinalIgnoreCase);

            harmony.PatchAll(typeof(StationExpandPatch));
            harmony.PatchAll(typeof(StationWindowPatch));
            harmony.PatchAll(typeof(StationLogicPatch));
            harmony.PatchAll(typeof(ControlPanelStoragePaging));
            harmony.PatchAll(typeof(ControlPanelWheelZoomGuard));

            Log.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} 已加载 (ExpansionMode={expansionMode.Value})");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
