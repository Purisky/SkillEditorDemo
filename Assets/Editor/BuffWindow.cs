using TreeNode.Editor;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEditor;
#if UNITY_EDITOR
namespace SkillEditorDemo
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