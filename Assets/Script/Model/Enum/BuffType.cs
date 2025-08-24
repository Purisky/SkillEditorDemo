using TreeNode.Utility;
namespace SkillEditorDemo.Model
{
    public enum CompeteType
    {
        [LabelInfo("无事发生"),Prompt("不处理,保留现有Buff")]
        None,
        [LabelInfo("独立"),Prompt("新Buff独立存在,与现有Buff共存,各自计算和触发")]
        Indie,
        [LabelInfo("刷新"),Prompt("刷新现有Buff的持续时间,不改变等级和层数")]
        Refresh,
        [LabelInfo("替换刷新"),Prompt("替换现有Buff,并刷新持续时间")]
        ReplaceRefresh,
        [LabelInfo("刷新至满层"),Prompt("将现有Buff层数刷新至最大层,并刷新持续时间")]
        Refresh2Max,
        [LabelInfo("叠层并刷新"),Prompt("现有Buff层数+1,不超过最大层,并刷新持续时间")]
        AddRefresh,
    }
}
