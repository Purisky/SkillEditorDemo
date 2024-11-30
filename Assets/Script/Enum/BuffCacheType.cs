using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    public enum BuffCacheType
    {
        #region Data
        [LabelInfo("伤害/总伤害")]
        TotalDmg,
        [LabelInfo("伤害/被防御抵消")]
        DefDmg,
        [LabelInfo("伤害/被护盾抵消")]
        SPDmg,
        [LabelInfo("伤害/剩余")]
        LeftDmg,
        [LabelInfo("伤害/真实伤害")]
        RealDmg,
        [LabelInfo("伤害/溢出伤害")]
        OverDmg,
        [LabelInfo("治疗/总治疗")]
        TotalHeal,
        [LabelInfo("治疗/溢出治疗")]
        OverHeal,
        [LabelInfo("治疗/真实治疗")]
        RealHeal,
        [LabelInfo("护盾/增加")]
        SPAdd,
        [LabelInfo("护盾/溢出")]
        OverSP,
        #endregion
    }
}
