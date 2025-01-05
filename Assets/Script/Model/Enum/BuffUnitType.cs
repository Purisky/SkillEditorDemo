using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
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
