---
scope: skill  
priority: 5
depends: [NodeRule.md]
---

# Skill节点规则

## 专属方法
- addskillasset(fileName): 创建一个新的SkillAsset文件

## 工具使用指南
- 使用ListNodes(null, "SkillAsset")获取适用于Skill资源的节点类型
- 使用GetNodePrompts(typeNames)批量查询具体节点的详细结构