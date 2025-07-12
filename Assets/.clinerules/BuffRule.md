---
scope: buff
priority: 5
depends: [NodeRule.md]
---

# Buff节点规则

## 专属方法
- addbuffasset(fileName): 创建一个新的BuffAsset文件
- addbuffnode(path, portPath, typeName, json): 在Buff文件中添加节点

## Buff特殊规范
- 效果持续时间单位必须为秒
- 必须包含effectType枚举字段
- 复合Buff实现标准：
  - 相关Buff放入同一Asset文件
  - 文件命名反映组合效果
  - 根节点添加关联关系注释

## 路径规范
- 基本结构：`[index].fieldName.ListName[index]`
- 单Buff结构示例：`[0].effects[0]`
- 多Buff结构示例：`[1].children[0]`
