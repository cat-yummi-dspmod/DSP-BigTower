using BepInEx;
using BepInEx.Configuration;
using YummiHarness;

namespace BigTower
{
    /// <summary>
    /// 兼容壳。不改 Station Assistant 的配置，只要求运行时把物流塔当成 8/9 格并自动扩容；槽位按钮与总控翻页关闭。
    /// </summary>
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(HarnessPlugin.PluginGuid)]
    [BepInDependency(StationAssistant.Plugin.PluginGuid)]
    public class BigTower : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.yummi.dsp.bigtower";
        public const string PLUGIN_NAME = "BigTower";
        public const string PLUGIN_VERSION = "1.4.3";
        internal const string SafeBigTowerPluginGuid = "com.yummi.dsp.safebigtower";

        private void Awake()
        {
            Harness.RegisterConflict(
                PLUGIN_NAME,
                SafeBigTowerPluginGuid,
                "SafeBigTower",
                "8/9 slot preset vs vanilla-safe 6/6 preset. Uninstall one. / 8、9 格预设与原版 6 格预设冲突，请只保留一个。");

            if (Harness.IsPluginLoaded(SafeBigTowerPluginGuid))
            {
                Logger.LogError($"{PLUGIN_NAME} 与 SafeBigTower 不能同时启用，已跳过 Treat 预设。");
                return;
            }

            const int schemaVersion = 1;
            Harness.ApplyConfigSchema(Config, schemaVersion, new ConfigSchemaOptions { WipeIfBelow = schemaVersion });
            Harness.PruneConfigKeys(Config, new ConfigDefinition(Harness.ConfigSchemaSection, Harness.ConfigSchemaVersionKey));

            StationAssistant.Treat.AutoStellarSlots(9);
            StationAssistant.Treat.AutoPlanetarySlots(8);
            StationAssistant.Treat.ExpandOnBuild(true);
            StationAssistant.Treat.ExpandOnOpen(true);
            StationAssistant.Treat.ManualSlotButton(false);
            StationAssistant.Treat.StationPanelScroll(false);
            StationAssistant.Treat.ControlPanelScroll(false);
            StationAssistant.Treat.PageSize(6);
            Logger.LogInfo("BigTower 1.4.3：已要求 Station Assistant 按 9/8 格、建造/打开时扩容；未改其配置文件。旧版 BigTower cfg 已清空。");
        }
    }
}
