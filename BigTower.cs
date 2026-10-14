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
    [BepInDependency(YummiHarness.HarnessPlugin.PluginGuid)]
    [BepInDependency(LogisticsStationSlotsConflict.LogisticsStationSlotsGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public class BigTower : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.yummi.dsp.bigtower";
        public const string PLUGIN_NAME =
            "More station slots (max 8/9, safe 6), kept after uninstall. | 运输塔扩容（max 8/9，safe 6），卸载mod后依旧保留6格";
        public const string PLUGIN_VERSION = "1.2.3";

        private static readonly Harmony harmony = new Harmony(PLUGIN_GUID);
        private static readonly YummiHarness.PatchOptions HarnessOptions = new YummiHarness.PatchOptions { ModName = "BigTower" };
        internal static ManualLogSource Log;
        internal static BigTower Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            Log = Logger;
            Log.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} 等待主菜单就绪后再启用（若与 LogisticsStationSlots 共存将弹窗并停用）");
            MainMenuBootstrap.Init(this);
            ConflictStartup.BeginScan(this);
            // 互斥 mod 登记到 YummiHarness，与 patch 失败等问题在主菜单合并成一个弹窗
            YummiHarness.Harness.RegisterConflict("BigTower", LogisticsStationSlotsConflict.LogisticsStationSlotsGuid,
                "LogisticsStationSlots");
            YummiHarness.Harness.PatchAll(harmony, typeof(MainMenuBootstrap), HarnessOptions);
        }

        internal void ApplyPatchesAndConfig()
        {
            if (LogisticsStationSlotsConflict.IsPresent())
            {
                Log.LogError(LogisticsStationSlotsConflict.GetErrorMessage());
                return;
            }

            ConfigEntry<string> expansionMode = Config.Bind("General", "ExpansionMode", "max", new ConfigDescription(
                "max: planetary stations 8 slots, interstellar 9. safe: all stations 6 slots (best vanilla compatibility after uninstall).\n" +
                "max：行星内运输塔 8 格、星际 9 格。safe：全部 6 格（卸载后原版兼容性最好）。",
                new AcceptableValueList<string>("max", "safe")));
            StationExpandPatch.UseSafeMode = expansionMode.Value.Equals("safe", StringComparison.OrdinalIgnoreCase);

            // 每个补丁单独校验目标方法是否与原版一致；失败的只跳过自己，其余功能照常
            YummiHarness.Harness.PatchAll(harmony, typeof(StationExpandPatch), HarnessOptions);
            YummiHarness.Harness.PatchAll(harmony, typeof(StationWindowPatch), HarnessOptions);
            YummiHarness.Harness.PatchAll(harmony, typeof(StationLogicPatch), HarnessOptions);
            YummiHarness.Harness.PatchAll(harmony, typeof(ControlPanelStoragePaging), HarnessOptions);
            YummiHarness.Harness.PatchAll(harmony, typeof(ControlPanelWheelZoomGuard), HarnessOptions);

            Log.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} 已加载 (ExpansionMode={expansionMode.Value})");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
