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
        public static string AddNode(string path, string nodePath, string typeName, string json)
        {
            Type type = GetValidType(typeName);
            if (type == null)
            {
                try
                {
                    TreeNodeGraphWindow window = GetWindow(path);
                    Type assetType =  window.JsonAsset.Data.GetType();
                    string text = string.Join("\n\t", GetNodes(null).Where(n => IsValidForAssetType(n.Type, assetType)).Select(n => n.HeadInfo()));
                    throw new ArgumentException(
                        $"需要添加的类型({typeName})不存在\n" +
                        $"当前资源({assetType.Name})允许的Node类型:\n{text}");
                }
                catch (Exception)
                {
                    string text = string.Join("\n\t", GetNodes(null).Select(n => n.HeadInfo()));
                    throw new ArgumentException(
                    $"需要添加的类型({typeName})不存在\n" +
                    $"以下是所有可用的Node信息:\n{text}");
                }
            }
            if (type.IsAbstract) { 
                
                string text = string.Join("\n\t", GetNodes(type).Select(n => n.HeadInfo()));
                throw new ArgumentException($"无法添加抽象类型({typeName}),请使用具体的子类:\n{text}"); }
            return AddNode(path, nodePath, type, json);
        }

        static Dictionary<string, Type> ValidNodeTypes;
        static Dictionary<string, Type> ValidAssetTypes;

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

        static Dictionary<string, Type> InitAssetTypes()
        {
            Dictionary<string, Type> assetTypes = new();
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.Inherited(typeof(TreeNodeAsset)) && !t.IsAbstract))
            {
                // 使用完整类名作为key
                assetTypes.Add(type.Name, type);
                
                // 如果类名以Asset结尾，也添加去掉Asset后缀的小写形式作为别名
                if (type.Name.EndsWith("Asset"))
                {
                    string simpleName = type.Name[..^5]; // 去掉"Asset"后缀
                    assetTypes.TryAdd(simpleName.ToLower(), type);
                }
            }
            return assetTypes;
        }

        public static Type GetValidType(string typeName)
        {
            ValidNodeTypes ??= InitNodes();
            return ValidNodeTypes.GetValueOrDefault(typeName);
        }

        public static Type GetValidAssetType(string assetTypeName)
        {
            ValidAssetTypes ??= InitAssetTypes();
            
            if (string.IsNullOrEmpty(assetTypeName))
            {
                return null;
            }

            // 首先尝试直接匹配
            if (ValidAssetTypes.TryGetValue(assetTypeName, out Type directMatch))
            {
                return directMatch;
            }

            // 如果没有找到，检查是否需要添加"Asset"后缀
            if (!assetTypeName.EndsWith("Asset"))
            {
                string withAssetSuffix = assetTypeName + "Asset";
                if (ValidAssetTypes.TryGetValue(withAssetSuffix, out Type withSuffixMatch))
                {
                    return withSuffixMatch;
                }
            }

            // 最后尝试小写匹配（用于简化名称）
            return ValidAssetTypes.GetValueOrDefault(assetTypeName.ToLower());
        }

        /// <summary>
        /// Opens a TreeNodeGraphWindow for the given file path
        /// </summary>
        private static TreeNodeGraphWindow OpenAsset(string path)
        {
            if (path.StartsWith("Assets/")) { path = path[7..]; }
            return JsonAssetHandler.OpenJsonAsset($"Assets/{path}");
        }

        /// <summary>
        /// Validates if a path exists in the node structure
        /// </summary>
        private static void ValidatePath(TreeNodeGraphWindow window, PAPath nodePath)
        {
            int index = 0;
            window.GraphView.Asset.Data.Nodes.ValidatePath(ref nodePath, ref index);
            if (index < nodePath.Depth - 1)
            {
                int index_ = 0;
                object endObject = window.GraphView.Asset.Data.Nodes.GetValueInternal<object>(ref nodePath,ref index_);
                throw new ArgumentException($"路径无效: 在'{nodePath.GetSubPath(0, index)}'(类型:{endObject?.GetType().TypeName()})下找不到'{nodePath.GetSubPath(index)}'");
            }
        }

        /// <summary>
        /// Validates JSON property against a type and checks for nested node issues
        /// </summary>
        private static void ValidateJsonProperty(Type type, JProperty jp, string nodePath)
        {
            MemberInfo[] members = type.GetMember(jp.Name);
            if (members.Length == 0)
            {
                string promptText = "";
                if (Prompts.TryGetValue(type.Name, out var prompt) && prompt is NodePrompt nodePrompt)
                {
                    promptText = "\n" + nodePrompt.ListDetail();
                }
                throw new FieldAccessException (@$"节点操作失败,{type.Name}中不应存在{jp.Name}字段,严格按照以下信息操作数据:{promptText}");
            }

            Type valueType = members[0].GetValueType();
            object value = jp.Value.Value<object>();
            if (valueType.Inherited(typeof(JsonNode)) ||
                (valueType.Inherited(typeof(IList)) && value is IList list &&
                 list.Count > 0 && valueType.GetGenericArguments()[0].Inherited(typeof(JsonNode))))
            {
                throw new InvalidOperationException($"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({nodePath}.{jp.Name})添加该节点:{valueType.TypeName()}");
            }



            if (valueType == typeof(FuncValue))
            {
                if (jp.Value.Type != JTokenType.Object)
                {
                    throw new FormatException($"节点操作失败,解析错误(FuncValue): {jp}");
                }
                if (jp.Value["Node"] != null && jp.Value["Node"].Type != JTokenType.Null)
                {
                    throw new InvalidOperationException($"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({nodePath}.{jp.Name}.Node)添加该节点: FuncNode");
                }
            }
            if (valueType == typeof(Model.TimeValue))
            {
                if (jp.Value.Type != JTokenType.Object)
                {
                    throw new FormatException($"节点操作失败,解析错误(TimeValue): {jp}");
                }
                if (jp.Value["Value"] is JToken valueJToken && valueJToken["Node"] != null && valueJToken["Node"].Type != JTokenType.Null)
                {
                    throw new InvalidOperationException($"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({nodePath}.{jp.Name}.Value.Node)添加该节点: FuncNode");
                }
            }
        }

        /// <summary>
        /// Saves changes and returns success message
        /// </summary>
        private static string SaveChanges(TreeNodeGraphWindow window, bool refresh = false)
        {
            //window.History.AddStep();
            window.SaveChanges();
            if (refresh)
            {
                window.Refresh();
            }
            return "Success";
        }

        static string AddNode(string filePath, string nodePath, Type type, string json)
        {
            TreeNodeGraphWindow window = GetWindow(filePath);

            ChildPort port = null;
            PAPath pAPath = new(nodePath);
            if (pAPath.ItemOfCollection)
            {
                pAPath = pAPath.GetParent();
            }
            if (!pAPath.IsEmpty)
            {
                ValidatePath(window, pAPath);
                port = window.GraphView.GetPort(pAPath);
                if (port == null) 
                {
                    int index_ = 0;
                    object obj = window.GraphView.Asset.Data.Nodes.GetValueInternal<object>(ref pAPath, ref index_);
                    if (type.Inherited(typeof(NumNode)))
                    {
                        if (obj is FuncValue funcValue)
                        {
                            pAPath = pAPath.Append(nameof(FuncValue.Node));
                        }
                        else if (obj is Model.TimeValue timeValue)
                        {
                            pAPath = pAPath.Append(nameof(Model.TimeValue.Value)).Append(nameof(FuncValue.Node));
                        }
                    }

                    else
                    {
                        throw new ArgumentException($"路径({pAPath})不是节点或者节点的集合");
                    }

                    throw new ArgumentException($"{pAPath}:路径类型不是节点或者节点的集合"); 
                }
                if (!port.portType.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"无法将节点({type.Name})添加到该路径,请检查路径类型({port.portType})是否与要添加的节点类型兼容");
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
                    ValidateJsonProperty(type, jp, nodePath);
                }

                jsonNode = (JsonNode)Json.Get(type, json);
            }
            int index = 0;
            PAPath path = pAPath;
            if (!path.IsEmpty)
            {
                window.GraphView.Asset.Data.Nodes.ValidatePath(ref path, ref index);
                if (path.Depth - 1 > index)
                {
                    throw new ArgumentException($"设置节点失败:路径不合法({nodePath})非法路径({path.GetSubPath(index)})");
                }
            }
            
            if (!window.GraphView.SetNodeByPath(jsonNode, nodePath))
            {
                throw new InvalidOperationException($"设置节点失败:目标路径({nodePath})无法添加({type.Name})");
            }




            // ✅ 使用新的连接支持方法 - 修复工具节点连接缺失问题
            window.GraphView.AddViewNodeWithConnection(jsonNode, path);
            window.GraphView.FormatNodes();
            if (port is NumPort numPort)
            {
                numPort.DisplayPopupText();
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

        public static List<NodePrompt> GetNodesByName(string typeName, string assetType)
        {
            List<NodePrompt> baseNodes;
            if (string.IsNullOrEmpty(typeName) || typeName.ToLower() == "null") 
            { 
                baseNodes = GetNodes(null); 
            }
            else
            {
                Type type = GetValidType(typeName);
                if (type == null) { return new(); }
                baseNodes = GetNodes(type);
            }

            // 如果指定了assetType，则进行过滤
            if (!string.IsNullOrEmpty(assetType) && assetType.ToLower() != "null")
            {
                return FilterNodesByAssetType(baseNodes, assetType);
            }

            return baseNodes;
        }

        /// <summary>
        /// 根据资源类型过滤节点
        /// </summary>
        private static List<NodePrompt> FilterNodesByAssetType(List<NodePrompt> nodes, string assetType)
        {
            Type assetTypeObj = GetValidAssetType(assetType);
            if (assetTypeObj == null)
            {
                // 如果未找到对应的资源类型，返回所有节点
                return nodes;
            }

            return nodes.Where(n => IsValidForAssetType(n.Type, assetTypeObj)).ToList();
        }

        /// <summary>
        /// 使用TypeReflectionInfo.IsAllowedInAsset判断节点类型是否适用于指定资源类型
        /// </summary>
        private static bool IsValidForAssetType(Type nodeType, Type assetType)
        {
            try
            {
                var typeInfo = TypeCacheSystem.GetTypeInfo(nodeType);
                return typeInfo.IsAllowedInAsset(assetType);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"Failed to check asset filter for {nodeType.Name}: {ex.Message}");
                return true; // 出错时默认允许
            }
        }

        public static List<NodePrompt> GetNodes(Type type)
        {
            if (type == null)
            {
                return Prompts.Values.OfType<NodePrompt>().ToList();
            }
            return Prompts.Values.OfType<NodePrompt>().Where(n => n.Type.Inherited(type) || n.Type == type).ToList();
        }


        static TreeNodeGraphWindow GetWindow(string path)
        {
            FileCheck(ref path);
            TreeNodeGraphWindow window = OpenAsset(path);
            if (window == null) { throw new InvalidOperationException("窗口创建失败"); }
            return window;
        }

        public static string ModifyNode(string path, string nodePath, string json)
        {
            TreeNodeGraphWindow window = GetWindow( path);
            ValidatePath(window, nodePath);


            object obj = window.GraphView.Asset.Data.GetValue<object>(nodePath);
            if (obj == null) 
            { 
                throw new ArgumentException("object not found at path"); 
            }
            if (obj is not JsonNode existNode)
            {
                if (obj is FuncValue)
                {
                    throw new InvalidOperationException($"目标:{nodePath} 是FuncValue类型,请使用AddNode({nodePath}.Node)添加节点");
                }
                else
                {
                    throw new InvalidOperationException($"目标:{nodePath} 不是继承自JsonNode的节点类型");
                }
            }

            Type type = existNode.GetType();
            bool success = false;
            foreach (JProperty jp in JObject.Parse(json).Properties())
            {
                ValidateJsonProperty(type, jp, nodePath);

                success |= existNode.SetValue(type, jp.Name, jp.Value);
            }

            if (success)
            {
                return SaveChanges(window, true);
            }

            throw new InvalidOperationException("Failed to modify node");
        }

        public static string RemoveNode(string path, string nodePath, bool recursive = true)
        {
            TreeNodeGraphWindow window = GetWindow(path);
            JsonNode existNode = window.GraphView.Asset.Data.GetValue<JsonNode>(nodePath);
            if (existNode == null) 
            { 
                throw new ArgumentException("node not found at path"); 
            }

            ViewNode viewNode = window.GraphView.NodeDic[existNode];
            PropertyAccessor.RemoveValue(window.GraphView.Asset.Data.Nodes, nodePath);
            if (!recursive)
            {
                window.GraphView.Asset.Data.Nodes.AddRange(viewNode.GetChildNodes().Select(n => n.Data));
            }
            window.GraphView.FormatNodes();
            return SaveChanges(window, true);
        }

        public static string ValidateAsset(string path)
        {
            TreeNodeGraphWindow window = GetWindow(path);
            return window.GraphView.Validate();
        }

        public static List<(string, string)> GetValidPortPath(string path)
        {
            TreeNodeGraphWindow window = GetWindow(path);
            return window.GraphView.GetAllNodePaths();
        }

        public static string GetAssetTreeView(string path)
        {
            TreeNodeGraphWindow window = GetWindow(path);
            return window.GraphView.GetTreeView();
        }

        public static void FileCheck(ref string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                string error = "File path is null or empty";
                Debug.LogError(error);
                throw new ArgumentException(error);
            }
            if (!path.EndsWith(".ja") && !path.EndsWith(".tpl"))
            {
                string error = $"不支持后缀不为.ja或者.tpl的文件";
                Debug.LogError(error);
                throw new NotSupportedException(error);
            }
            if (path.StartsWith("Assets/")) { path = path[7..]; }
            if (!File.Exists($"Assets/{path}"))
            {
                string error = $"文件不存在: {path}";
                throw new FileNotFoundException(error);
            }
        }
    }
}
