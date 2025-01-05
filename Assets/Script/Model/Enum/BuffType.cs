using TreeNode.Utility;
using UnityEngine;
namespace SkillEditorDemo
{
    public enum CompeteType
    {
        [LabelInfo("无事发生")]
        None,
        [LabelInfo("被动")]
        Eternal,
        [LabelInfo("独立")]
        Indie,
        [LabelInfo("刷新")]
        Refresh,
        [LabelInfo("替换刷新")]
        ReplaceRefresh,
        [LabelInfo("刷新至满层")]
        Refresh2Max,
        [LabelInfo("叠层并刷新")]
        AddRefresh,
    }
}
