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
        public static string AddNode([Desc("文件路径")] string path, [Desc("添加的节点路径")] string portPath, [Desc("Node类型")] string typeName, [Desc("以合并方式并入新对象,无需重复添加$type,数据禁止嵌套其他Node,所有Node数据必须通过AddNode工具添加")] string json)
        {
            return ToolUtil.AddNode(path, portPath, typeName, json);
        }
        [Tool("修改Node数据")]
        public static string ModifyNode([Desc("文件路径")] string path, [Desc("修改的节点路径")] string portPath, [Desc("以覆盖合并方式修改对象,修改时禁止嵌套其他Node,所有Node数据必须通过AddNode工具添加")] string json)
        {
            return ToolUtil.ModifyNode(path, portPath, json);
        }
        [Tool("删除一个Node")]
        public static string RemoveNode([Desc("文件路径")] string path, [Desc("删除的节点路径")] string portPath)
        {
            return ToolUtil.RemoveNode(path, portPath, true);
        }







        [Tool("获取可用的Node信息")]
        public static string ListNodes([Desc("null时获取所有Node,否则获取继承自baseType的Node")] string baseType)
        {
            return string.Join("\n", ToolUtil.GetNodesByName(baseType).Select(n => n.HeadInfo()));
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


        [Tool("在所有节点构造结束后用于检查当前资源中是否有不应存在的空值或者其他数值越界行为,不应频繁调用,返回Success表示没有问题")]
        public static string ValidateAsset([Desc("文件路径")] string path)
        {
            return ToolUtil.ValidateAsset(path);
        }


    }
}
