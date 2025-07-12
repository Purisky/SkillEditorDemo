using System;
using System.Collections.Generic;
using System.Linq;
using TreeNode;
using TreeNode.Editor;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkillEditorDemo
{
    public static class ToolUtil
    {
        public static string AddNode(string path, string portPath, string typeName, string json)
        {
            Type type = GetValidType(typeName);
            if (type is null) { return "type not valid"; }
            return AddNode(path, portPath, type, json);
        }
        public static Dictionary<string, Type> ValidNodeTypes;
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
        public static string AddNode(string filePath, string portPath, Type type, string json)
        {
            TreeNodeGraphWindow window = JsonAssetHandler.OpenJsonAsset($"Assets/{filePath}");
            if (window == null) { return "file not exist"; }
            PropertyElement propertyElement = window.GraphView.Find(portPath);
            if (propertyElement == null) { return "path not valid"; }
            ChildPort port = propertyElement.Q<ChildPort>();
            if (port == null) { return "field is not JsonNode or collection of JsonNode"; }
            if (!port.portType.IsAssignableFrom(type)) { return "type not match"; }
            JsonNode jsonNode = null;
            if (string.IsNullOrEmpty(json))
            {
                jsonNode = (JsonNode)Activator.CreateInstance(type);
            }
            else
            {
                jsonNode = (JsonNode)Json.Get(type, json);
            }
            if (!window.GraphView.SetNodeByPath(jsonNode, portPath))
            {
                return "set value error";
            }
            window.GraphView.AddViewNode(jsonNode, port);
            window.GraphView.FormatNodes();
            window.History.AddStep();
            window.SaveChanges();
            return "Success";
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

            //foreach (var b in prompts.Values)
            //{
            //    Debug.Log(b);
            //}
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
            return Prompts.Values.OfType<NodePrompt>().Where(n => n.Type.Inherited(type)).ToList();
        }

    }
}
