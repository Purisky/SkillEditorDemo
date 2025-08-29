# SkillEditorDemo

## 项目概述
基于Unity的可视化技能编辑器Demo，支持通过节点图的方式设计和编辑技能、Buff等游戏逻辑。该项目集成了TreeNode节点编辑器系统和MCP4Unity工具服务，为游戏开发者提供了一个强大的可视化逻辑编辑解决方案。

<iframe src="//player.bilibili.com/player.html?isOutside=true&aid=115105492631965&bvid=BV1nEh1zdEGm&cid=31998741810&p=1" scrolling="no" border="0" frameborder="no" framespacing="0" allowfullscreen="true"></iframe>

## 核心特性

### 🎯 可视化节点编辑
- **技能编辑器**：通过节点图方式设计技能逻辑，支持复杂的技能组合和效果链
- **Buff编辑器**：可视化Buff效果编辑，支持触发器、条件判断和动作执行
- **树状结构**：基于TreeNode系统，专门针对游戏逻辑优化的树形节点编辑器

### 🛠️ 技术架构
- **TreeNode 0.1.0**：基于Unity UI Toolkit的节点编辑器系统
  - JSON数据存储，便于版本控制和外部处理
  - 源代码生成技术，消除运行时反射开销
  - 支持自定义节点类型和属性绘制器
  - 内置中英文国际化支持

- **MCP4Unity**：Unity编辑器扩展工具服务
  - 内置HTTP服务器，支持远程工具调用
  - 自动扫描和注册工具方法
  - 基于Model Context Protocol SDK

## 安装说明

### 安装步骤

1. **克隆项目**
   ```bash
   git clone https://github.com/Purisky/SkillEditorDemo.git
   cd SkillEditorDemo
   ```

2. **初始化子模块**
   ```bash
   git submodule update --init --recursive
   ```

3. **构建源代码生成器**
   ```bash
   cd "Assets/TreeNode/Generator~"
   dotnet build TreeNodeSourceGenerator.csproj
   ```