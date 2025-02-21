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
    public abstract class UnitNode : JsonNode
    {
        public virtual string GetText()
        {
            return "单位";
        }
        public abstract List<Unit> GetUnits(TrigInfo info,CombatCache cache);


    }
    [NodeInfo(typeof(UnitNode), "当前单位", 100, "单位/当前单位")]
    public class LastUnit : UnitNode
    {
        public override string GetText() => "当前单位";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.CurrentID) };
        }
    }
    [NodeInfo(typeof(UnitNode), "Buff单位获取", 140, "单位/Buff单位获取"), AssetFilter(true, typeof(BuffAsset))]
    public class GetBuffUnit : UnitNode
    {
        [ShowInNode, LabelInfo(Hide = true)]
        public BuffUnitType BuffUnitType;
        public override string GetText() => $"Buff.{BuffUnitType.GetLabel()}";

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
    public class GetAllUnits : UnitNode
    {
        public override string GetText() => $"所有单位";
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
    public class UnitFilter : UnitNode//todo optimize
    {
        [Child(true), TitlePort]
        public List<UnitNode> UnitList;
        [Child(true), LabelInfo("条件")]
        public Condition Condition;

        public override string GetText()
        {
            string unitText = "";
            if (UnitList != null)
            {
                unitText = string.Join(",", UnitList.Select(n => n.GetText()));
            }
            return $"([{unitText}]=>{Condition.GetText()})";
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
    public class OrderUnits : UnitNode//todo
    {
        [Child(true), TitlePort]
        public List<UnitNode> UnitList;
        [ShowInNode, LabelInfo("比较数值", 45)]
        public FuncValue Compare;
        [ShowInNode, LabelInfo("反序", 30)]
        public bool ByDescending;

        public override string GetText()
        {
            string unitText = "";
            if (UnitList != null)
            {
                unitText = string.Join(",", UnitList.Select(n => n.GetText()));
            }
            return $"([{unitText}].排序)";
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
    public class TakeOutUnit : UnitNode
    {
        [Child(true), TitlePort]
        public List<UnitNode> UnitList;
        [ShowInNode, LabelInfo("索引", 45)]//-1=^1
        public FuncValue Index;
        public override string GetText()
        {
            string unitText = "";
            if (UnitList != null)
            {
                unitText = string.Join(",", UnitList.Select(n => n.GetText()));
            }
            return $"([{unitText}].[{Index.GetText()}])";
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
    public class LastUnitList : UnitNode
    {
        public override string GetText()=> $"当前单位列表";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return info.CacheList.Select(e => Unit.Get(e)).ToList();
        }
    }

    [NodeInfo(typeof(UnitNode), "单位迭代器", 140, "单位/迭代器")]
    public class UnitEnumerator : UnitNode
    {
        public override string GetText() => $"单位迭代器";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.EnumeratorID) };
        }
    }

    [NodeInfo(typeof(UnitNode), "触发单位", 140, "单位/触发单位")]
    public class TriggerUnit : UnitNode
    {
        public override string GetText() => $"触发单位";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.TriggerID) };
        }
    }
    [NodeInfo(typeof(UnitNode), "源", 140, "单位/源")]
    public class SourceUnit : UnitNode// buff source/skill owner
    {
        public override string GetText() => $"源";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            return new List<Unit> { Unit.Get(info.SourceID) };
        }
    }



    public static class UnitNodeExtensions
    { 
        public static Unit GetUnit(this UnitNode node, TrigInfo info, CombatCache cache)
        {
            return node.GetUnits(info, cache)[0];
        }
    }
}
