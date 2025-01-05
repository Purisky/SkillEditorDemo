using System;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    public struct STrigType
    {
        public const TrigType On = 0;
        public const TrigType Aft = (TrigType)1024;
        public const TrigType ed = (TrigType)2048;
    }
    public enum TriggerSequence
    {
        [LabelInfo("时")]
        On,
        [LabelInfo("后")]
        Aft,
    }
    public enum TrigType
    {
        [LabelInfo("开始"), NoPassive, NoSeq]
        Start,
        [LabelInfo("被移除"), NoPassive, NoSeq]
        Remove,
        [LabelInfo("时间耗尽"), NoPassive, NoSeq]
        TimeOut,
        [LabelInfo("被移除或时间耗尽"), NoPassive, NoSeq]
        RemoveOrTimeOut,
        [LabelInfo("周期/1秒"), NoPassive, NoSeq]
        ByTime_1,
        [LabelInfo("周期/0.5秒"), NoPassive, NoSeq]
        ByTime_2,
        [LabelInfo("周期/0.333秒"), NoPassive, NoSeq]
        ByTime_3,
        [LabelInfo("周期/0.25秒"), NoPassive, NoSeq]
        ByTime_4,
        [LabelInfo("周期"), NoPassive, NoSeq]
        ByTime_5,



        [LabelInfo("盒子命中"), NoSeq]
        Hit,
        [LabelInfo("击杀")]
        Kill,
        [LabelInfo("治疗")]
        Heal,
        [LabelInfo("过量治疗")]
        OverHeal,
        [LabelInfo("暴击")]
        Crit,
        [LabelInfo("闪避")]
        Dodge,
        [LabelInfo("护盾增加")]
        SPAdd,
        [LabelInfo("护盾破裂"), NoSeq]
        SPBreak,
        [LabelInfo("伤害/命中伤害"), NoSeq]
        HitDmg,
        [LabelInfo("伤害/造成任意")]
        Dmg,
        [LabelInfo("伤害/对护盾伤害")]
        SPDmg,
        [LabelInfo("伤害/对生命伤害")]
        HPDmg,
        [LabelInfo("魔力消耗")]
        ManaCost,
        [LabelInfo("魔力增加(非自然回复)")]
        ManaAdd,




    }


    public static partial class Extensions
    {
        public static TrigType ed(this TrigType trig) => STrigType.ed | trig;
        public static TrigType Remove_ed(this TrigType trig) => trig & (~STrigType.ed);
        public static bool ised(this TrigType trig) => (trig & STrigType.ed) == STrigType.ed;
        public static TrigType Inverse_ed(this TrigType trig) => trig.ised() ? trig.Remove_ed() : trig.ed();
    }

    /// <summary>
    /// 无 被动触发场景
    /// </summary>
    public class NoPassiveAttribute : Attribute { }
    /// <summary>
    /// 无 后触发场景
    /// </summary>
    public class NoSeqAttribute : Attribute { }

}
