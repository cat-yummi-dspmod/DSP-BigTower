using BepInEx;
using BepInEx.Configuration;
using YummiHarness;

namespace BigTower
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(HarnessPlugin.PluginGuid)]
    [BepInDependency(StationAssistant.Plugin.PluginGuid)]
    public class BigTower : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.yummi.dsp.bigtower";
        public const string PLUGIN_NAME = "BigTower";
        public const string PLUGIN_VERSION = "2.0.0";
        internal const string SafeBigTowerPluginGuid = "com.yummi.dsp.safebigtower";

        private void Awake()
        {
            Harness.RegisterConflict(PLUGIN_NAME, SafeBigTowerPluginGuid, "SafeBigTower");
            if (Harness.IsPluginLoaded(SafeBigTowerPluginGuid))
            {
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
            StationAssistant.Treat.ControlPanelScroll(true);
            StationAssistant.Treat.PageSize(6);
        }
    }
}
