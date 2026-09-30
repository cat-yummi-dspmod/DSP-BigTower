using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace BigTower
{
    /// <summary>
    /// 运输塔新建时、打开窗口时自动扩容到 TargetSlots 格，窗口直接显示全部格子。
    /// 不改存档格式，卸载 mod 回到原版后扩容仍然保留。
    /// </summary>
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class BigTower : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.yummi.dsp.bigtower";
        public const string PLUGIN_NAME = "大塔 (Big Tower)";
        public const string PLUGIN_VERSION = "1.0.0";

        private static readonly Harmony harmony = new Harmony(PLUGIN_GUID);
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            ConfigEntry<int> targetSlots = Config.Bind("General", "TargetSlots", 9, "运输塔扩容后的格子数");
            StationExpandPatch.TargetSlots = targetSlots.Value;

            harmony.PatchAll(typeof(StationExpandPatch));
            harmony.PatchAll(typeof(StationWindowPatch));
            harmony.PatchAll(typeof(StationLogicPatch));

            Log.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} 已加载");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
