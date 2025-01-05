using TreeNode.Editor;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEditor;
#if UNITY_EDITOR
namespace SkillEditorDemo
{
    [NodeAsset(typeof(SkillAsset))]
    public class SkillWindow : TreeNodeGraphWindow
    {
        [MenuItem("Assets/Create/TreeNode/Skill")]
        public static void CreateFile()
        {
            CreateFile<SkillAsset>();
        }
    }
}
#endif