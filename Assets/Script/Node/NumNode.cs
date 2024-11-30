using Newtonsoft.Json;
using System.Collections.Generic;
using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    //[NodeInfo(typeof(NumNode), "全局取值", 100, "取值/全局")]
    //public class GlobalValueGetter : NumNode
    //{
    //    public override string GetText()
    //    {
    //        return "全局";
    //    }
    //}
    [NodeInfo(typeof(NumNode), "Buff取值", 100, "取值/Buff"),AssetFilter(true,typeof(BuffAsset))]
    public class BuffValueGetter : NumNode
    {
        [ShowInNode,LabelInfo(Hide =true)]
        public BuffValueType Type;



        public override string GetText()
        {
            return $"Buff.{Type.GetLabel()}";
        }
    }
    [NodeInfo(typeof(NumNode), "Buff缓存", 100, "取值/Buff缓存"), AssetFilter(true, typeof(BuffAsset))]
    public class BuffCacheGetter : NumNode
    {
        [ShowInNode, LabelInfo(Hide = true)]
        public BuffCacheType Type;

        public override string GetText()
        {
            return $"Buff缓存.{Type.GetLabel()}";
        }
    }
    [NodeInfo(typeof(NumNode), "属性取值", 140, "取值/属性")]
    public class StatGetter : NumNode
    {
        [Child(true), TitlePort]
        public Entity Entity;
        [ShowInNode,LabelInfo(Hide = true)]
        public StatType Type;


        public override string GetText()
        {
            return $"{Entity?.GetText()??"Null"}.{Type.GetLabel().Replace("/","_")}";
        }
    }
    [NodeInfo(typeof(NumNode), "技能取值", 100, "取值/技能")]
    public class SkillValueGetter : NumNode
    {
        public override string GetText()
        {
            return "技能";
        }
    }
}
