using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#ECDFB0")]
    [Prompt(@"单位节点的基类,所有的单位节点都继承自UnitNode,用于在游戏场景中获取单位信息如当前单位/所有单位/筛选单位等")]
    public abstract class UnitNode : JsonNode
    {
        public abstract List<Unit> GetUnits(TrigInfo info,CombatCache cache);


    }
    [NodeInfo(typeof(UnitNode), "当前单位", 100, "单位/当前单位")]
    [Prompt(@"获取当前单位,在触发逻辑中最近的单位,这是一个在运行时随时可能变化对象的单位节点,如果有更加准确的单位节点,应该尽量避免使用该节点")]
    public partial class LastUnit : UnitNode
    {
        public override string GetText(int indent = 0) => "当前单位";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.CurrentID) };
        }
    }
    [NodeInfo(typeof(UnitNode), "Buff单位获取", 140, "单位/Buff单位获取"), AssetFilter(true, typeof(BuffAsset))]
    [Prompt(@"获取当前Buff的涉及单位,如Buff的携带者/创建者等")]
    public partial class GetBuffUnit : UnitNode
    {
        [ShowInNode, LabelInfo(Hide = true)]
        [Prompt(@"Buff单位的类型")]
        public BuffUnitType BuffUnitType;
        public override string GetText(int indent = 0) => $"Buff.{BuffUnitType.GetLabel()}";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            EcsPackedEntity entity = BuffUnitType switch
            {
                BuffUnitType.Carrier => info.BuffCarrierID,
                BuffUnitType.Creator => info.BuffCreatorID,
                _ => info.CurrentID
            };
            return new List<Unit> { Unit.Get(entity) };
        }
    }
    [NodeInfo(typeof(List<UnitNode>), "获取所有单位", 140, "单位/获取所有")]
    [Prompt(@"获取当前所有单位,性能敏感")]
    public partial class GetAllUnits : UnitNode
    {
        public override string GetText(int indent = 0) => $"所有单位";
        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            List<Unit> list = new();
            foreach (var entity in UnitSystem.Filter)//todo 
            {
                list.Add(Unit.Get(entity));
            }
            info.CacheList = list.Select(u => u.Entity).ToArray();
            return list;
        }
    }
    [NodeInfo(typeof(List<UnitNode>), "单位筛选", 140, "单位/筛选")]
    [Prompt(@"筛选单位列表,根据条件筛选出符合条件的单位")]
    public partial class UnitFilter : UnitNode//todo optimize
    {
        [Child(true), TitlePort]
        [Prompt(@"需要筛选的单位列表")]
        public List<UnitNode> UnitList;
        [Child(true), LabelInfo("条件")]
        [Prompt(@"筛选条件,只有满足条件的单位才会被保留,用于对比的单位需要从UnitEnumerator(单位迭代器)中取出进行对比")]
        public Condition Condition;

        public override string GetText(int indent = 0)
        {
            string unitText = "单位列表";
            if (UnitList != null && UnitList.Count > 0)
            {
                if (UnitList.Count == 1)
                {
                    unitText = UnitList[0].GetText(indent);
                }
                else
                {
                    unitText = $"[{string.Join(",", UnitList.Select(n => n.GetText(indent)))}]";
                }
            }
            
            string conditionText = Condition?.GetText(indent) ?? "条件";
            return $"筛选 {unitText} 中符合({conditionText})的单位";
        }

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            List<Unit> temp = UnitList.SelectMany(n => n.GetUnits(info, cache)).Distinct().ToList();
            List<Unit> result = new();
            for (int i = 0; i < temp.Count; i++)
            {
                info.EnumeratorID = temp[i].Entity;
                if (Condition.GetResult(info, cache))
                { 
                    result.Add(temp[i]);
                }
            }
            info.CacheList = result.Select(u => u.Entity).ToArray();
            return result;
        }
    }


    [NodeInfo(typeof(List<UnitNode>), "单位排序", 140, "单位/排序")]
    [Prompt(@"对单位列表进行排序,根据指定的比较数值进行排序")]
    public partial class OrderUnits : UnitNode//todo
    {
        [Child(true), TitlePort]
        [Prompt(@"需要排序的单位列表")]
        public List<UnitNode> UnitList;
        [ShowInNode, LabelInfo("比较数值", 45)]
        [Prompt(@"用于比较的数值,对比的单位需要从UnitEnumerator(单位迭代器)中取出进行对比")]
        public FuncValue Compare;
        [ShowInNode, LabelInfo("反序", 30)]
        [Prompt(@"是否反序排序,默认升序,如果为true则降序")]
        public bool ByDescending;

        public override string GetText(int indent = 0)
        {
            string unitText = "单位列表";
            if (UnitList != null && UnitList.Count > 0)
            {
                if (UnitList.Count == 1)
                {
                    unitText = UnitList[0].GetText(indent);
                }
                else
                {
                    unitText = $"[{string.Join(",", UnitList.Select(n => n.GetText(indent)))}]";
                }
            }
            
            string compareText = Compare?.GetText(indent) ?? "数值";
            string orderText = ByDescending ? "降序" : "升序";
            return $"排序 {unitText} (按 {compareText} {orderText})";
        }

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            List<Unit> temp = UnitList.SelectMany(n => n.GetUnits(info, cache)).Distinct().ToList();
            temp.Sort((a, b) =>
            {
                info.EnumeratorID = a.Entity;
                float aVal = Compare.GetResult(info, cache);
                info.EnumeratorID = b.Entity;
                float bVal = Compare.GetResult(info, cache);
                return (ByDescending?-1:1)* aVal.CompareTo(bVal);
            });
            info.CacheList = temp.Select(u => u.Entity).ToArray();
            return temp;
        }
    }
    [NodeInfo(typeof(UnitNode), "取出单位", 140, "单位/取出")]
    public partial class TakeOutUnit : UnitNode
    {
        [Child(true), TitlePort]
        [Prompt(@"需要取出的单位列表")]
        public List<UnitNode> UnitList;
        [ShowInNode, LabelInfo("索引", 45)]//-1=^1
        [Prompt(@"需要取出的单位的索引,如果为负数则从后向前取,如-1表示最后一个单位,0表示第一个单位")]
        public FuncValue Index;
        public override string GetText(int indent = 0)
        {
            string unitText = "单位列表";
            if (UnitList != null && UnitList.Count > 0)
            {
                if (UnitList.Count == 1)
                {
                    unitText = UnitList[0].GetText(indent);
                }
                else
                {
                    unitText = $"[{string.Join(",", UnitList.Select(n => n.GetText(indent)))}]";
                }
            }
            
            string indexText = Index?.GetText(indent) ?? "0";
            return $"取出 {unitText} 的第 {indexText} 个";
        }

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            List<Unit> temp = UnitList.SelectMany(n => n.GetUnits(info, cache)).Distinct().ToList();
            int index = (int)Index.GetResult(info, cache);
            if (index <0)
            {
                index = temp.Count + index;
            }
            index = Math.Clamp(index, 0, temp.Count - 1);
            return new List<Unit> { temp[index] };
        }
    }
    [NodeInfo(typeof(List<UnitNode>), "当前单位列表", 140, "单位/当前单位列表")]//cache
    [Prompt(@"获取最近缓存的单位列表,用于提升性能,避免无意义的列表获取或者筛选")]
    public partial class LastUnitList : UnitNode
    {
        public override string GetText(int indent = 0)=> $"缓存单位列表";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return info.CacheList.Select(e => Unit.Get(e)).ToList();
        }
    }

    [NodeInfo(typeof(UnitNode), "单位迭代器", 140, "单位/迭代器")]
    [Prompt(@"获取当前迭代器的单位,用于在迭代器中获取当前单位")]
    public partial class UnitEnumerator : UnitNode
    {
        public override string GetText(int indent = 0) => $"迭代单位";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.EnumeratorID) };
        }
    }

    [NodeInfo(typeof(UnitNode), "触发单位", 140, "单位/触发单位")]
    [Prompt(@"获取触发当前逻辑的单位,通常是技能/Buff/碰撞的触发者,比如触发器需要其他单位参与时的触发者")]
    public partial class TriggerUnit : UnitNode
    {
        public override string GetText(int indent = 0) => $"触发单位";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.TriggerID) };
        }
    }
    [NodeInfo(typeof(UnitNode), "源", 140, "单位/源")]
    [Prompt(@"获取当前技能/buff的来源单位,通常是技能的施法者或者Buff的源单位")]
    public partial class SourceUnit : UnitNode// buff source/skill owner
    {
        public override string GetText(int indent = 0) => $"源";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.SourceID) };
        }
    }



    public  static class UnitNodeExtensions
    { 
        public static Unit GetUnit(this UnitNode node, TrigInfo info, CombatCache cache)
        {
            return node.GetUnits(info, cache)[0];
        }
    }
}
