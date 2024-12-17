using System;
using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    public enum CombatCacheType
    {
        [LabelInfo("伤害/总伤害")]
        TotalDmg,
        [LabelInfo("伤害/被防御抵消")]
        DefDmg,
        [LabelInfo("伤害/被护盾抵消")]
        SPDmg,
        [LabelInfo("伤害/剩余")]
        LeftDmg,
        [LabelInfo("伤害/生命伤害")]
        LifeDmg,
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
        [LabelInfo("魔法/增加")]
        ManaAdd,
        [LabelInfo("魔法/消耗")]
        ManaCost,
        [LabelInfo("魔法/溢出")]
        OverMana,
        [Obsolete]
        Count,
    }
}
