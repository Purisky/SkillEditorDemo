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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
    [NodeInfo(typeof(List<UnitNode>), "获取所有单位", 140, "单位/获取所有")]
    public class GetAllUnits : UnitNode
    {
        public override string GetText() => $"所有单位";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            throw new NotImplementedException();
        }
    }
    [NodeInfo(typeof(List<UnitNode>), "单位筛选", 140, "单位/筛选")]
    public class UnitFilter : UnitNode
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
            throw new NotImplementedException();
        }
    }


    [NodeInfo(typeof(List<UnitNode>), "单位排序", 140, "单位/排序")]
    public class OrderUnits : UnitNode
    {
        [Child(true), TitlePort]
        public List<UnitNode> UnitList;
        [ShowInNode, LabelInfo("比较数值", 45)]
        public FuncValue Min;
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
            throw new NotImplementedException();
        }
    }
    [NodeInfo(typeof(UnitNode), "取出单位", 140, "单位/取出")]
    public class TakeOutUnit : UnitNode
    {
        [Child(true), TitlePort]
        public List<UnitNode> UnitList;
        [ShowInNode, LabelInfo("索引", 45)]
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
            throw new NotImplementedException();
        }
    }
    [NodeInfo(typeof(List<UnitNode>), "当前单位列表", 140, "单位/当前单位列表")]
    public class LastUnitList : UnitNode
    {
        public override string GetText()=> $"当前单位列表";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            throw new NotImplementedException();
        }
    }

    [NodeInfo(typeof(UnitNode), "单位迭代器", 140, "单位/迭代器")]
    public class UnitEnumerator : UnitNode
    {
        public override string GetText() => $"单位迭代器";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            throw new NotImplementedException();
        }
    }
    [NodeInfo(typeof(Condition), "碰撞单位", 100, "单位/碰撞")]
    public class UnitCollision : UnitNode
    {
        public override string GetText() => $"碰撞单位";

        public override List<Unit> GetUnits(TrigInfo info, CombatCache cache)
        {
            throw new NotImplementedException();
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
