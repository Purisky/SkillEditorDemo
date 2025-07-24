using Leopotam.EcsLite;
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
    [Prompt(@"ActionNode是执行节点的基类,所有的执行节点都继承自ActionNode")]
    public abstract class ActionNode : JsonNode, IGrowID<ActionNode>
    {
        [JsonIgnore]
        public int GrowID { get; set; }
        public static T Get<T>(int id) where T : ActionNode => (T)IGrowID < ActionNode >.Get(id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigCount"></param>
        /// <param name="info"></param>
        /// <param name="cache"></param>
        /// <returns>exit trigger event if return false</returns>
        public abstract bool Handle(int trigCount, TrigInfo info, CombatCache cache);
    }
    [NodeInfo(typeof(ActionNode), "分支执行", 100, "执行/分支执行")]
    [Prompt(@"根据条件的真假执行不同的ActionNode")]
    public class ConditionAction : ActionNode
    {
        [Child(true), TitlePort]
        [Prompt(@"条件节点")]
        public Condition Condition;
        [Child, LabelInfo(Text = "真", Width = 10)]
        [Prompt(@"条件为真时执行的ActionNode")]
        public ActionNode True;
        [Child, LabelInfo(Text = "假", Width = 10)]
        [Prompt(@"条件为假时执行的ActionNode")]
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



    [NodeInfo(typeof(ActionNode), "单位属性修改", 200, "执行/单位属性修改")]
    [Prompt(@"可以修改单位的属性值")]
    public class StatModify : ActionNode
    {
        [Child(true), TitlePort]
        [Prompt(@"用于获取所修改的对象,可以修改复数对象")]
        public List<UnitNode> UnitNodes;
        [ShowInNode, LabelInfo(Hide = true), Group("Stat"),Dropdown(nameof(GetStatTypes))]
        [Prompt(@"将要修改的属性类型,修改分为两种模式:
1.修改常驻属性(Stat),常驻属性的修改需要定义持续时间,因此必须挂载在Buff上,对应的触发器节点(TrigNode)的触发类型必须是Start,如果Buff被移除,属性值会恢复,以下是其正确的结构
- BuffNode
  - TrigNode(TrigType:Start)
    - StatModify
2.修改资源(Value),资源的修改是即时生效的,不需要持续时间,没有结构限制,
修改资源可用于在不触发相关逻辑的情况下直接操纵单位的当前血量(HP)/当前魔力(Mana)等资源,如正常造成伤害等可直接使用伤害节点(Damage)")]
        public StatType StatType;
        [ShowInNode(ShowIf = nameof(ShowStat)), LabelInfo(Hide = true), Group("Stat")]
        [Prompt(@"属性修改的计算类型,仅在StatType为属性时生效")]
        public StatModType StatModType;
        [ShowInNode(ShowIf = nameof(ShowRes)), LabelInfo(Hide = true), Group("Stat")]
        [Prompt(@"资源修改的计算类型,仅在StatType为资源时生效")]
        public ValueModType ResModType;
        [Child, LabelInfo(Hide = true)]
        [Prompt(@"属性修改的具体值")]
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
        public override string GetInfo()
        {
            return $"{nameof(StatModify)}({StatType})";
        }
    }
    [NodeInfo(typeof(ActionNode), "伤害", 200, "执行/伤害")]
    [Prompt(@"用于对单位造成伤害,这个伤害可以进一步触发其他的TrigNode")]
    public class Damage : ActionNode
    {
        [Child(true), TitlePort]
        [Prompt(@"用于获取所造成伤害的对象")]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true),Dropdown(nameof(DmgTypes))]
        [Prompt(@"伤害的类型,在当前字段中禁止使用Any,伤害类型会影响伤害的计算,如物理伤害(Physic)会受到物理伤害减免及加成的影响")]
        public DmgType DmgType;
        [ShowInNode, LabelInfo("直接"), Group("Type")]
        [Prompt(@"是否为直接伤害,作为触发器的标签,一些基于伤害触发的效果会选择过滤非直接伤害以阻止触发进一步的效果")]
        public bool Direct;
        [ShowInNode, LabelInfo("闪避"), Group("Type")]
        [Prompt(@"是否可以被闪避,如果为true,则目标单位可以通过闪避机制来规避伤害")]
        public bool Dodge_able;
        [ShowInNode, LabelInfo("暴击"), Group("Type")]
        [Prompt(@"是否可以暴击,如果为true,则伤害可以触发暴击效果")]
        public bool Crit_able;
        [Child, LabelInfo(Hide = true)]
        [Prompt(@"伤害的具体值")]
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
            //Debug.Log("Damage Handle");
            Unit from = Unit.Get(info.SourceID);
            cache.DmgType = DmgType;
            cache.DirectDmg = Direct;
            cache.Dodge_able = Dodge_able;
            cache.Crit_able = Crit_able;
            float dmgMod = from?.StatHandler.GetDmgMod(DmgType) ?? 1;
            List<Unit> units =  UnitNode.GetUnits(info, cache);
            for (int i = 0; i < units.Count; i++)
            {
                info.CurrentID = units[i].Entity;
                CombatCache clone = cache.Clone();
                float dmgValue = Value.GetResult(info, clone);
                clone[CombatCacheType.TotalDmg] = dmgValue * dmgMod;
                TryMakeDmg(trigCount, units[i], clone, from);
            }


            return true;
        }

        public static void TryMakeDmg(int trigCount, Unit unit, CombatCache cache, Unit from)
        {
            //Debug.Log("TryMakeDmg");
            if (!unit.HitCheck(trigCount, from, cache)) { return; }
            if (!unit.Trig(trigCount, TrigType.Hit.ed(), cache, from)) { return; }
            if (!unit.CritCheck(trigCount, from, cache)) { return; };
            unit.TakeDmg(trigCount, cache, from);
        }
        public override string GetInfo()
        {
            return $"{nameof(Damage)}({DmgType})";
        }
    }
    [NodeInfo(typeof(ActionNode), "向Buff存储数据", 160, "执行/向Buff存储数据"), AssetFilter(true, typeof(BuffAsset))]
    [Prompt(@"用于存储Buff在运行时的数据方便进一步调用,这个数值可以跨越整个Buff的生命周期,以下是其用例:
    1.受到相应触发时,以累加等方式存储任意来源的数值
    2.受到相应触发时,为其他效果提供参数->使用BuffValueGetter节点中的RuntimeData调用存储的数值
注意!当前存值的读写不需要使用Key,一个Buff只存在一个存值数据")]
    public class BuffRuntimeData : ActionNode
    {
        [ShowInNode, LabelInfo(Hide = true), Group("Value", Width = 50)]
        [Prompt(@"数值修改的计算类型")]
        public ValueModType ValueModType;
        [Child, LabelInfo(Hide = true), Group("Value")]
        [Prompt(@"修改的具体值")]
        public FuncValue Value;
        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
    }
    [NodeInfo(typeof(ActionNode), "添加Buff", 160, "执行/添加Buff")]
    [Prompt(@"可以添加指定的Buff到指定的单位上,并且可以指定Buff的等级,层数,参数等信息")]
    public class AddBuff : ActionNode
    {
        [Child(true), TitlePort]
        [Prompt(@"用于获取所添加Buff的对象")]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        [Prompt(@"Buff的ID")]
        public string ID;
        [ShowInNode, LabelInfo("等级", 30)]
        [Prompt(@"Buff的等级")]
        public FuncValue Level;
        [ShowInNode, LabelInfo("层数", 30)]
        [Prompt(@"Buff的层数")]
        public FuncValue Degree;
        [ShowInNode, LabelInfo("参数0", 30)]
        [Prompt(@"Buff的参数0,Buff的创建参数可以用于在Buff之间传递参数")]
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

        public override string GetInfo()
        {
            return $"{nameof(AddBuff)}({ID})";
        }
    }
    [NodeInfo(typeof(ActionNode), "添加随机Buff", 180, "执行/添加随机Buff")]
    [Prompt(@"可以添加随机的复数Buff到指定的单位上,并且可以指定Buff的等级,层数,参数等信息")]
    public class AddRandomBuffs : ActionNode
    {
        [Child(true), TitlePort]
        [Prompt(@"用于获取所添加Buff的对象")]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        [Prompt(@"Buff的ID列表")]
        public List<string> IDs;
        [ShowInNode, LabelInfo("可重复", 40), Group("Count")]
        [Prompt(@"是否允许重复添加相同的Buff")]
        public bool Repeat;
        [ShowInNode, LabelInfo("数量", 30), Group("Count")]
        [Prompt(@"Buff的添加次数,每次添加时都会随机抽取,受Repeat影响排除重复Buff")]
        public FuncValue Count;
        [ShowInNode, LabelInfo("等级", 30)]
        [Prompt(@"Buff的等级")]
        public FuncValue Level;
        [ShowInNode, LabelInfo("层数", 30)]
        [Prompt(@"Buff的层数")]
        public FuncValue Degree;
        [ShowInNode, LabelInfo("参数0", 30)]
        [Prompt(@"Buff的参数0,Buff的创建参数可以用于在Buff之间传递参数")]
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
        public override string GetInfo()
        {
            return $"{nameof(AddRandomBuffs)}({string.Join(",", IDs)})";
        }
    }

    [NodeInfo(typeof(ActionNode), "终止触发事件", 180, "执行/终止触发事件"), AssetFilter(true, typeof(BuffAsset))]
    [Prompt(@"当条件为真时,终止触发事件,如完整的命中伤害流程为:
    闪避检测->暴击检测->伤害计算->护盾结算->生命值结算->等等
    在任意触发环节中插入StopTrigEvent,可以终止后续的触发事件,如在暴击检测时插入,如果目标造成了暴击则阻止后续流程可以实现单位只能受到非暴击伤害")]
    public class StopTrigEvent : ActionNode
    {
        [Child, TitlePort]
        [Prompt(@"终止触发事件的条件")]
        public Condition Condition;

        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return !Condition.GetResult(info, cache);
        }
    }
    [NodeInfo(typeof(ActionNode), "尝试移除Buff", 180, "执行/尝试移除Buff"), AssetFilter(true, typeof(BuffAsset))]
    [Prompt(@"尝试移除单位身上的同IDBuff")]
    public class TryRemoveBuff : ActionNode
    {
        [Child(true), TitlePort]
        [Prompt(@"用于获取需要移除Buff的对象")]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        [Prompt(@"Buff的ID")]
        public string ID;
        [Child, LabelInfo(Text = "成功", Width = 15)]
        [Prompt(@"成功移除Buff时执行的ActionNode")]
        public ActionNode True;
        [Child, LabelInfo(Text = "失败", Width = 15)]
        [Prompt(@"未成功移除Buff时执行的ActionNode")]
        public ActionNode False;
        static DropdownList<string> Buffs => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns;


        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            return true;
        }
        public override string GetInfo()
        {
            return $"{nameof(TryRemoveBuff)}({ID})";
        }
    }
    [NodeInfo(typeof(ActionNode), "修改战斗缓存", 180, "执行/修改战斗缓存"), AssetFilter(true, typeof(BuffAsset))]
    [Prompt(@"可以修改战斗缓存中的数值,如伤害值/治疗量等,如:
    Buff的效果是将受到的首次伤害减半,则可以通过CombatCacheModify修改TotalDmg的数值为TotalDmg/2,当这个效果触发一次后移除这个buff即可实现,
    这个节点仅适用于能够产生战斗缓存的触发器类型之后,如Dodge/Heal/Dmg/SPDmg等")]
    public class CombatCacheModify : ActionNode
    {
        [ShowInNode, LabelInfo(Hide = true)]
        [Prompt(@"需要修改的战斗缓存类型")]
        public CombatCacheType CacheType;
        [ShowInNode, LabelInfo(Hide = true), Group("Value",Width = 50)]
        [Prompt(@"数值修改的计算类型")]
        public ValueModType ModType;
        [Child, LabelInfo(Hide = true), Group("Value")]
        [Prompt(@"修改的具体值")]
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
        public override string GetInfo()
        {
            return $"{nameof(CombatCacheModify)}({CacheType})";
        }
    }
    [NodeInfo(typeof(ActionNode), "按圆创建对象", 180, "执行/按圆创建对象")]
    [Prompt(@"可以以指定的圆形分布创建指定数量的对象")]
    public class CreateObjsInCircle : ActionNode
    {
        [Child(true), TitlePort]
        [Prompt(@"所创建的对象")]
        public ObjNode ObjNode;
        [Child(true), LabelInfo("起始位置")]
        [Prompt(@"圆心的位置及朝向")]
        public TransformNode Origin;
        [ShowInNode, LabelInfo("数量")]
        [Prompt(@"创建的对象数量")]
        public FuncValue Count;
        [ShowInNode, LabelInfo("使用角度")]
        [Prompt(@"是否使用角度,如果为true,则会以朝向为中轴按照角度分布对象,否则按照数量对360°进行角度平分分布对象")]
        public bool ByAngle;
        [ShowInNode(ShowIf =nameof(ByAngle)), LabelInfo("角度")]
        [Prompt(@"使用角度时每个对象之间的角度差")]
        public FuncValue Angle;
        [ShowInNode, LabelInfo("放射距离")]
        [Prompt(@"对象到圆心的距离")]
        public FuncValue Radius;




        public override bool Handle(int trigCount, TrigInfo info, CombatCache cache)
        {
            int count =(int)Count.GetResult(info, cache);
            TransformCmp origin = Origin.GetResult(info, cache);
            float radius = Radius.GetResult(info, cache);
            float angle = ByAngle? Angle.GetResult(info, cache) : 360f / count;
            for (int i = 0; i < count; i++)
            {
                Angle currentAngle = origin.Rot + (ByAngle?(i - count / 2f + 0.5f):i) * angle;
                TransformCmp transformCmp = new ()
                {
                    Pos = origin.Pos + currentAngle.GetVector() * radius,
                    Rot = currentAngle
                };
                int entity  = EcsWorld.Inst.NewEntity();
                entity.Add(transformCmp);
                ObjNode.Create(entity, info, cache.Clone());
            }
            return true;
        }
    }




}
