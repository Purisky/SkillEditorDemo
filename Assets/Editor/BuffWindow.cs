using SkillEditorDemo.Model;
using System;
using TreeNode.Editor;
using TreeNode.Utility;
using UnityEditor;
#if UNITY_EDITOR
namespace SkillEditorDemo.Editor
{
    [NodeAsset(typeof(BuffAsset)),Serializable]
    public class BuffWindow: TreeNodeGraphWindow
    {
        [MenuItem("Assets/Create/TreeNode/Buff")]
        public static void CreateFile()
        {
            CreateFile<BuffAsset>();
        }
    }
}
#endif