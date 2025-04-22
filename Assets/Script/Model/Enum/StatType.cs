using System;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    using Dft = DefaultValueAttribute;
    using DT = DmgType;
    using Parent = StatParentAttribute;
    using SC = StatClass;
    using ST = StatType;


    public enum StatModType
    {
        [LabelInfo("基础叠加")]
        BaseAdd,
        [LabelInfo("叠乘")]
        Multiply,
        [LabelInfo("最终叠加")]
        FinalAdd
    }
    public enum ValueModType
    {
        [LabelInfo("=")]
        Set,
        [LabelInfo("+")]
        Add,
        [LabelInfo("*")]
        Multiply
    }

    public partial class Stat
    {
        public const int Offset = 8;
        public const int Type = 1 << 24;
    }
    public enum StatClass
    {
        None,
        HP,
        Mana,
        SP,//ShieldPoint

        ResCount,


        DmgMod,
        DmgedMod,
        Atk,
        Speed,
        Def,
        Crit,
        Dodge,
        Range,
        Count,
        Time,
        Size,


        Stat = 1 << 24,
        Math = 1 << 25,
    }




    public enum StatType
    {
        [HideEnum]
        None = 0,
        #region 血量
        [LabelInfo("血量/当前值")]
        HP = SC.HP << Stat.Offset,
        [LabelInfo("血量/最大值")]
        MaxHP = SC.Stat | HP,
        [LabelInfo("血量/每秒回复量")]
        HPRecover = MaxHP | 1,
        #endregion
        #region 护盾
        [LabelInfo("护盾/当前值")]
        SP = SC.SP << Stat.Offset,
        [LabelInfo("护盾/最大值")]
        SPMax = SC.Stat | SP,
        [LabelInfo("护盾/每秒回复量")]
        SPRecover = SPMax | 1,
        [LabelInfo("护盾/回复延迟")]
        SPRecoverDelay = SPMax | 2,
        #endregion
        #region 魔力
        [LabelInfo("魔力/当前值")]
        Mana = SC.Mana << Stat.Offset,
        [LabelInfo("魔力/最大值"), Dft(100)]
        ManaMax = SC.Stat | Mana,
        [LabelInfo("魔力/每秒回复量")]
        ManaRecover = ManaMax | 1,
        #endregion

        #region 伤害
        [LabelInfo("伤害/总加成"), Dft(1)]
        DmgMod = SC.Stat | (SC.DmgMod << Stat.Offset),
        [LabelInfo("伤害/物理加成"), Dft(1), Parent(DmgMod)]
        DmgMod_Physic = DmgMod | DT.Physic,
        [LabelInfo("伤害/火焰加成"), Dft(1), Parent(DmgMod)]
        DmgMod_Fire = DmgMod | DT.Fire,
        [LabelInfo("伤害/冰霜加成"), Dft(1), Parent(DmgMod)]
        DmgMod_Frost = DmgMod | DT.Frost,
        [LabelInfo("伤害/雷电加成"), Dft(1), Parent(DmgMod)]
        DmgMod_Lightning = DmgMod | DT.Lightning,
        [LabelInfo("承伤/总加成"), Dft(1)]
        DmgedMod = SC.Stat | (SC.DmgedMod << Stat.Offset),
        [LabelInfo("承伤/物理加成"), Dft(1), Parent(DmgedMod)]
        DmgedMod_Physic = DmgedMod | DT.Physic,
        [LabelInfo("承伤/火焰加成"), Dft(1), Parent(DmgedMod)]
        DmgedMod_Fire = DmgedMod | DT.Fire,
        [LabelInfo("承伤/冰霜加成"), Dft(1), Parent(DmgedMod)]
        DmgedMod_Frost = DmgedMod | DT.Frost,
        [LabelInfo("承伤/雷电加成"), Dft(1), Parent(DmgedMod)]
        DmgedMod_Lightning = DmgedMod | DT.Lightning,
        #endregion 
        #region 速度
        [LabelInfo("速度/总加成"), Dft(1)]
        Speed = SC.Stat | (SC.Speed << Stat.Offset),
        [LabelInfo("速度/移动"), Dft(1), Parent(Speed)]
        MoveSpeed = Speed | 1,
        [LabelInfo("速度/状态机"), Dft(1), Parent(Speed)]
        StateSpeed = Speed | 2,
        [LabelInfo("速度/攻击"), Dft(1), Parent(Speed)]
        AtkSpeed = Speed | 3,
        [LabelInfo("速度/施法"), Dft(1), Parent(Speed)]
        CastSpeed = Speed | 4,
        #endregion
        #region 攻击
        [LabelInfo("攻击/基础值"), Dft(10)]
        Atk = SC.Stat | (SC.Atk << Stat.Offset),
        #endregion
        #region 防御
        [LabelInfo("防御/基础值")]
        Def = SC.Stat | (SC.Def << Stat.Offset),
        [LabelInfo("防御/防御穿透")]
        DefIgnore = Def | 1,
        #endregion
        #region 暴击
        [LabelInfo("暴击/基础值")]
        Crit = SC.Stat | (SC.Crit << Stat.Offset),
        [LabelInfo("暴击/伤害系数"), Dft(2)]
        CritDmgMod = Crit | 1,
        [LabelInfo("暴击/抗性")]
        CritResist = Crit | 2,
        [LabelInfo("暴击/暴伤抗性")]
        CritDmgModResist = Crit | 3,
        #endregion
        #region 闪避
        [LabelInfo("闪避/基础值")]
        Dodge = SC.Stat | (SC.Dodge << Stat.Offset),
        [LabelInfo("闪避/精准"), Dft(1)]
        Accurate = Dodge | 1,
        [LabelInfo("闪避/忽视闪避")]
        DodgeIgnore = Dodge | 2,
        #endregion
        #region 范围
        [LabelInfo("范围/总加成"), Dft(1)]
        Range = SC.Stat | (SC.Range << Stat.Offset),
        #endregion
        #region 数量
        [HideEnum]
        Count = SC.Stat | (SC.Count << Stat.Offset),
        [LabelInfo("数量/额外投射物")]
        ProjectileCount = Count | 1,
        #endregion
        #region 时间
        [HideEnum]
        Time = SC.Stat | (SC.Time << Stat.Offset),
        [LabelInfo("时间/增益时间"), Dft(1)]
        BuffTime = Time | 1,
        [LabelInfo("时间/减益时间"), Dft(1)]
        DebuffTime = Time | 2,
        #endregion
        #region 大小
        [HideEnum]
        Size = SC.Stat | (SC.Size << Stat.Offset),

        #endregion


    }

    /// <summary>
    /// 伤害类型
    /// </summary>
    public enum DmgType
    {
        ///<summary>任意</summary>
        [LabelInfo("任意")]
        Any,
        ///<summary>物理</summary>
        [LabelInfo("物理")]
        Physic,
        ///<summary>火焰</summary>
        [LabelInfo("火焰")]
        Fire,
        ///<summary>冰霜</summary>
        [LabelInfo("冰霜")]
        Frost,
        ///<summary>雷电</summary>
        [LabelInfo("雷电")]
        Lightning,
    }
    /// <summary>
    /// 暴击类型
    /// </summary>
    public enum CritType
    {
        ///<summary>任意</summary>
        [LabelInfo("任意")]
        Any,
        ///<summary>物理</summary>
        [LabelInfo("非暴击")]
        NonCrit,
        ///<summary>火焰</summary>
        [LabelInfo("暴击")]
        Crit,
    }
    /// <summary>
    /// 伤害直接类型
    /// </summary>
    public enum DmgDirectType
    {
        ///<summary>任意</summary>
        [LabelInfo("任意")]
        Any,
        ///<summary>物理</summary>
        [LabelInfo("非直接")]
        NonDirect,
        ///<summary>火焰</summary>
        [LabelInfo("直接")]
        Direct,
    }
    /// <summary>
    /// 属性默认值：用于取不到数据时,不添加代表0
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultValueAttribute : Attribute
    {
        public float Value;
        public DefaultValueAttribute(float value)
        {
            Value = value;
        }
    }
    /// <summary>
    /// 父属性：取值时，乘父属性的值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StatParentAttribute : Attribute
    {
        public ST Type;
        public bool Mult;
        public StatParentAttribute(ST type, bool mult = true)
        {
            Type = type;
            Mult = mult;
        }
    }
    public static partial class Extensions
    {
        const ST ST = (ST)(int)SC.Stat;
        public static bool IsStat(this ST stat) => stat.HasFlag(ST);
        public static SC GetStatClass(this ST st) => (SC)(((int)st & (~(int)ST)) >> Stat.Offset);
        public static ST MaxResStatType(this ST st) => (ST)((int)ST | (int)st);
        /// <summary>
        /// 属性大类的当前资源属性类型
        /// </summary>
        public static ST CurResStatType(this SC sc) => (ST)((int)sc << Stat.Offset);
        public static ST MaxResStatType(this SC sc) => (ST)((int)ST | (int)sc.CurResStatType());

        public static ST StatType(this DT dt, bool ed) => (ST)((int)(ed ? ST.DmgedMod : ST.DmgMod) | (int)dt);


    }
}
