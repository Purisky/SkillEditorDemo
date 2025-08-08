using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TreeNode.Runtime;
using TreeNode.Runtime.Generated;

namespace SkillEditorDemo.PerformanceServices
{
    #region 回退访问器实现
    
    /// <summary>
    /// 反射访问器提供者 - 作为最终回退机制
    /// </summary>
    internal class ReflectionAccessorProvider : INodeAccessorProvider
    {
        private readonly ConcurrentDictionary<Type, INodeAccessor> _cache = new ConcurrentDictionary<Type, INodeAccessor>();
        
        public INodeAccessor GetAccessor(Type nodeType)
        {
            return _cache.GetOrAdd(nodeType, type => new ReflectionNodeAccessor(type));
        }
        
        public void RegisterAccessor(Type nodeType, INodeAccessor accessor)
        {
            _cache.AddOrUpdate(nodeType, accessor, (key, oldValue) => accessor);
        }
        
        public bool TryGetAccessor(Type nodeType, out INodeAccessor accessor)
        {
            if (nodeType != null && typeof(JsonNode).IsAssignableFrom(nodeType))
            {
                accessor = GetAccessor(nodeType);
                return true;
            }
            
            accessor = null;
            return false;
        }
    }
    
    /// <summary>
    /// 反射节点访问器 - 最基础的实现
    /// </summary>
    internal class ReflectionNodeAccessor : INodeAccessor
    {
        private readonly Type _nodeType;
        private readonly List<MemberInfo> _childMembers;
        private readonly Dictionary<string, int> _renderOrderMap;
        
        public ReflectionNodeAccessor(Type nodeType)
        {
            _nodeType = nodeType ?? throw new ArgumentNullException(nameof(nodeType));
            _childMembers = new List<MemberInfo>();
            _renderOrderMap = new Dictionary<string, int>();
            
            AnalyzeChildMembers();
        }
        
        public Type GetNodeType() => _nodeType;
        
        public void CollectChildren(JsonNode node, List<JsonNode> children)
        {
            if (node == null || children == null) return;
            
            foreach (var member in _childMembers)
            {
                try
                {
                    var value = GetMemberValue(member, node);
                    if (value == null) continue;
                    
                    if (value is JsonNode childNode)
                    {
                        children.Add(childNode);
                    }
                    else if (value is System.Collections.IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            if (item is JsonNode child)
                            {
                                children.Add(child);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWarning($"[ReflectionNodeAccessor] 访问成员 {member.Name} 失败: {ex.Message}");
                }
            }
        }
        
        public void CollectChildrenWithMetadata(JsonNode node, List<(JsonNode, string, int)> children)
        {
            if (node == null || children == null) return;
            
            foreach (var member in _childMembers)
            {
                try
                {
                    var value = GetMemberValue(member, node);
                    if (value == null) continue;
                    
                    var renderOrder = GetRenderOrder(member.Name);
                    
                    if (value is JsonNode childNode)
                    {
                        children.Add((childNode, member.Name, renderOrder));
                    }
                    else if (value is System.Collections.IEnumerable enumerable)
                    {
                        int index = 0;
                        foreach (var item in enumerable)
                        {
                            if (item is JsonNode child)
                            {
                                children.Add((child, $"{member.Name}[{index}]", renderOrder + index));
                            }
                            index++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWarning($"[ReflectionNodeAccessor] 访问成员 {member.Name} 失败: {ex.Message}");
                }
            }
        }
        
        public int GetRenderOrder(string memberName)
        {
            return _renderOrderMap.TryGetValue(memberName, out var order) ? order : 1000;
        }
        
        public void CollectChildrenToBuffer(JsonNode node, JsonNode[] buffer, out int count)
        {
            count = 0;
            if (node == null || buffer == null) return;
            
            var tempList = new List<JsonNode>();
            CollectChildren(node, tempList);
            
            for (int i = 0; i < Math.Min(tempList.Count, buffer.Length); i++)
            {
                buffer[i] = tempList[i];
                count++;
            }
        }
        
        public bool HasChildren(JsonNode node)
        {
            if (node == null) return false;
            
            foreach (var member in _childMembers)
            {
                try
                {
                    var value = GetMemberValue(member, node);
                    if (value != null)
                    {
                        if (value is JsonNode) return true;
                        if (value is System.Collections.IEnumerable enumerable)
                        {
                            foreach (var item in enumerable)
                            {
                                if (item is JsonNode) return true;
                            }
                        }
                    }
                }
                catch
                {
                    // 忽略访问错误
                }
            }
            
            return false;
        }
        
        public int GetChildCount(JsonNode node)
        {
            if (node == null) return 0;
            
            int count = 0;
            foreach (var member in _childMembers)
            {
                try
                {
                    var value = GetMemberValue(member, node);
                    if (value != null)
                    {
                        if (value is JsonNode)
                        {
                            count++;
                        }
                        else if (value is System.Collections.IEnumerable enumerable)
                        {
                            foreach (var item in enumerable)
                            {
                                if (item is JsonNode) count++;
                            }
                        }
                    }
                }
                catch
                {
                    // 忽略访问错误
                }
            }
            
            return count;
        }
        
        private void AnalyzeChildMembers()
        {
            var members = _nodeType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => (m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field))
                .Where(m => HasChildAttribute(m) || HasTitlePortAttribute(m))
                .ToList();
            
            foreach (var member in members)
            {
                _childMembers.Add(member);
                _renderOrderMap[member.Name] = CalculateRenderOrder(member);
            }
        }
        
        private bool HasChildAttribute(MemberInfo member)
        {
            // 动态查找 ChildAttribute
            var childAttr = member.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name == "ChildAttribute");
            return childAttr != null;
        }
        
        private bool HasTitlePortAttribute(MemberInfo member)
        {
            // 动态查找 TitlePortAttribute
            var titlePortAttr = member.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name == "TitlePortAttribute");
            return titlePortAttr != null;
        }
        
        private int CalculateRenderOrder(MemberInfo member)
        {
            int order = 1000; // 默认顺序
            
            if (HasTitlePortAttribute(member))
            {
                order = 0;
            }
            else if (HasChildAttribute(member))
            {
                // 尝试获取 Require 属性
                var childAttr = member.GetCustomAttributes()
                    .FirstOrDefault(attr => attr.GetType().Name == "ChildAttribute");
                
                if (childAttr != null)
                {
                    var requireProperty = childAttr.GetType().GetProperty("Require");
                    if (requireProperty != null && requireProperty.GetValue(childAttr) is bool require)
                    {
                        order = require ? 100 : 200;
                    }
                    else
                    {
                        order = 200; // 默认为非必需
                    }
                }
            }
            
            return order + Math.Abs(member.Name.GetHashCode()) % 100;
        }
        
        private object GetMemberValue(MemberInfo member, object obj)
        {
            return member.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)member).GetValue(obj),
                MemberTypes.Field => ((FieldInfo)member).GetValue(obj),
                _ => null
            };
        }
        
        private void LogWarning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }
    }
    
    /// <summary>
    /// 默认节点访问器 - 空实现，确保系统稳定性
    /// </summary>
    internal class DefaultNodeAccessor : INodeAccessor
    {
        private readonly Type _nodeType;
        
        public DefaultNodeAccessor(Type nodeType)
        {
            _nodeType = nodeType ?? typeof(JsonNode);
        }
        
        public Type GetNodeType() => _nodeType;
        
        public void CollectChildren(JsonNode node, List<JsonNode> children) { }
        
        public void CollectChildrenWithMetadata(JsonNode node, List<(JsonNode, string, int)> children) { }
        
        public int GetRenderOrder(string memberName) => 1000;
        
        public void CollectChildrenToBuffer(JsonNode node, JsonNode[] buffer, out int count) 
        { 
            count = 0; 
        }
        
        public bool HasChildren(JsonNode node) => false;
        
        public int GetChildCount(JsonNode node) => 0;
    }
    
    #endregion
}