using MCP4Unity;
using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TreeNode;
using TreeNode.Editor;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine.UIElements;

namespace SkillEditorDemo
{
    public class NodeTools
    {
        [Tool("为一个Asset文件添加Node")]
        public static string AddNode([Desc("文件路径")] string path, [Desc("添加的节点路径")] string portPath, [Desc("Node类型")] string typeName, [Desc("Node数据json,以合并方式并入新对象")] string json)
        {
            return NodeTools.AddNode(path, portPath, typeName, json);
        }
        [Tool("获取可用的Node信息")]
        public static List<string> ListNodes([Desc("null时获取所有Node,否则获取继承自baseType的Node")] string baseType)
        {
            return ToolUtil.GetNodesByName(baseType).Select(n => n.HeadInfo()).ToList();
        }
        [Tool("获取Node的结构与用法")]
        public static string GetNodePrompt(string typeName)
        {
            if (ToolUtil.Prompts.TryGetValue(typeName, out var prompt) && prompt is NodePrompt nodePrompt)
            {
                return nodePrompt.ListDetail();
            }
            else
            {
                return $"Node {typeName} not found";
            }
        }
    }
}
