using HarmonyLib;
using UnityEngine;

namespace BigTower
{
    /// <summary>
    /// 原版运输塔窗口只创建 6 行存储 UI；这里按格子数补足行数，窗口高度仍由原版按格子数计算。
    /// 窗口高过屏幕时整体等比缩小。
    /// </summary>
    [HarmonyPatch]
    public static class StationWindowPatch
    {
        private const int RowTop = -90;
        private const int RowHeight = 76;
        private const float ScreenMargin = 80f;

        private static readonly AccessTools.FieldRef<UIStationWindow, UIStationStorage[]> storageUIsRef =
            AccessTools.FieldRefAccess<UIStationWindow, UIStationStorage[]>("storageUIs");

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStationWindow), "OnStationIdChange")]
        [YummiHarness.Guard("a94c837d579532f7")]
        public static void OnStationIdChange_Prefix(UIStationWindow __instance)
        {
            if (!__instance.active || __instance.transport == null || __instance.stationId <= 0)
            {
                return;
            }
            StationComponent station = __instance.transport.stationPool[__instance.stationId];
            if (station == null || station.id != __instance.stationId || station.storage == null)
            {
                return;
            }
            StationExpandPatch.Expand(station);
            EnsureRows(__instance, station.storage.Length);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStationWindow), "OnStationIdChange")]
        [YummiHarness.Guard("a94c837d579532f7")]
        public static void OnStationIdChange_Postfix(UIStationWindow __instance)
        {
            FitToScreen(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStationWindow), "RefreshTrans")]
        [YummiHarness.Guard("5d8291ee6f5744c2")]
        public static void RefreshTrans_Postfix(UIStationWindow __instance)
        {
            FitToScreen(__instance);
        }

        private static void EnsureRows(UIStationWindow window, int count)
        {
            UIStationStorage[] rows = storageUIsRef(window);
            if (rows.Length >= count)
            {
                return;
            }
            UIStationStorage[] expanded = new UIStationStorage[count];
            rows.CopyTo(expanded, 0);
            for (int i = rows.Length; i < count; i++)
            {
                UIStationStorage row = Object.Instantiate(window.storageUIPrefab, window.storageUIPrefab.transform.parent);
                ((RectTransform)row.transform).anchoredPosition = new Vector2(40f, RowTop - RowHeight * i);
                row.stationWindow = window;
                row._Create();
                if (window.inited)
                {
                    row._Init(window.data);
                }
                expanded[i] = row;
            }
            storageUIsRef(window) = expanded;
        }

        private static void FitToScreen(UIStationWindow window)
        {
            RectTransform windowTrans = window.windowTrans;
            Canvas canvas = windowTrans.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                return;
            }
            float height = windowTrans.sizeDelta.y;
            float available = ((RectTransform)canvas.rootCanvas.transform).rect.height - ScreenMargin;
            float scale = height > available && height > 0f ? available / height : 1f;
            windowTrans.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
