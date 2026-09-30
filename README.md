# BigTower

<details>
<summary>Read me in English</summary>

***Logistics stations with 9 storage slots***

## Overview

* Every Planetary Logistics Station and Interstellar Logistics Station gets **9 storage slots** (vanilla: 4 and 5).
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

* `BepInEx/config/com.yummi.dsp.bigtower.cfg`
  * `TargetSlots` (default `9`): number of slots after expansion.
  * Lowering the value does not shrink stations that are already expanded.

## Save compatibility

* No extra save data is written. Expanded stations are stored in the vanilla save format, so **they stay expanded even after you uninstall this mod**.
* After uninstalling, the vanilla game only partly supports the extra slots:
  * The station window shows at most 6 slots; slots 7+ are hidden and can't be edited.
  * Belt input and sorters only fill slots 1-5.
  * Drones and vessels delivering to slots 7+ **lose the items**. Clear the Demand settings on slots 7+ before uninstalling.
  * Supply from the extra slots and belt output keep working.

## Notes

* Orbital Collectors and Advanced Mining Machines are not expanded.
* The station page in the Control Panel still shows only the first 6 slots.
* Multiplayer is untested.

## CREDITS

* [Dyson Sphere Program](https://store.steampowered.com/app/1366540): The great game
* [BepInEx](https://bepinex.dev/): Base modding framework

</details>

<details>
<summary>中文说明</summary>

***运输塔扩容到 9 格***

## 功能说明

* 行星内物流运输站和星际物流运输站都扩容到 **9 格**（原版分别为 4 格和 5 格）。
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

* `BepInEx/config/com.yummi.dsp.bigtower.cfg`
  * `TargetSlots`（默认 `9`）：扩容后的格子数。
  * 调小该值不会缩减已经扩容的运输塔。

## 存档兼容

* 不写入任何额外存档数据，扩容后的运输塔以原版存档格式保存，**卸载本 mod 后扩容依然保留**。
* 卸载后原版对多出来的格子只有部分支持：
  * 运输塔窗口最多显示 6 格，第 7 格起看不到也改不了。
  * 传送带输入和分拣器只往第 1~5 格放货。
  * 无人机和运输船往第 7 格起送货时**货物会丢失**，卸载前请先清掉第 7 格起的需求设置。
  * 从多出来的格子供货、传送带输出不受影响。

## 注意事项

* 轨道采集器和大型采矿机不扩容。
* 总控面板里的运输塔页面仍只显示前 6 格。
* 未在多人游戏中测试。

## 鸣谢

* [戴森球计划](https://store.steampowered.com/app/1366540): 伟大的游戏
* [BepInEx](https://bepinex.dev/): 基础模组框架

</details>
