# TypeCacheSystem与代码生成一致性开发指南

## 📋 概述

本指南规定了任何对反射信息/TypeCacheSystem的修改都必须同步考虑Generator~.PrecompiledTypeInfo中代码生成的修改，以保持运行时反射与编译时代码生成的一致性。

## 🎯 核心原则

### 1. 双向同步原则
- 运行时TypeCacheSystem的任何结构或逻辑修改必须在PrecompiledTypeInfo代码生成器中同步实现
- 代码生成器产生的预编译信息必须与运行时反射产生的结果完全一致

### 2. 优先级原则
- 系统优先使用预编译的TypeReflectionInfo（通过IPropertyAccessor接口）
- 当预编译信息不可用时，回退到运行时反射构建

## 🔧 需要保持一致的关键组件

### 1. TypeReflectionInfo 结构

#### 基础类型信息
```csharp
// 运行时分析方法 vs 编译时生成
IsUserDefinedType         ↔ AnalyzeIsUserDefinedType()
ContainsJsonNode          ↔ AnalyzeContainsJsonNode()
MayContainNestedJsonNode  ↔ AnalyzeMayContainNestedJsonNode()
HasParameterlessConstructor ↔ AnalyzeHasParameterlessConstructor()
Constructor              ↔ 生成构造函数委托代码
```

#### Attribute 信息
```csharp
// 运行时提取 vs 编译时生成
NodeInfo        ↔ GenerateAttributeInfo() - NodeInfoAttribute
AssetFilter     ↔ GenerateAttributeInfo() - AssetFilterAttribute  
PortColor       ↔ GenerateAttributeInfo() - PortColorAttribute
```

### 2. UnifiedMemberInfo 生成

#### 成员基础信息
```csharp
// 运行时分析 vs 编译时生成
MemberType               ↔ 区分Property/Field
ValueType                ↔ member.Type分析
Category                 ↔ DetermineMemberCategory()
IsChild/IsTitlePort      ↔ Attribute分析
ShowInNode               ↔ ShowInNodeAttribute分析
RenderOrder              ↔ CalculateRenderOrder()
GroupName                ↔ GetGroupName()
IsMultiValue             ↔ 集合类型检测
MayContainNestedStructure ↔ AnalyzeMayContainNestedStructure()
MayContainNestedJsonNode  ↔ AnalyzeMayContainNestedJsonNode()
```

#### 成员Attribute信息
```csharp
// 运行时提取 vs 编译时生成
ShowInNodeAttribute  ↔ GenerateMemberAttributeInfo()
LabelInfoAttribute   ↔ GenerateMemberAttributeInfo() 
StyleAttribute       ↔ GenerateMemberAttributeInfo()
GroupAttribute       ↔ GenerateMemberAttributeInfo()
OnChangeAttribute    ↔ GenerateMemberAttributeInfo()
DropdownAttribute    ↔ GenerateMemberAttributeInfo()
TitlePortAttribute   ↔ GenerateMemberAttributeInfo()
```

#### 访问器委托
```csharp
// 运行时创建 vs 编译时生成
Getter  ↔ GenerateGetterInitializer() - 生成Lambda表达式代码
Setter  ↔ GenerateSetterInitializer() - 处理结构体/值类型/引用类型
```

## 📝 修改检查清单

### ✅ 当修改TypeCacheSystem时需要检查：

1. **类型分析方法修改**
   - [ ] IsUserDefinedType逻辑 → 更新AnalyzeIsUserDefinedType()
   - [ ] ContainsJsonNode逻辑 → 更新AnalyzeContainsJsonNode()
   - [ ] MayContainNestedJsonNode逻辑 → 更新AnalyzeMayContainNestedJsonNode()
   - [ ] HasParameterlessConstructor逻辑 → 更新AnalyzeHasParameterlessConstructor()

2. **成员分析方法修改**
   - [ ] 成员类别判断逻辑 → 更新DetermineMemberCategory()
   - [ ] 渲染顺序计算逻辑 → 更新CalculateRenderOrder()
   - [ ] 分组名称提取逻辑 → 更新GetGroupName()
   - [ ] 嵌套结构分析逻辑 → 更新AnalyzeMayContainNestedStructure()

3. **Attribute提取方法修改**
   - [ ] 类型Attribute提取 → 更新GenerateAttributeInfo()
   - [ ] 成员Attribute提取 → 更新GenerateMemberAttributeInfo()
   - [ ] Attribute初始化器格式 → 更新GenerateAttributeInitializer()

4. **访问器生成修改**
   - [ ] Getter委托创建 → 更新GenerateGetterInitializer()
   - [ ] Setter委托创建 → 更新GenerateSetterInitializer()
   - [ ] 结构体属性处理 → 同步特殊处理逻辑

### ✅ 当修改PrecompiledTypeInfo时需要检查：

1. **生成代码结构**
   - [ ] 确保生成的TypeReflectionInfo结构与运行时一致
   - [ ] 确保AllMembers列表排序逻辑与运行时一致
   - [ ] 确保MemberLookup字典构建正确

2. **类型兼容性**
   - [ ] 生成的委托签名与运行时期望一致
   - [ ] Attribute实例化代码正确
   - [ ] 类型转换和空值处理一致

3. **性能考量**
   - [ ] 避免在生成代码中使用运行时反射
   - [ ] 确保生成的委托高效执行

## 🔍 验证方法

### 1. 单元测试
```csharp
// 对比运行时反射与预编译信息是否一致
[Test]
public void TestPrecompiledVsRuntimeConsistency()
{
    var type = typeof(TestNode);
    var runtimeInfo = BuildTypeInfoViaReflection(type);
    var precompiledInfo = GetPrecompiledTypeInfo(type);
    
    AssertTypeInfoConsistency(runtimeInfo, precompiledInfo);
}
```

### 2. 集成测试
- 使用相同的测试用例验证运行时和预编译路径
- 确保序列化/反序列化行为一致
- 验证UI渲染结果一致

### 3. 性能测试
- 对比访问速度：预编译 vs 运行时反射
- 确保预编译版本有明显性能优势

## ⚠️ 常见陷阱

### 1. 编译时与运行时环境差异
- 编译时无法访问运行时动态信息
- 注意Unity特定类型的处理差异
- Attribute参数的编译时常量限制

### 2. 类型安全问题  
- 生成的强类型委托需要正确的类型转换
- 结构体vs引用类型的setter处理差异
- 空值和默认值的处理一致性

### 3. 性能假设
- 避免在生成代码中意外引入反射调用
- 注意Lambda表达式的捕获和性能影响

## 🚀 最佳实践

### 1. 渐进式迁移
- 新功能优先实现预编译版本
- 逐步将现有类型迁移到预编译模式
- 保持运行时反射作为后备方案

### 2. 测试驱动开发
- 先写验证一致性的测试
- 修改任一侧时立即运行对比测试
- 建立持续集成检查

### 3. 文档同步
- 修改时同步更新相关注释
- 保持示例代码的准确性
- 记录已知限制和边界情况

## 📋 修改流程

1. **分析影响范围** - 确定修改会影响哪些组件
2. **同步实现** - 在运行时和编译时同时修改
3. **运行测试** - 验证一致性和正确性
4. **性能验证** - 确保预编译版本性能优势
5. **文档更新** - 更新相关文档和注释

---

**重要提醒**: 任何破坏此一致性的修改都可能导致难以调试的运行时错误，请务必严格遵循此指南进行开发。
