using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BigTower
{
    /// <summary>
    /// 总控面板原版只显示 6 格且无滚动条；在储物栏区域滚轮翻页查看扩容后的格子。
    /// </summary>
    [HarmonyPatch]
    public static class ControlPanelStoragePaging
    {
        private sealed class PageState
        {
            public int Page;
            public int LastStationId = -1;
            public Canvas Canvas;
            public float LastScroll;
        }

        private const int DefaultPageSize = 6;

        private static int pageSize = DefaultPageSize;
        private static readonly ConditionalWeakTable<StationComponent, PageState> states = new ConditionalWeakTable<StationComponent, PageState>();

        private static float nextPageTime;
        private static Text pageLabel;
        private static UIControlPanelStationInspector lastInspector;
        private static UIControlPanelStationStorage[] lastUis;

        private static readonly AccessTools.FieldRef<UIControlPanelStationInspector, UIControlPanelStationStorage[]> storageUIsRef =
            AccessTools.FieldRefAccess<UIControlPanelStationInspector, UIControlPanelStationStorage[]>("storageUIs");

        internal static bool CursorOverPagingArea()
        {
            if (lastInspector == null || lastUis == null || lastUis.Length == 0)
            {
                return false;
            }
            if (lastInspector.gameObject == null || !lastInspector.gameObject.activeInHierarchy)
            {
                return false;
            }
            StationComponent station = lastInspector.station;
            if (!StorageShowingStation(station, lastUis))
            {
                return false;
            }
            return MouseOverStorage(lastInspector, StateOf(station), lastUis);
        }

        private static int ValidCount(StationComponent station)
        {
            if (station.isCollector || station.isVeinCollector)
            {
                return station.collectionIds?.Length ?? 0;
            }
            return station.storage?.Length ?? 0;
        }

        private static PageState StateOf(StationComponent station)
        {
            if (!states.TryGetValue(station, out PageState state))
            {
                state = new PageState();
                states.Add(station, state);
            }
            return state;
        }

        private static int ClampPage(PageState state, int valid)
        {
            int maxPage = valid > pageSize ? (valid - 1) / pageSize : 0;
            if (state.Page > maxPage)
            {
                state.Page = maxPage;
            }
            if (state.Page < 0)
            {
                state.Page = 0;
            }
            return state.Page;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIControlPanelStationStorage), nameof(UIControlPanelStationStorage.SetData))]
        [YummiHarness.Guard("61111e7e94b232ee")]
        public static void SetData_Prefix(StationComponent _station, ref int _index)
        {
            if (_station == null)
            {
                return;
            }
            int valid = ValidCount(_station);
            if (valid <= pageSize)
            {
                return;
            }
            int page = ClampPage(StateOf(_station), valid);
            if (page > 0)
            {
                int mapped = page * pageSize + _index;
                if (mapped < valid)
                {
                    _index = mapped;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIControlPanelStationInspector), "_OnUpdate")]
        [YummiHarness.Guard("b89aade680ae7db9")]
        public static void OnUpdate_Postfix(UIControlPanelStationInspector __instance)
        {
            UIControlPanelStationStorage[] uis = storageUIsRef(__instance);
            StationComponent station = __instance.station;
            if (station == null || uis == null || uis.Length == 0)
            {
                return;
            }
            pageSize = uis.Length;
            int valid = ValidCount(station);
            if (valid <= pageSize)
            {
                lastInspector = null;
                lastUis = null;
                ControlPanelPageIndicator.Update(pageLabel, 0, 0);
                return;
            }
            lastInspector = __instance;
            lastUis = uis;
            PageState pageState = StateOf(station);
            if (pageState.LastStationId != station.id)
            {
                pageState.LastStationId = station.id;
                pageState.Page = 0;
            }
            EnsurePageLabel(__instance, uis);
            ControlPanelPageIndicator.Update(pageLabel, pageState.Page, (valid - 1) / pageSize);

            int delta = 0;
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                delta = 1;
            }
            else if (Input.GetKeyDown(KeyCode.PageUp))
            {
                delta = -1;
            }
            if (delta == 0)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (scroll == 0f)
                {
                    pageState.LastScroll = 0f;
                    return;
                }
                if (scroll == pageState.LastScroll)
                {
                    return;
                }
                pageState.LastScroll = scroll;
                if (Input.GetMouseButton(0) || Time.unscaledTime < nextPageTime || !StorageShowingStation(station, uis) || !MouseOverStorage(__instance, pageState, uis))
                {
                    return;
                }
                delta = scroll > 0f ? -1 : 1;
                nextPageTime = Time.unscaledTime + 0.05f;
            }
            if (delta == 0)
            {
                return;
            }
            int prev = ClampPage(pageState, valid);
            pageState.Page = prev + delta;
            pageState.Page = ClampPage(pageState, valid);
            if (pageState.Page != prev)
            {
                nextPageTime = Time.unscaledTime + 0.15f;
                Rebind(__instance, station, uis);
            }
        }

        private static void EnsurePageLabel(UIControlPanelStationInspector inspector, UIControlPanelStationStorage[] uis)
        {
            if (pageLabel != null || inspector == null || uis == null || uis.Length == 0 || uis[0] == null)
            {
                return;
            }
            RectTransform anchor = inspector.transform as RectTransform;
            Text fontSource = uis[0].GetComponentInChildren<Text>(true);
            if (anchor == null || fontSource == null)
            {
                return;
            }
            pageLabel = ControlPanelPageIndicator.Create(
                "bigtower-cpanel-page-label",
                anchor,
                fontSource,
                new Vector2(0f, -22f),
                TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f));
        }

        private static bool StorageShowingStation(StationComponent station, UIControlPanelStationStorage[] uis)
        {
            for (int i = 0; i < uis.Length; i++)
            {
                if (uis[i] == null)
                {
                    continue;
                }
                if (uis[i].station == station)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool MouseOverStorage(UIControlPanelStationInspector inspector, PageState state, UIControlPanelStationStorage[] uis)
        {
            Camera cam = CameraFor(inspector, state);
            Vector2 mouse = Input.mousePosition;
            for (int i = 0; i < uis.Length; i++)
            {
                if (uis[i] == null || !uis[i].gameObject.activeInHierarchy)
                {
                    continue;
                }
                RectTransform rt = uis[i].transform as RectTransform;
                if (rt != null && RectTransformUtility.RectangleContainsScreenPoint(rt, mouse, cam))
                {
                    return true;
                }
            }
            return false;
        }

        private static Camera CameraFor(Component c, PageState state)
        {
            if (state.Canvas == null)
            {
                state.Canvas = c.GetComponentInParent<Canvas>();
            }
            Canvas canvas = state.Canvas;
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }
            return canvas.worldCamera;
        }

        private static void Rebind(UIControlPanelStationInspector inspector, StationComponent station, UIControlPanelStationStorage[] uis)
        {
            PlanetFactory factory = inspector.factory;
            if (factory == null)
            {
                return;
            }
            int valid = ValidCount(station);
            int baseIndex = StateOf(station).Page * pageSize;
            try
            {
                for (int i = 0; i < uis.Length; i++)
                {
                    UIControlPanelStationStorage row = uis[i];
                    if (row == null)
                    {
                        continue;
                    }
                    if (baseIndex + i < valid)
                    {
                        if (!row.gameObject.activeSelf)
                        {
                            row.gameObject.SetActive(true);
                        }
                        row.SetData(factory, station, i);
                    }
                    else if (row.gameObject.activeSelf)
                    {
                        row.gameObject.SetActive(false);
                    }
                }
            }
            catch (Exception ex)
            {
                BigTower.Log?.LogWarning($"总控面板储物栏翻页刷新失败: {ex.Message}");
            }
        }
    }

    internal static class ControlPanelPageIndicator
    {
        private static readonly HashSet<string> logged = new HashSet<string>();

        internal static Text Create(string name, RectTransform anchor, Text fontSource, Vector2 offset, TextAnchor alignment, Vector2 anchorRef, Vector2 pivot)
        {
            if (anchor == null)
            {
                return null;
            }
            Font font = ResolveFont(fontSource, anchor);
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(anchor, false);
            RectTransform rt = (RectTransform)go.transform;
            rt.anchorMin = anchorRef;
            rt.anchorMax = anchorRef;
            rt.pivot = pivot;
            rt.anchoredPosition = offset;
            rt.sizeDelta = new Vector2(90f, 30f);
            rt.SetAsLastSibling();
            Text label = go.GetComponent<Text>();
            label.font = font;
            label.fontSize = fontSource != null && fontSource.fontSize > 0 ? fontSource.fontSize : 14;
            label.fontStyle = fontSource != null ? fontSource.fontStyle : FontStyle.Normal;
            label.alignment = alignment;
            label.color = new Color(0.75f, 0.88f, 1f, 0.95f);
            label.raycastTarget = false;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.text = string.Empty;
            if (logged.Add(name) && font == null)
            {
                BigTower.Log?.LogWarning("总控面板页码：找不到可用字体");
            }
            return label;
        }

        internal static void Update(Text label, int page, int maxPage)
        {
            if (label == null)
            {
                return;
            }
            bool show = maxPage > 0;
            if (label.gameObject.activeSelf != show)
            {
                label.gameObject.SetActive(show);
            }
            if (show)
            {
                string text = (page + 1) + "/" + (maxPage + 1);
                if (label.text != text)
                {
                    label.text = text;
                }
            }
        }

        private static Font ResolveFont(Text source, RectTransform anchor)
        {
            if (source != null && source.font != null)
            {
                return source.font;
            }
            Transform t = anchor;
            for (int depth = 0; depth < 8 && t != null; depth++)
            {
                Text[] texts = t.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i] != null && texts[i].font != null)
                    {
                        return texts[i].font;
                    }
                }
                t = t.parent;
            }
            return null;
        }
    }
}
