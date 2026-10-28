# Pancakes（CysTerra）

[CysTerra](https://docs.eew.cryville.world/) 通用版扩展：订阅 [PancakesAPI](https://wiki.aloys23.link/wiki/PancakesAPI) 的 GlobalQuake（`gq`）通道，接入事件摘要、事件详情与语音播报。

对齐共通端 **Alpha 0.0.15**、接口层 **Cryville.EEW 0.8.14**、Features / TTS **0.8.1**。扩展拆分与官方约定一致：事件源与事件摘要同包，事件详情与 TTS 为独立扩展。

## 安装

扩展仅适用于 CysTerra **通用版**。以下三个包均需安装，版本 **0.4.0**：

| 组件 | 导入包 |
| --- | --- |
| 事件源 + 事件摘要 | `Pancakes.CysTerra-0.4.0.zip` |
| 事件详情 | `Pancakes.CysTerra.Features-0.4.0.zip` |
| 语音消息 | `Pancakes.CysTerra.TTS-0.4.0.zip` |

每个 zip 的主 dll 位于根目录，可直接导入。发布附件见本仓库 [Releases](https://github.com/Jian11323/Pancakes.CysTerra/releases)。

## 说明

- WebSocket 固定为 `wss://api.aloys23.link/api/v1/alert/ws/quake/gq`，设置中不展示，无需填写密钥或其他配置。
- 载荷按 PancakesAPI `/gq` 文档解析（`source` / `type` / `action` / `payload`）；烈度为 MMI。
- 上游没有的震级、深度、坐标不会补写。

开发说明见 [CysTerra 扩展文档](https://docs.eew.cryville.world/) 与 [PancakesAPI/gq](https://wiki.aloys23.link/wiki/PancakesAPI/gq)。

## 许可证

版权所有 © 2026 纪安。

本程序以 **GNU LGPL-3.0-or-later** 授权。LGPL 正文见 `LICENSE.txt`，其所引用的 GPL-3.0 正文见 `COPYING.txt`。对应源代码即本仓库；发布的扩展包内同时附带这两份许可证。
