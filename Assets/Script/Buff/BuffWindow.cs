using TreeNode.Editor;
using TreeNode.Runtime;
using UnityEditor;

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
