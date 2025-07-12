---
scope: skill  
priority: 5
depends: [NodeRule.md]
---

# Skill节点规则

## 专属方法
- addskillasset(fileName): 创建一个新的SkillAsset文件
- addskillnode(path, portPath, typeName, json): 在Skill文件中添加节点

## Skill特殊规范
- 必须包含cooldown字段（单位：秒）
- 伤害值必须为整数
- 技能范围类型枚举标准
- 必须指定目标选择类型（单体/群体/区域）
