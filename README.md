# BigTower

<details>
<summary>Read me in English</summary>

***More storage slots on logistics stations***

## Overview

* Vanilla: Planetary Logistics Stations get **4** slots, Interstellar Logistics Stations get **5**.
* **max** (default): Planetary Logistics Stations get **8** slots, Interstellar Logistics Stations get **9**. Planetary stations are capped at 8 to avoid conflicts with vanilla logic at 9 slots.
* **safe**: Every station gets **6** slots.
* New stations expand on build; existing ones expand the first time you open their window.
* The station window shows all slots; if it is taller than the screen, it scales down.
* Extra slots support drones, vessels, belts, sorters, sandbox keep-amount, and copy/paste like vanilla slots.
* Works well with TowerFactory.

## Config

* `BepInEx/config/com.yummi.dsp.bigtower.cfg`, or the r2modman Config editor
  * `ExpansionMode`: `max` (default) or `safe`.
  * Lowering the mode does not shrink stations that are already expanded.

## Save compatibility

* No extra save data. Expanded stations stay expanded after uninstalling this mod.
* After uninstalling, vanilla only partly supports extra slots (window shows 6, belt/sorter input only slots 1–5, deliveries to slots 7+ can lose items). **safe** (6 slots) is the least risky.

## Notes

* Orbital Collectors and Advanced Mining Machines are not expanded.
* Control Panel station view still shows only 6 slots.
* Multiplayer is untested.

## CREDITS

* [Dyson Sphere Program](https://store.steampowered.com/app/1366540): The great game
* [BepInEx](https://bepinex.dev/): Base modding framework

## Author

* cat yummi

</details>

<details>
<summary>中文说明</summary>

***运输塔扩容更多格子***

## 功能说明

* 原版情况下，行星内物流运输站 **4** 格、星际物流运输站 **5** 格。
* **max**（默认）：行星内物流运输站 **8 格**，星际物流运输站 **9 格**。行星内限制为 8 格，避免 9 格与原版逻辑冲突报错。
* **safe**：所有运输站统一 **6 格**。
* 新建即扩容；已有运输塔在首次打开窗口时扩容。
* 窗口显示全部格子；过高时整体缩小。
* 扩出来的格子支持无人机、运输船、传送带、分拣器、沙盒保持数量、复制粘贴等。
* 可与塔厂（TowerFactory）配合使用。

## 配置

* `BepInEx/config/com.yummi.dsp.bigtower.cfg`，或 r2modman 的 Config editor
  * `ExpansionMode`：`max`（默认）或 `safe`。
  * 调低模式不会缩减已经扩容的运输塔。

## 存档兼容

* 不写入额外存档数据，卸载后扩容仍保留。
* 卸载后原版对多出来的格子只有部分支持（窗口最多 6 格、传送带/分拣器只认前 5 格、第 7 格起送货可能丢货）。**safe**（6 格）相对最稳妥。

## 注意事项

* 轨道采集器和大型采矿机不扩容。
* 总控面板仍只显示前 6 格。
* 未在多人游戏中测试。

## 鸣谢

* [戴森球计划](https://store.steampowered.com/app/1366540): 伟大的游戏
* [BepInEx](https://bepinex.dev/): 基础模组框架

## 制作人

* 流浪法师.悠米

</details>
