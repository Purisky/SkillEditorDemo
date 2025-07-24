using MCP4Unity;
using SkillEditorDemo.Model;
using System.Collections.Generic;
using System.Linq;
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
            if (fileName.EndsWith(".ja")) {
                fileName = fileName[..^3];
            }
            bool success = TreeNodeGraphWindow.CreateFile<BuffAsset>($"{Application.dataPath}/{BuffPath}", fileName);
            if (success)
            {
                JsonAssetHandler.OpenJsonAsset($"{Application.dataPath}/{BuffPath}/{fileName}.ja");
            }
            return success;
        }


    }
}
