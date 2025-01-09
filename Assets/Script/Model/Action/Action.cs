using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
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
        [Child, LabelInfo(Text = "真", Width = 10)]
        public ActionNode True;
        [Child, LabelInfo(Text = "假", Width = 10)]
        public ActionNode False;

        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            bool condition = Condition.GetResult(info, cache);
            if (condition)
            {
                return True?.Handle(trigCount, info, cache) ?? true;
            }
            else
            {
                return False?.Handle(trigCount, info, cache) ?? true;
            }
        }
    }



    [NodeInfo(typeof(ActionNode), "实体属性修改", 200, "执行/实体属性修改")]
    public class StatModify : ActionNode
    {
        [Child(true), TitlePort]
        public List<UnitNode> UnitNodes;

        [ShowInNode, LabelInfo(Hide = true), Group("Stat"),Dropdown(nameof(GetStatTypes))]
        public StatType StatType;
        [ShowInNode(ShowIf = nameof(ShowStat)), LabelInfo(Hide = true), Group("Stat")]
        public StatModType StatModType;
        [ShowInNode(ShowIf = nameof(ShowRes)), LabelInfo(Hide = true), Group("Stat")]
        public ValueModType ResModType;
        [Child, LabelInfo(Hide = true)]
        public FuncValue Value;

        [JsonIgnore]
        bool ShowStat => StatType != StatType.None && StatType.IsStat();
        [JsonIgnore]
        bool ShowRes => StatType != StatType.None && !StatType.IsStat();


        static List<StatType> GetStatTypes(Type type)
        {
            //Debug.Log(type);
            if (type == typeof(BuffAsset))
            {
                return statTypes;
            }
            return valueTypes;
        }
        static readonly List<StatType> statTypes = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToList();
        static readonly List<StatType> valueTypes = statTypes.Where(s => !s.IsStat()).ToList();
        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            if (ShowStat && info.TrigID == -1) { return true; }
            List<Unit> units = UnitNodes.SelectMany(n=> n.GetUnits(info, cache)).ToList();
            for (int i = 0; i < units.Count; i++)
            {
                if (ShowStat)
                {
                    units[i].StatHandler.Add(StatType,new ActionCache(info.TrigID,GrowID));
                }
                else
                { 
                    float value = Value.GetResult(info, cache);
                    units[i].StatHandler.SetValue(StatType, value, ResModType);
                }
            }
            return true;
        }
        public float GetRuntimeValue(TrigInfo info, CombatCache cache)
        {
            return Value.GetResult(info, cache);
        }
    }
    [NodeInfo(typeof(ActionNode), "伤害", 200, "执行/伤害")]
    public class Damage : ActionNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true),Dropdown(nameof(DmgTypes))]
        public DmgType DmgType;
        [ShowInNode, LabelInfo("直接"), Group("Type")]
        public bool Direct;
        [ShowInNode, LabelInfo("闪避"), Group("Type")]
        public bool Dodge_able;
        [ShowInNode, LabelInfo("暴击"), Group("Type")]
        public bool Crit_able;
        [Child, LabelInfo(Hide = true)]
        public FuncValue Value;


        static readonly List<DmgType> DmgTypes = new()
        {
            DmgType.Physic,
            DmgType.Fire,
            DmgType.Frost,
            DmgType.Lightning,
        };



        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            Unit from = Unit.Get(info.SourceID);
            cache.DmgType = DmgType;
            cache.DirectDmg = Direct;
            cache.Dodge_able = Dodge_able;
            cache.Crit_able = Crit_able;
            float dmgMod = from?.StatHandler.GetDmgMod(DmgType) ?? 1;
            List<Unit> units =  UnitNode.GetUnits(info, cache);
            for (int i = 0; i < units.Count; i++)
            {
                info.CurrentID = units[i].Index;
                CombatCache clone = cache.Clone();
                float dmgValue = Value.GetResult(info, clone);
                clone[CombatCacheType.TotalDmg] = dmgValue * dmgMod;
                TryMakeDmg(trigCount, units[i], clone, from);
            }


            return true;
        }

        public static void TryMakeDmg(int trigCount, Unit unit, CombatCache cache, Unit from)
        {
            if (!unit.HitCheck(trigCount, from, cache)) { return; }
            if (!unit.Trig(trigCount, TrigType.Hit.ed(), cache, from)) { return; }
            if (!unit.CritCheck(trigCount, from, cache)) { return; };
            unit.TakeDmg(trigCount, cache, from);
        }
    }
    [NodeInfo(typeof(ActionNode), "存储Buff数据", 160, "执行/存储Buff数据"), AssetFilter(true, typeof(BuffAsset))]
    public class BuffRuntimeData : ActionNode
    {
        [ShowInNode, LabelInfo(Hide = true), Group("Value", Width = 50)]
        public ValueModType ValueModType;
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

    [NodeInfo(typeof(ActionNode), "终止触发事件", 180, "执行/终止触发事件"), AssetFilter(true, typeof(BuffAsset))]
    public class StopTrigEvent : ActionNode
    {
        [Child, TitlePort]
        public Condition Condition;

        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return !Condition.GetResult(info, cache);
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
            float value = Value.GetResult(info, cache);
            switch (ModType)
            {
                case ValueModType.Set:
                    cache[CacheType] = value;
                    break;
                case ValueModType.Add:
                    cache[CacheType] += value;
                    break;
                case ValueModType.Multiply:
                    cache[CacheType] *= value;
                    break;
            }
            return true;
        }
    }
    [NodeInfo(typeof(ActionNode), "创建对象", 180, "执行/创建对象"), AssetFilter(true, typeof(BuffAsset))]
    public class CreateObj : ActionNode
    {
        [Child(true), TitlePort]
        public ObjNode ObjNode;



        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            throw new NotImplementedException();
        }
    }




}
