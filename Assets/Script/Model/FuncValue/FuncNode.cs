using Newtonsoft.Json;
using System.Collections.Generic;
using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#7CFC00")]
    public abstract class FuncNode : NumNode
    { 
    }
    [PortColor("#7CFC00")]
    public  class FuncValue : NumValue<FuncNode>
    {

    }
    [NodeInfo(typeof(FuncNode), "Buff取值", 100, "取值/Buff"),AssetFilter(true,typeof(BuffAsset))]
    public class BuffValueGetter : FuncNode
    {
        [ShowInNode,LabelInfo(Hide =true)]
        public BuffValueType Type;



        public override string GetText()
        {
            return $"Buff.{Type.GetLabel()}";
        }
    }
    [NodeInfo(typeof(FuncNode), "战斗数值缓存", 100, "取值/战斗数值缓存")]
    public class CombatCacheGetter : FuncNode
    {
        [ShowInNode, LabelInfo(Hide = true)]
        public CombatCacheType Type;

        public override string GetText()
        {
            return $"战斗数值缓存.{Type.GetLabel()}";
        }
    }
    [NodeInfo(typeof(FuncNode), "属性取值", 140, "取值/属性")]
    public class StatGetter : FuncNode
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;
        [ShowInNode,LabelInfo(Hide = true)]
        public StatType Type;


        public override string GetText()
        {
            return $"{UnitNode?.GetText()??"Null"}.{Type.GetLabel().Replace("/","_")}";
        }
    }
    [NodeInfo(typeof(FuncNode), "技能取值", 100, "取值/技能")]
    public class SkillValueGetter : FuncNode
    {
        public override string GetText()
        {
            return "技能";
        }
    }
}
