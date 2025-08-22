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
using static TreeNode.Runtime.TypeCacheSystem;
using Debug = TreeNode.Utility.Debug;


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
                    Type assetType = window.JsonAsset.Data.GetType();
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
            if (type.IsAbstract)
            {

                string text = string.Join("\n\t", GetNodes(type).Select(n => n.HeadInfo()));
                throw new ArgumentException($"无法添加抽象类型({typeName}),请使用具体的子类:\n{text}");
            }
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
            //Debug.Log(index);
            if (index < nodePath.Depth - 1)
            {
                int index_ = 0;
                object endObject = window.GraphView.Asset.Data.Nodes.GetValueInternal<object>(ref nodePath, ref index_);
                throw new ArgumentException($"路径无效: 在'{nodePath.GetSubPath(0, index)}'(类型:{endObject?.GetType().TypeName()})下找不到'{nodePath.GetSubPath(index)}'");
            }
        }

        ///// <summary>
        ///// Validates JSON property against a type and checks for nested node issues
        ///// </summary>
        //private static void ValidateJsonProperty(Type type, JProperty jp, string nodePath)
        //{
        //    TypeCacheSystem.TypeReflectionInfo typeReflectionInfo = TypeCacheSystem.GetTypeInfo(type);




        //    MemberInfo[] members = type.GetMember(jp.Name);
        //    if (members.Length == 0)
        //    {
        //        string promptText = "";
        //        if (Prompts.TryGetValue(type.Name, out var prompt) && prompt is NodePrompt nodePrompt)
        //        {
        //            promptText = "\n" + nodePrompt.ListDetail();
        //        }
        //        throw new FieldAccessException (@$"节点操作失败,{type.Name}中不应存在{jp.Name}字段,严格按照以下信息操作数据:{promptText}");
        //    }

        //    Type valueType = members[0].GetValueType();
        //    object value = jp.Value.Value<object>();
        //    if (valueType.Inherited(typeof(JsonNode)) ||
        //        (valueType.Inherited(typeof(IList)) && value is IList list &&
        //         list.Count > 0 && valueType.GetGenericArguments()[0].Inherited(typeof(JsonNode))))
        //    {
        //        throw new InvalidOperationException($"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({nodePath}.{jp.Name})添加该节点:{valueType.TypeName()}");
        //    }



        //    if (valueType == typeof(FuncValue))
        //    {
        //        if (jp.Value.Type != JTokenType.Object)
        //        {
        //            throw new FormatException($"节点操作失败,解析错误(FuncValue): {jp}");
        //        }
        //        if (jp.Value["Node"] != null && jp.Value["Node"].Type != JTokenType.Null)
        //        {
        //            throw new InvalidOperationException($"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({nodePath}.{jp.Name}.Node)添加该节点: FuncNode");
        //        }
        //    }
        //    if (valueType == typeof(Model.TimeValue))
        //    {
        //        if (jp.Value.Type != JTokenType.Object)
        //        {
        //            throw new FormatException($"节点操作失败,解析错误(TimeValue): {jp}");
        //        }
        //        if (jp.Value["Value"] is JToken valueJToken && valueJToken["Node"] != null && valueJToken["Node"].Type != JTokenType.Null)
        //        {
        //            throw new InvalidOperationException($"节点操作失败,禁止嵌套添加节点: {jp.Name},使用AddNode({nodePath}.{jp.Name}.Value.Node)添加该节点: FuncNode");
        //        }
        //    }
        //}
        private static void SetNonNodeValue(JsonNode jsonNode, TypeReflectionInfo typeReflectionInfo, JProperty jp, string nodePath)
        {
            UnifiedMemberInfo memberInfo = typeReflectionInfo.GetMember(jp.Name) ?? throw new FieldAccessException(@$"节点操作失败,{typeReflectionInfo.Type.Name}中不应存在{jp.Name}字段");
            if (memberInfo.Category == MemberCategory.JsonNode)
            {
                throw new InvalidOperationException($"{jp.Name}:操作失败,禁止嵌套添加节点,使用AddNode(...,{nodePath}.{jp.Name},{memberInfo.ValueType.Name})添加该节点");
            }
            Type valueType = memberInfo.ValueType;
            if (valueType == typeof(FuncValue))
            {
                if (jp.Value.Type != JTokenType.Object)
                {
                    throw new FormatException($"{jp.Name}:操作失败,解析错误(FuncValue): {jp}");
                }
                if (jp.Value["Node"] != null && jp.Value["Node"].Type != JTokenType.Null)
                {
                    throw new InvalidOperationException($"{jp.Name}:操作失败,禁止嵌套添加节点,使用AddNode(...,{nodePath}.{jp.Name}.Node,FuncNode)添加该节点");
                }
            }
            if (valueType == typeof(Model.TimeValue))
            {
                if (jp.Value.Type != JTokenType.Object)
                {
                    throw new FormatException($"{jp.Name}:操作失败,解析错误(TimeValue): {jp}");
                }
                if (jp.Value["Value"] is JToken valueJToken && valueJToken["Node"] != null && valueJToken["Node"].Type != JTokenType.Null)
                {
                    throw new InvalidOperationException($"{jp.Name}:操作失败,禁止嵌套添加节点,使用AddNode({nodePath}.{jp.Name}.Value.Node)添加该节点: FuncNode");
                }
            }
            try
            {
                object obj = jp.Value.ToObject(valueType);
                memberInfo.Setter(jsonNode, obj);
            }
            catch (Exception e)
            {
                throw e;
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
                int index_ = 0;
                object obj = window.GraphView.Asset.Data.Nodes.GetValueInternal<object>(ref pAPath, ref index_);
                PathExpansionResult result = FuzzyPathResolver.TryExpandPath(pAPath, obj, type);
                if (result.IsSuccess)
                {
                    Debug.Log($"FuzzyPathResolver: {pAPath} expanded to {result.ExpandedPath}");
                    pAPath = result.ExpandedPath;
                }
                port = window.GraphView.GetPort(pAPath);
                if (port == null)
                {
                    throw new ArgumentException($"{pAPath}:路径类型不是节点或者节点的集合");
                }
                if (!port.portType.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"无法将节点({type.Name})添加到该路径,请检查路径类型({port.portType})是否与要添加的节点类型兼容");
                }
            }

            JsonNode jsonNode = (JsonNode)Activator.CreateInstance(type);

            string jsonError = "";
            if (!string.IsNullOrEmpty(json))
            {
                JObject job = JObject.Parse(json);
                TypeReflectionInfo typeReflectionInfo = TypeCacheSystem.GetTypeInfo(type);
                List<Exception> exceptions = new();
                foreach (JProperty jp in job.Properties())
                {
                    try
                    {
                        SetNonNodeValue(jsonNode, typeReflectionInfo, jp, nodePath);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
                if (exceptions.Count > 0)
                {
                    jsonError = $"\n{string.Join("\n", exceptions.Select(e => e.Message))}";
                }
            }
            if (!window.GraphView.SetNodeByPath(jsonNode, pAPath))
            {
                throw new InvalidOperationException($"设置节点失败:目标路径({nodePath})无法添加({type.Name})");
            }
            window.GraphView.AddViewNodeWithConnection(jsonNode, pAPath);
            window.GraphView.FormatNodes();
            if (port is NumPort numPort)
            {
                numPort.DisplayPopupText();
            }
            return $"{SaveChanges(window)}{jsonError}";
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
            TreeNodeGraphWindow window = GetWindow(path);
            ValidatePath(window, nodePath);

            PAPath pAPath = nodePath;
            int index_ = 0;
            object obj = window.GraphView.Asset.Data.Nodes.GetValueInternal<object>(ref pAPath, ref index_) ?? throw new ArgumentException($"object not found at {path}");
            if (obj is not JsonNode existNode)
            {
                PathExpansionResult result = FuzzyPathResolver.TryExpandPath(pAPath, obj);
                existNode = null;
                if (result.IsSuccess)
                {
                    Debug.Log($"FuzzyPathResolver: {pAPath} expanded to {result.ExpandedPath}");
                    pAPath = result.ExpandedPath;
                    existNode = window.GraphView.Asset.Data.GetValue<JsonNode>(pAPath);
                }
            }
            if (existNode == null)
            {
                throw new ArgumentException($"node not found at {path}");
            }
            string error = "";
            List<string> successlist = new(); ;
            if (!string.IsNullOrEmpty(json))
            {
                JObject job = null;
                try
                {
                    job = JObject.Parse(json);
                }
                catch (Exception)
                {
                    throw new Newtonsoft.Json.JsonSerializationException(json);
                }
                TypeReflectionInfo typeReflectionInfo = TypeCacheSystem.GetTypeInfo(existNode.GetType());
                List<Exception> exceptions = new();
                foreach (JProperty jp in job.Properties())
                {
                    try
                    {
                        SetNonNodeValue(existNode, typeReflectionInfo, jp, nodePath);
                        successlist.Add(jp.Name);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
                if (exceptions.Count > 0)
                {
                    error = $"\n{string.Join("\n", exceptions.Select(e => e.Message))}";
                }
            }
            if (successlist.Count > 0)
            {
                return $"{SaveChanges(window, true)}:成功设置以下字段[{string.Join(',', successlist)}]{error}";
            }
            else
            {
                throw new ArgumentException($"没有成功修改任何字段,请检查json格式是否正确或者字段是否存在于节点({existNode.GetType().Name})中{error}");
            }
        }

        public static string RemoveNode(string path, string nodePath, bool recursive = true)
        {
            TreeNodeGraphWindow window = GetWindow(path);
            PAPath pAPath = nodePath;
            int index_ = 0;
            object obj = window.GraphView.Asset.Data.Nodes.GetValueInternal<object>(ref pAPath, ref index_);
            if (obj is not JsonNode existNode)
            {
                PathExpansionResult result = FuzzyPathResolver.TryExpandPath(pAPath, obj);
                if (result.IsSuccess)
                {
                    Debug.Log($"FuzzyPathResolver: {pAPath} expanded to {result.ExpandedPath}");
                    pAPath = result.ExpandedPath;
                }
                existNode = window.GraphView.Asset.Data.GetValue<JsonNode>(pAPath);
            }
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
            string error = null;
            if (path.StartsWith("Assets/")) { path = path[7..]; }
            if (string.IsNullOrEmpty(path))
            {
                error = "File path is null or empty";
                Debug.LogError(error);
                throw new ArgumentException(error);
            }
            if (!path.EndsWith(".ja") && !path.EndsWith(".tpl"))
            {
                int index = path.LastIndexOf('.');
                if (index > 0)
                {
                    path = path[..index];
                }
                //模糊获取文件
                bool jaExist = File.Exists($"Assets/{path}.ja");
                bool tplExist = File.Exists($"Assets/{path}.tpl");
                if (jaExist && tplExist)
                {
                    error = $"文件({path})存在多个后缀(.ja和.tpl),请指定后缀";
                    Debug.LogError(error);
                    throw new ArgumentException(error);
                }
                else if (jaExist)
                {
                    path += ".ja";
                }
                else if (tplExist)
                {
                    path += ".tpl";
                }
                else
                {
                    error = $"不支持后缀不为.ja或者.tpl的文件";
                    Debug.LogError(error);
                    throw new NotSupportedException(error);
                }
            }
        }
    }
}
