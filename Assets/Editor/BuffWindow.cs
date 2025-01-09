using SkillEditorDemo.Model;
using TreeNode.Editor;
using TreeNode.Utility;
using UnityEditor;
#if UNITY_EDITOR
namespace SkillEditorDemo.Editor
{
    [NodeAsset(typeof(BuffAsset))]
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