using System;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
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
        [LabelInfo("Buff启动"), NoPassive, NoSeq]
        Start,
        [LabelInfo("Buff移除"), NoPassive, NoSeq]
        Remove,
        [LabelInfo("时间到期"), NoPassive, NoSeq]
        TimeOut,
        [LabelInfo("移除或到期"), NoPassive, NoSeq]
        RemoveOrTimeOut,
        [LabelInfo("每秒触发"), NoPassive, NoSeq]
        ByTime_1,
        [LabelInfo("每0.5秒触发"), NoPassive, NoSeq]
        ByTime_2,
        [LabelInfo("每0.33秒触发"), NoPassive, NoSeq]
        ByTime_3,
        [LabelInfo("每0.25秒触发"), NoPassive, NoSeq]
        ByTime_4,
        [LabelInfo("周期性触发"), NoPassive, NoSeq]
        ByTime_5,



        [LabelInfo("碰撞命中"), NoSeq]
        Hit,
        [LabelInfo("击杀单位")]
        Kill,
        [LabelInfo("治疗单位")]
        Heal,
        [LabelInfo("过量治疗")]
        OverHeal,
        [LabelInfo("造成暴击")]
        Crit,
        [LabelInfo("闪避攻击")]
        Dodge,
        [LabelInfo("护盾增加")]
        SPAdd,
        [LabelInfo("护盾破裂"), NoSeq]
        SPBreak,
        [LabelInfo("命中造成伤害"), NoSeq]
        HitDmg,
        [LabelInfo("造成伤害")]
        Dmg,
        [LabelInfo("造成护盾伤害")]
        SPDmg,
        [LabelInfo("造成生命伤害")]
        HPDmg,
        [LabelInfo("消耗魔力")]
        ManaCost,
        [LabelInfo("获得魔力")]
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
