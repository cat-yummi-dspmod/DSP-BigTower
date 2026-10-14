using HarmonyLib;
using UnityEngine;

namespace BigTower
{
    /// <summary>
    /// 在总控面板储物栏翻页时吞掉滚轮，避免同时缩放镜头。
    /// </summary>
    [HarmonyPatch]
    public static class ControlPanelWheelZoomGuard
    {
        private static bool SuppressZoom()
        {
            try
            {
                return ControlPanelStoragePaging.CursorOverPagingArea();
            }
            catch
            {
                return false;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VFInput), "get__cameraZoomIn")]
        [YummiHarness.Guard("7af092698a0fcc7e")]
        public static bool ZoomIn_Prefix(ref float __result)
        {
            if (!SuppressZoom())
            {
                return true;
            }
            __result = 0f;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VFInput), "get__cameraZoomOut")]
        [YummiHarness.Guard("4f5adcab95ca0a54")]
        public static bool ZoomOut_Prefix(ref float __result)
        {
            if (!SuppressZoom())
            {
                return true;
            }
            __result = 0f;
            return false;
        }
    }
}
