# BigTower

<details>
<summary>Read me in English</summary>

***Logistics stations with 6 or 9 storage slots***

## Overview

* Every Planetary Logistics Station and Interstellar Logistics Station gets **9 storage slots** by default, or **6** if you prefer (vanilla: 4 and 5).
  * Newly built stations are expanded right away.
  * Existing stations are expanded the first time you open their window.
* The station window shows all slots at once. If the window gets taller than the screen, it is scaled down to fit.
* The extra slots work just like the vanilla ones:
  * Logistics drones and vessels (supply and demand)
  * Belt input and output
  * Sorters putting items into the station
  * Sandbox mode "keep amount"
  * Copy / paste settings and blueprints
* Works well with TowerFactory: more slots means room for recipes with more ingredients.

## Config

* `BepInEx/config/com.yummi.dsp.bigtower.cfg`, or the r2modman Config editor
  * `TargetSlots`: `9` (default) or `6`.
    * `6`: the safe choice. The vanilla game still fully supports all 6 slots after you uninstall this mod (see below).
    * `9`: more slots, but only partly supported by the vanilla game after uninstalling.
  * Switching from 9 to 6 does not shrink stations that already have 9 slots.

## Save compatibility

* No extra save data is written. Expanded stations are stored in the vanilla save format, so **they stay expanded even after you uninstall this mod**.
* How the vanilla game handles the extra slots after uninstalling:
  * The station window shows 6 slots; slots 7+ are hidden and can't be edited.
  * Drones and vessels deliver into slots 1-6. Deliveries to slots 7+ **lose the items**, so clear the Demand settings on slots 7+ before uninstalling.
  * Belt input and sorters only fill slots 1-5.
  * Supply from every slot and belt output keep working.
* In short: with `TargetSlots = 6`, the only thing you lose after uninstalling is belt / sorter input into slot 6.

## Notes

* Orbital Collectors and Advanced Mining Machines are not expanded.
* The station page in the Control Panel still shows only the first 6 slots.
* Multiplayer is untested.

## CREDITS

* [Dyson Sphere Program](https://store.steampowered.com/app/1366540): The great game
* [BepInEx](https://bepinex.dev/): Base modding framework

## Author

* cat yummi

</details>

<details>
<summary>中文说明</summary>

***运输塔扩容到 6 或 9 格***

## 功能说明

* 行星内物流运输站和星际物流运输站默认扩容到 **9 格**，也可以选 **6 格**（原版分别为 4 格和 5 格）。
  * 新建的运输塔建好即扩容。
  * 已有的运输塔在第一次打开窗口时扩容。
* 运输塔窗口直接显示全部格子；窗口高过屏幕时会整体缩小。
* 扩出来的格子和原版格子用法完全一样：
  * 物流无人机和运输船（供应、需求）
  * 传送带输入、输出
  * 分拣器往塔里放货
  * 沙盒模式的保持数量
  * 复制粘贴设置和蓝图
* 可以和塔厂（TowerFactory）配合使用：格子多了，原料多的配方也放得下。

## 配置

* `BepInEx/config/com.yummi.dsp.bigtower.cfg`，或 r2modman 的 Config editor
  * `TargetSlots`：`9`（默认）或 `6`。
    * `6`：稳妥的选择，卸载本 mod 后原版仍能完整支持全部 6 格（见下文）。
    * `9`：格子更多，但卸载后原版只能部分支持。
  * 从 9 改成 6 不会缩减已经是 9 格的运输塔。

## 存档兼容

* 不写入任何额外存档数据，扩容后的运输塔以原版存档格式保存，**卸载本 mod 后扩容依然保留**。
* 卸载后原版对多出来的格子的支持情况：
  * 运输塔窗口显示 6 格，第 7 格起看不到也改不了。
  * 无人机和运输船能往第 1~6 格送货；往第 7 格起送货时**货物会丢失**，卸载前请先清掉第 7 格起的需求设置。
  * 传送带输入和分拣器只往第 1~5 格放货。
  * 所有格子的供货、传送带输出不受影响。
* 简单来说：`TargetSlots = 6` 时，卸载后唯一失去的只是传送带和分拣器往第 6 格放货。

## 注意事项

* 轨道采集器和大型采矿机不扩容。
* 总控面板里的运输塔页面仍只显示前 6 格。
* 未在多人游戏中测试。

## 鸣谢

* [戴森球计划](https://store.steampowered.com/app/1366540): 伟大的游戏
* [BepInEx](https://bepinex.dev/): 基础模组框架

## 制作人

* 流浪法师.悠米

</details>
