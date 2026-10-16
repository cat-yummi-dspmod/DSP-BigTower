using BepInEx;

namespace BigTower
{
    /// <summary>
    /// 兼容壳。不改 Station Assistant 的配置，只要求运行时把物流塔当成 8/9 格、面板可滚动、总控不滚动。
    /// </summary>
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(YummiHarness.HarnessPlugin.PluginGuid)]
    [BepInDependency(StationAssistant.Plugin.PluginGuid)]
    public class BigTower : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.yummi.dsp.bigtower";
        public const string PLUGIN_NAME = "BigTower";
        public const string PLUGIN_VERSION = "1.4.0";

        private void Awake()
        {
            StationAssistant.Treat.AutoStellarSlots(9);
            StationAssistant.Treat.AutoPlanetarySlots(8);
            StationAssistant.Treat.ManualSlotButton(true);
            StationAssistant.Treat.StationPanelScroll(true);
            StationAssistant.Treat.ControlPanelScroll(true);
            StationAssistant.Treat.PageSize(6);
            Logger.LogInfo("BigTower 1.4.0 已要求 Station Assistant 按 9/8 格运行，未修改其配置文件。");
        }
    }
}
