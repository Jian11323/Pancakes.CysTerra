# Pancakes.CysTerra

CysTerra 扩展：订阅 PancakesAPI GlobalQuake（`/gq`）。

版本 **0.4.4**。

- 报告/TTS 优先 `NameLocation` 内置地名库；上游英文 region 仅作低优先级回退
- 失效时间从本报发出起算（避免发震时刻导致刚进列表就进「已失效报告」）

LGPL-3.0-or-later。
