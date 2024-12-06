using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#E8AC29")]
    public abstract class ActionNode : JsonNode, IGrowID<ActionNode>
    {
        [JsonIgnore]
        public int GrowID { get; set; }
        public static T Get<T>(int id) where T : ActionNode => (T)IGrowID < ActionNode >.Get(id);
        public abstract bool Handle(int trigCount, TrigInfo info, CombatCache cache);
    }
    [NodeInfo(typeof(ActionNode), "分支执行", 100, "执行/分支执行")]
    public class ConditionAction : ActionNode
    {
        [Child(true), TitlePort]
        public Condition Condition;
        [Child(true), LabelInfo(Text = "真", Width = 10)]
        public ActionNode True;
        [Child(true), LabelInfo(Text = "假", Width = 10)]
        public ActionNode False;

        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
    }



    [NodeInfo(typeof(ActionNode), "实体属性修改", 200, "执行/实体属性修改")]
    public class StatModify : ActionNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;

        [ShowInNode, LabelInfo(Hide = true), Group("Stat")]
        public StatType StatType;
        [ShowInNode(ShowIf = nameof(isStat)), LabelInfo(Hide = true), Group("Stat")]
        public StatModType StatModType;
        [ShowInNode(ShowIf = nameof(isRes)), LabelInfo(Hide = true), Group("Stat")]
        public ValueModType ResModType;
        [Child, LabelInfo(Hide = true)]
        public FuncValue Value;

        [JsonIgnore]
        bool isStat => StatType != StatType.None && StatType.IsStat();
        [JsonIgnore]
        bool isRes => StatType != StatType.None && !StatType.IsStat();


        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
        public float GetRuntimeValue(TrigInfo info, CombatCache cache)
        {
            return Value.Value;
        }
    }
    [NodeInfo(typeof(ActionNode), "伤害", 200, "执行/伤害")]
    public class Damage : ActionNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true)]
        public DmgType DmgType;
        [ShowInNode, LabelInfo("直接"), Group("Type")]
        public bool Direct;
        [ShowInNode, LabelInfo("闪避"), Group("Type")]
        public bool Dodge;
        [ShowInNode, LabelInfo("暴击"), Group("Type")]
        public bool Crit;
        [Child, LabelInfo(Hide = true)]
        public FuncValue Value;


        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
    }
    [NodeInfo(typeof(ActionNode), "存储临时数据", 160, "执行/存储临时数据"), AssetFilter(true, typeof(BuffAsset))]
    public class TempData : ActionNode
    {
        [ShowInNode, LabelInfo(Hide = true), Group("Value", Width = 50)]
        public ValueModType ResModType;
        [Child, LabelInfo(Hide = true), Group("Value")]
        public FuncValue Value;
        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
    }
    [NodeInfo(typeof(ActionNode), "添加Buff", 160, "执行/添加Buff")]
    public class AddBuff : ActionNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        public string ID;
        [ShowInNode, LabelInfo("等级", 30)]
        public FuncValue Level;
        [ShowInNode, LabelInfo("层数", 30)]
        public FuncValue Degree;
        [ShowInNode, LabelInfo("参数0", 30)]
        public FuncValue Param0;
        //[ShowInNode, LabelInfo("参数1", 30)]
        //public NumValue Param1;
        //[ShowInNode, LabelInfo("参数2", 30)]
        //public NumValue Param2;
        //[ShowInNode, LabelInfo("参数3", 30)]
        //public NumValue Param3;


        static DropdownList<string> Buffs => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns;



        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }

    }
    [NodeInfo(typeof(ActionNode), "添加随机Buff", 180, "执行/添加随机Buff")]
    public class AddRandomBuffs : ActionNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        public List<string> IDs;
        [ShowInNode, LabelInfo("可重复", 40), Group("Count")]
        public bool Repeat;
        [ShowInNode, LabelInfo("数量", 30), Group("Count")]
        public FuncValue Count;
        [ShowInNode, LabelInfo("等级", 30)]
        public FuncValue Level;
        [ShowInNode, LabelInfo("层数", 30)]
        public FuncValue Degree;
        [ShowInNode, LabelInfo("参数0", 30)]
        public FuncValue Param0;
        //[ShowInNode, LabelInfo("参数1", 30)]
        //public NumValue Param1;
        //[ShowInNode, LabelInfo("参数2", 30)]
        //public NumValue Param2;
        //[ShowInNode, LabelInfo("参数3", 30)]
        //public NumValue Param3;
        static DropdownList<string> Buffs => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns;

        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
    }

    [NodeInfo(typeof(ActionNode), "终止触发", 180, "执行/终止触发"), AssetFilter(true, typeof(BuffAsset))]
    public class StopTrig : ActionNode
    {
        [Child, TitlePort]
        public Condition Condition;

        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return false;
        }
    }
    [NodeInfo(typeof(ActionNode), "尝试移除Buff", 180, "执行/尝试移除Buff"), AssetFilter(true, typeof(BuffAsset))]
    public class TryRemoveBuff : ActionNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        public string ID;
        [Child, LabelInfo(Text = "成功", Width = 15)]
        public ActionNode True;
        [Child, LabelInfo(Text = "失败", Width = 15)]
        public ActionNode False;
        static DropdownList<string> Buffs => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns;


        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
    }
    [NodeInfo(typeof(ActionNode), "修改战斗缓存", 180, "执行/修改战斗缓存"), AssetFilter(true, typeof(BuffAsset))]
    public class CombatCacheModify : ActionNode
    {
        [ShowInNode, LabelInfo(Hide = true)]
        public CombatCacheType CacheType;
        [ShowInNode, LabelInfo(Hide = true), Group("Value",Width = 50)]
        public ValueModType ModType;
        [Child, LabelInfo(Hide = true), Group("Value")]
        public FuncValue Value;


        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
    }


}
