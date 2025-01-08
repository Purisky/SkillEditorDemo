using SkillEditorDemo.Model;
using TreeNode.Editor;
using TreeNode.Utility;
using UnityEditor;
#if UNITY_EDITOR
namespace SkillEditorDemo.Editor
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