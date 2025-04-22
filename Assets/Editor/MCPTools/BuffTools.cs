using MCP4Unity;
using SkillEditorDemo.Model;
using System.Collections.Generic;
using TreeNode.Editor;
using UnityEngine;

namespace SkillEditorDemo.MCP
{
    public class BuffTools
    {
        const string BuffPath = "Resources/Buff";
        [Tool("添加一个BuffAsset文件", "当返回false时说明fileName重复")]
        public static bool AddBuffAsset(string fileName)
        {
            return TreeNodeGraphWindow.CreateFile<BuffAsset>($"{Application.dataPath}/{BuffPath}", fileName);
        }

        [Tool("为一个Buff文件添加Node")]
        public static string AddBuffNode([Desc("文件路径")]string path, [Desc("添加的节点路径")] string portPath, [Desc("Node类型")] string typeName, [Desc("Node数据json")] string json)
        {
           return NodeTools.AddNode( path, portPath, typeName, json);
        }




        [Tool("获取可用的Node信息")]
        public static List<string> ListNodes([Desc("null时获取所有Node,否则获取继承自baseType的Node")] string baseType)
        {


            return new();                          



        }



        [Tool("获取Node的结构与用法")]
        public static string GetNodePrompt(string typeName)
        {




            return "";
        }
        
    }
}
