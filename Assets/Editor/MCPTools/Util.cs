using Newtonsoft.Json.Linq;
using SkillEditorDemo.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TreeNode;
using TreeNode.Editor;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkillEditorDemo
{
    public static class ToolUtil
    {
        public static string AddNode(string path, string portPath, string typeName, string json)
        {
            Type type = GetValidType(typeName);
            if (type is null) { return $"需要添加的类型({typeName})不存在,使用ListNodes获取可用的Node信息"; }
            return AddNode(path, portPath, type, json);
        }
        
        static Dictionary<string, Type> ValidNodeTypes;
        static Dictionary<string, Type> InitNodes()
        {
            Dictionary<string, Type> nodes = new();
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(n => n.Inherited(typeof(JsonNode)))
                )
            {
                nodes.Add(type.Name, type);
            }
            return nodes;
        }
        
        public static Type GetValidType(string typeName)
        {
            ValidNodeTypes ??= InitNodes();
            return ValidNodeTypes.GetValueOrDefault(typeName);
        }
        
        /// <summary>
        /// Opens a TreeNodeGraphWindow for the given file path
        /// </summary>
        private static TreeNodeGraphWindow OpenAsset(string path)
        {
            return JsonAssetHandler.OpenJsonAsset($"Assets/{path}");
        }
        
        /// <summary>
        /// Validates if a path exists in the node structure
        /// </summary>
        private static string ValidatePath(TreeNodeGraphWindow window, string portPath, out int index)
        {
            if (!PropertyAccessor.GetValidPath(window.GraphView.Asset.Data.Nodes, portPath, out index))
            {
                string validPath = portPath[..(index)].TrimEnd('.');
                Type validType = null;
                object parent = PropertyAccessor.TryGetParent(window.GraphView.Asset.Data.Nodes, validPath, out string last);
                if (last.StartsWith('[') && parent is IList list)
                {
                    if (int.TryParse(last[1..^1], out int index2) && index2 < list.Count)
                    {
                        validType = list[index2].GetType();
                    }
                }
                else
                {
                    validType = parent.GetType().GetMember(last)[0].GetValueType();
                }
                return $"路径无效: 在'{validPath}'(类型:{validType?.TypeName()})下找不到'{portPath[index..]}'";
            }
            return null; // Path is valid
        }
        
        /// <summary>
        /// Validates JSON property against a type and checks for nested node issues
        /// </summary>
        private static string ValidateJsonProperty(Type type, JProperty jp, string portPath)
        {
            MemberInfo[] members = type.GetMember(jp.Name);
            if (members.Length == 0)
            {
                string promptText = "";
                if (Prompts.TryGetValue(type.Name, out var prompt) && prompt is NodePrompt nodePrompt)
                {
                    promptText = "\n" + nodePrompt.ListDetail();
                }
                return @$"节点操作失败,{type.Name}中应不存在{jp.Name}字段,严格按照以下信息操作数据:{promptText}";
            }
            
            Type valueType = members[0].GetValueType();
            object value = jp.Value.Value<object>();
            if (valueType.Inherited(typeof(JsonNode)) || 
                (valueType.Inherited(typeof(IList)) && value is IList list && 
                 list.Count > 0 && valueType.GetGenericArguments()[0].Inherited(typeof(JsonNode))))
            {
                return $"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({portPath}.{jp.Name})添加该节点:{valueType.TypeName()}";
            }
            if (valueType == typeof(FuncValue) &&
                jp.Value["Node"] != null)
            { 
                return $"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({portPath}.{jp.Name}.Node)添加该节点: FuncNode";
            }
            if (valueType == typeof(Model.TimeValue) &&
                jp.Value["Value"] is JToken valueJToken&&
                valueJToken["Node"]!= null)
            {
                return $"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({portPath}.{jp.Name}.Value.Node)添加该节点: FuncNode";
            }
            return null; // Validation passed
        }
        
        /// <summary>
        /// Saves changes and returns success message
        /// </summary>
        private static string SaveChanges(TreeNodeGraphWindow window, bool refresh = false)
        {
            window.History.AddStep();
            window.SaveChanges();
            if (refresh)
            {
                window.Refresh();
            }
            return "Success";
        }
        
        static string AddNode(string filePath, string portPath, Type type, string json)
        {
            TreeNodeGraphWindow window = OpenAsset(filePath);
            if (window == null) { return "文件不存在"; }
            
            ChildPort port = null;
            if (!string.IsNullOrEmpty(portPath))
            {
                int index;
                string pathError = ValidatePath(window, portPath, out index);
                if (pathError != null) return pathError;
                string path = portPath;
                if (portPath.EndsWith(".Node"))
                {
                    object parent = PropertyAccessor.TryGetParent(window.GraphView.Asset.Data.Nodes, portPath, out string last);
                    if (parent is FuncValue)
                    {
                        path = portPath[..^5];
                    }
                }
                port = window.GraphView.GetPort(path);
                if (port == null) { return $"{path}:路径类型不是节点或者节点的集合"; }
                if (!port.portType.IsAssignableFrom(type)) {
                    return $"无法将节点({type.Name})添加到该路径,请检查路径类型({port.portType})是否与要添加的节点类型兼容";
                }
            }
            
            JsonNode jsonNode = null;
            if (string.IsNullOrEmpty(json))
            {
                jsonNode = (JsonNode)Activator.CreateInstance(type);
            }
            else
            {
                foreach (JProperty jp in JObject.Parse(json).Properties())
                {
                    string validationError = ValidateJsonProperty(type, jp, portPath);
                    if (validationError != null) return validationError.Replace("节点操作失败", "节点添加失败");
                }
                
                jsonNode = (JsonNode)Json.Get(type, json);
            }

            if (!window.GraphView.SetNodeByPath(jsonNode, portPath))
            {
                return $"设置节点失败:目标路径({portPath})无法添加({type.Name})";
            }
            window.GraphView.AddViewNode(jsonNode, port);
            window.GraphView.FormatNodes();
            if (port is NumPort numPort)
            {
                numPort.TryPopUpText();
            }
            return SaveChanges(window); 
        }
        
        public static Dictionary<string, BasePrompt> Prompts = InitPrompts();
        
        static Dictionary<string, BasePrompt> InitPrompts()
        {
            Dictionary<string, BasePrompt> prompts = new();
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(n => n.Inherited(typeof(JsonNode))))
            {
                NodePrompt nodePrompt = new(type);
                nodePrompt.HandleFields(prompts);
                prompts[type.Name] = nodePrompt;
            }
            return prompts;
        }
        
        public static List<NodePrompt> GetNodesByName(string typeName)
        {
            if (typeName == null || typeName.ToLower() == "null") { return GetNodes(null); }
            Type type = GetValidType(typeName);
            if (type == null) { return new(); }
            return GetNodes(type);
        }
        
        public static List<NodePrompt> GetNodes(Type type)
        {
            if (type == null)
            {
                return Prompts.Values.OfType<NodePrompt>().ToList();
            }
            return Prompts.Values.OfType<NodePrompt>().Where(n => n.Type.Inherited(type)|| n.Type== type).ToList();
        }

        public static string ModifyNode(string path, string portPath, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return "Skip: json is empty";
            }

            TreeNodeGraphWindow window = OpenAsset(path);
            if (window == null) { return "file not exist"; }

            int index;
            string pathError = ValidatePath(window, portPath, out index);
            if (pathError != null) return pathError;


            object obj = window.GraphView.Asset.Data.GetValue<object>(portPath);
            if (obj == null) { return "object not found at path"; }
            if (obj is not JsonNode existNode)
            {
                if (obj is FuncValue)
                {
                    return $"目标:{portPath} 是FuncValue类型,请使用AddNode({portPath}.Node)添加节点";
                }
                else
                {
                    return $"目标:{portPath} 不是继承自JsonNode的节点类型";
                }
            }

            Type type = existNode.GetType();
            bool success = false;
            foreach (JProperty jp in JObject.Parse(json).Properties())
            {
                string validationError = ValidateJsonProperty(type, jp, portPath);
                if (validationError != null) return validationError.Replace("节点操作失败", "节点修改失败");

                success |= existNode.SetValue(type, jp.Name, jp.Value);
            }

            if (success)
            {
                return SaveChanges(window, true);
            }

            return "Failed to modify node";
        }

        public static string RemoveNode(string path, string portPath, bool recursive = true)
        {
            TreeNodeGraphWindow window = OpenAsset(path);
            if (window == null) { return "file not exist"; }
            
            JsonNode existNode = window.GraphView.Asset.Data.GetValue<JsonNode>(portPath);
            if (existNode == null) { return "node not found at path"; }
            
            ViewNode viewNode = window.GraphView.NodeDic[existNode];
            PropertyAccessor.SetValueNull(window.GraphView.Asset.Data.Nodes, portPath);
            if (!recursive)
            {
                window.GraphView.Asset.Data.Nodes.AddRange(viewNode.GetChildNodes().Select(n => n.Data));
            }
            window.GraphView.FormatNodes();
            return SaveChanges(window, true);
        }

        public static string ValidateAsset(string path)
        {
            TreeNodeGraphWindow window = OpenAsset(path);
            if (window == null) { return "file not exist"; }
            return window.GraphView.Validate();
        }
    }
}
