using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public enum BuffUnitType
    {
        [LabelInfo("Buff携带者")]
        Carrier,
        [LabelInfo("创建者")]
        Creator,
        [LabelInfo("源")]
        Source,
        [LabelInfo("触发目标")]
        TriggerTarget,
    }
}
