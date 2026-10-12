# Changelog

## 1.2.2

- Thunderstore / BepInEx title and description wording (English first, then Chinese).

## 1.2.1

- **Control panel**: mouse wheel / Page Up–Down paging for extra station slots in the Total Control Panel (vanilla shows only 6).
- **Incompatible with LogisticsStationSlots**: if both are installed, BigTower disables itself, logs an error, and shows a message on the main menu.
- Thunderstore / BepInEx display name and description (English first, then Chinese).

## 1.2.0

- **ExpansionMode** replaces `TargetSlots`: `max` (default) or `safe`.
- **max**: planetary logistics stations **8** slots, interstellar **9** (vanilla **4** / **5**). Planetary stations no longer expand to 9 slots (fixes errors with vanilla logic).
- **safe**: all stations **6** slots (best compatibility after uninstalling the mod).
- README and Thunderstore description updated.

## 1.1.0

- Config `TargetSlots`: choose **9** (default) or **6** slots for all stations.

## 1.0.0

- Auto-expand on build / first open station window; window shows all slots (scaled if too tall).
- Extra slots: drones, vessels, belt in/out, sorters, sandbox keep-amount; expanded layout kept in vanilla saves after uninstall (with vanilla limits on hidden slots).
- Dependency: BepInEx 5.4.17.

---

# 更新日志

## 1.2.2

- Thunderstore / BepInEx 标题与描述文案（英文在前、中文在后）。

## 1.2.1

- **总控面板**：储物栏滚轮 / Page Up–Down 翻页，可查看第 7 格及以后（原版只显示 6 格）。
- **与 LogisticsStationSlots 不兼容**：同时安装时 BigTower 自动停用，写 Error 日志并在主菜单弹窗提示。
- Thunderstore / BepInEx 显示名称与描述：英文在前、中文在后。

## 1.2.0

- 配置项改为 **ExpansionMode**（`max` / `safe`），不再使用 `TargetSlots`。
- **max**（默认）：行星内 **8** 格、星际 **9** 格（原版 **4** / **5**）；行星内不再扩到 9 格，修复与原版冲突导致的报错。
- **safe**：全部运输站 **6** 格，卸载后原版兼容性最好。
- 更新 README 与 Thunderstore 描述。

## 1.1.0

- 配置 `TargetSlots`：可选 **9**（默认）或 **6** 格。

## 1.0.0

- 新建 / 首次打开窗口时自动扩容；窗口显示全部格子（过高时缩小）。
- 扩格支持无人机、运输船、传送带、分拣器、沙盒保持数量；卸载后扩容仍写入原版存档（多出的格子在原版中部分可用）。
- 依赖 BepInEx 5.4.17。
