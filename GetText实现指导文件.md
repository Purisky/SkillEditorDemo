# GetText()方法实现指导文件

## 概述

本文档规定了所有继承自JsonNode的类中GetText()方法的实现标准，确保生成的文本描述符合中文自然语言习惯，便于人类和AI agent理解。

## 核心原则

### 1. 中文自然语言优先
- 使用符合中文语法习惯的表达方式
- 避免英文术语和技术性表达
- 优化语序，使描述更自然流畅

### 2. 简洁明了
- 去掉冗余词汇和多余标点符号
- 只显示非默认值的参数
- 使用简化的格式表达

### 3. 逻辑清晰
- 使用统一的格式规范
- 保持一致的描述风格
- 便于系统化处理和解析

## 符号使用规范

### 保留的符号类型

#### 数学比较符号
```
>    大于
<    小于  
=    等于
≠    不等于
≥    大于等于
≤    小于等于
```

#### 数学计算符号
```
+    加法
-    减法  
*    乘法
/    除法
```

#### 逻辑运算符号
```
&    逻辑与
|    逻辑或
!    逻辑非
```

### 符号使用原则
- **数学表达式保持原样**：`ATK*1.5+10` 而不是 `攻击力乘以1.5加10`
- **逻辑表达式保持简洁**：`(条件1&条件2)` 而不是 `(条件1且条件2)`
- **比较表达式直观明了**：`(HP>50)` 而不是 `(HP大于50)`

## 括号使用规范

### 必须使用括号的情况
1. **包含嵌套关系的表达式**
   ```csharp
   // 复合条件
   return $"({string.Join("&", conditions)})";
   
   // 数学计算
   return $"({left}>{right})";
   ```

2. **多个元素的逻辑组合**
   ```csharp
   // 多个单位选择
   return $"存在任意({string.Join("|", units)})单位";
   ```

### 可以省略括号的情况
1. **简单的单一条件**
   ```csharp
   // 单一条件，无嵌套
   return $"{unit}存在{buffId}";
   ```

2. **确保无歧义的简单表达**
   ```csharp
   // 明确的单一动作
   return $"物理伤害";
   ```

## 布尔值表示规范

### 标准表示
- **真值**：使用 `"真"`
- **假值**：使用 `"假"`

### 避免的表达
- ❌ "总是成立"、"任意伤害"、"true"、"false"
- ✅ "真"、"假"

```csharp
// 正确示例
if (conditions.Count == 0) return "真";

// 错误示例
if (conditions.Count == 0) return "总是成立";
```

## 条件分支格式规范

### 统一格式
```
如果{condition}:{truetext}否则:{falsetext}
```

### 格式要点
- 使用 **"如果...否则"** 而不是 "若...则"
- 使用 **冒号 (:)** 分隔条件和结果
- 去掉多余的逗号和连接词

### 实现示例
```csharp
// 标准条件分支
return $"如果{condition}:{trueAction}否则:{falseAction}";

// 成功失败分支
return $"尝试{action},如果成功:{successAction}否则:{failAction}";
```

## 语序优化规则

### 1. 修饰语前置
```csharp
// ✅ 正确：修饰语前置
return $"{unit}存在同源的{buffId}";

// ❌ 错误：修饰语后置
return $"{unit}存在{buffId}(同源)";
```

### 2. 动词前置
```csharp
// ✅ 正确：动词前置
return $"存在任意({unitText})单位";

// ❌ 错误：宾语前置
return $"({unitText}中存在任意单位)";
```

### 3. 介词优化
```csharp
// ✅ 正确：自然介词
return $"从{target}移除{buffId}";

// ❌ 错误：别扭介词
return $"移除{target}的{buffId}";
```

### 4. 连接词简化
```csharp
// ✅ 正确：简洁连接
return $"当{condition}时终止事件";

// ❌ 错误：冗余标点
return $"当{condition}时,终止事件";
```

## 类型名称中文化

### 伤害类型映射
```csharp
string dmgTypeText = DmgType switch
{
    DmgType.Physic => "物理",
    DmgType.Fire => "火焰",
    DmgType.Frost => "冰霜", 
    DmgType.Lightning => "闪电",
    _ => DmgType.ToString()
};
```

### 操作类型映射
```csharp
string modTypeText = ModType switch
{
    ValueModType.Set => "设置为",
    ValueModType.Add => "增加",
    ValueModType.Multiply => "乘以",
    _ => "修改为"
};
```

### 属性修饰映射
```csharp
List<string> properties = new List<string>();
if (Direct) properties.Add("直接");        // 不是"直接伤害"
if (Dodge_able) properties.Add("可闪避");
if (Crit_able) properties.Add("可暴击");
```

## 参数显示规范

### 只显示非默认值
```csharp
List<string> details = new List<string>();
if (levelText != "1") details.Add($"等级{levelText}");      // 默认值1不显示
if (degreeText != "1") details.Add($"层数{degreeText}");    // 默认值1不显示
if (!string.IsNullOrEmpty(param0Text)) details.Add($"参数{param0Text}");
```

### 简化格式
```csharp
// ✅ 正确：简洁格式
string detailText = $"({string.Join(",", details)})";

// ❌ 错误：冗余格式  
string detailText = $"(等级:{level},层数:{degree})";
```

## 常见实现模式

### 1. 简单条件判断
```csharp
public override string GetText()
{
    if (简单条件)
    {
        return "简单描述";  // 无括号
    }
    return $"复杂描述({details})";  // 有括号
}
```

### 2. 条件分支处理
```csharp
public override string GetText()
{
    string trueAction = True?.GetText() ?? "无事发生";
    string falseAction = False?.GetText() ?? "无事发生";
    return $"如果{condition}:{trueAction}否则:{falseAction}";
}
```

### 3. 列表处理
```csharp
public override string GetText()
{
    if (list == null || list.Count == 0) return "真";
    if (list.Count == 1) return list[0].GetText();
    return $"({string.Join("&", list.Select(n => n.GetText()))})";
}
```

### 4. 参数组装
```csharp
public override string GetText()
{
    List<string> parts = new List<string>();
    if (condition1) parts.Add("部分1");
    if (condition2) parts.Add("部分2");
    
    return parts.Count == 0 ? "真" : 
           parts.Count == 1 ? parts[0] : 
           $"({string.Join("且", parts)})";
}
```

## 示例对照表

### 条件表达示例
| 场景 | ❌ 错误示例 | ✅ 正确示例 |
|------|------------|------------|
| 简单比较 | HP大于50 | (HP>50) |
| 数学计算 | 攻击力乘以2加10 | (ATK*2+10) |
| 逻辑组合 | 条件1且条件2 | (条件1&条件2) |
| 单位存在 | [敌人,友军]中存在任意单位 | 存在任意(敌人\|友军)单位 |
| Buff检测 | 敌人.存在Buff(燃烧) | 敌人存在燃烧Buff |

### 动作表达示例
| 场景 | ❌ 错误示例 | ✅ 正确示例 |
|------|------------|------------|
| 条件分支 | 若HP>50则添加Buff,否则无事发生 | 如果(HP>50):添加Buff否则:无事发生 |
| 伤害描述 | 对敌人造成100点Physic伤害(直接伤害,可暴击) | 对敌人造成100点物理伤害(直接,可暴击) |
| Buff操作 | 为敌人添加燃烧Buff(等级:2,层数:1) | 为敌人添加燃烧Buff(等级2) |
| 移除操作 | 移除敌人的护盾Buff,成功时:XXX | 从敌人移除护盾Buff,如果成功:XXX否则:YYY |

## 实现检查清单

### ✅ 语言规范检查
- [ ] 使用中文自然语言描述
- [ ] 避免英文术语和技术表达
- [ ] 语序符合中文习惯

### ✅ 符号使用检查  
- [ ] 数学符号保持原样 (+,-,*,/,>,<,=,≠,≥,≤)
- [ ] 逻辑符号保持原样 (&,|,!)
- [ ] 括号使用恰当（嵌套用括号，简单可省略）

### ✅ 格式规范检查
- [ ] 布尔值使用"真"/"假"
- [ ] 条件分支使用"如果:否则:"格式
- [ ] 参数只显示非默认值
- [ ] 类型名称已中文化

### ✅ 实现质量检查
- [ ] 处理null值情况
- [ ] 单条件时简化显示
- [ ] 列表为空时有合理默认值
- [ ] 格式保持一致性

## 维护说明

本指导文件应该：
1. **定期更新**：随着新的节点类型和需求变化及时修订
2. **团队共享**：确保所有开发者遵循相同标准
3. **示例验证**：新增示例前先验证其正确性
4. **版本控制**：重要修改需要记录变更原因

---

**版本**: 1.0  
**更新日期**: 2025年8月19日  
**适用范围**: 所有继承自JsonNode的类的GetText()方法实现
