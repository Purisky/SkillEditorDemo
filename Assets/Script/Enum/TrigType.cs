using System;
using TreeNode.Runtime;
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
    }


    public static partial class Extensions
    {
        public static TrigType ed(this TrigType trig) => STrigType.ed | trig;
        public static TrigType Removed(this TrigType trig) => trig & (~STrigType.ed);
        public static bool ised(this TrigType trig) => (trig & STrigType.ed) == STrigType.ed;
        public static TrigType Inversed(this TrigType trig) => trig.ised() ? trig.Removed() : trig.ed();
    }

    /// <summary>
    /// 无 被动触发场景
    /// </summary>
    public class NoPassiveAttribute : Attribute
    {
        public NoPassiveAttribute()
        {
        }
    }
    /// <summary>
    /// 无 后触发场景
    /// </summary>
    public class NoSeqAttribute : Attribute
    {
        public NoSeqAttribute()
        {
        }
    }
}
