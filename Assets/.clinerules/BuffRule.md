﻿---
scope: buff
priority: 5
depends: [NodeRule.md]
---

# Buff节点规则

## 专属方法
- addbuffasset(fileName): 创建一个新的BuffAsset文件
## Buff特殊规范
 - 一个完整的Buff必须由BuffNode作为根节点构成
 - Buff的效果部分主要由TrigNode控制,由TrigNode决定Buff的触发时机,TrigNode可以为空,Buff可作为一个没有实际效果的占位Buff配合其他Buff使用
 - TrigNode只有一种,所有的触发场景都在TrigNode中使用字段定义
 BuffNode
   - TrigNode (可选)
     - ActionNode (可选)
     - ConditionNode (可选)
当创建一组相关的Buff时,可以将它们放在同一个BuffAsset中,并通过BuffNode的portPath来区分不同的Buff