using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#7CFC00")]
    public abstract class FuncNode : NumNode, IGrowID<FuncNode>
    {
        [JsonIgnore] public int GrowID { get; set; }
        public static T Get<T>(int id) where T : FuncNode => (T)IGrowID<FuncNode>.Get(id);
        public abstract float GetResult(TrigInfo info, CombatCache cache);
    }
    public class FuncValue : NumValue<FuncNode>
    {
        [JsonIgnore] FuncNode FuncNode {
            get {
                if (funcNode == null) {
                    if (Node == null) { funcNode = new ConstValue() { Value = Value };  }
                    else { funcNode = (FuncNode)Node; }
                }
                return funcNode;
            }
        }
        [JsonIgnore] FuncNode funcNode;
        public float GetResult(TrigInfo info, CombatCache cache)
        { 
            return FuncNode.GetResult(info, cache);
        }
        public float GetResult(TrigInfo info) => GetResult(info, CombatCache._);
        public float GetResult(CombatCache cache) => GetResult(TrigInfo.Empty, cache);
        public float GetResult() => GetResult(TrigInfo.Empty);
    }

    public class ConstValue : FuncNode
    {
        public float Value;
        public override string GetText()
        {
            return $"{Value}";
        }
        public override float GetResult(TrigInfo info, CombatCache cache)
        {
            return Value;
        }
    }
    
    public struct TimeValue
    {
        [ShowInNode,Group("type",Width = 40),LabelInfo(Hide =true)]
        public TimeType Type;
        [Child,LabelInfo(Hide =true), Group("type")]
        public FuncValue Value;

        public readonly int GetTick(TrigInfo info, CombatCache cache)
        {
            float value = Value.GetResult(info, cache);
            return Type== TimeType.GameTick ? (int)value : (int)(value * Time.GameTickPerSec);
        }
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

        public override float GetResult(TrigInfo info, CombatCache cache)
        {
            throw new System.NotImplementedException();
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

        public override float GetResult(TrigInfo info, CombatCache cache)
        {
            return cache[Type];
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

        public override float GetResult(TrigInfo info, CombatCache cache)
        {
            Unit unit = UnitNode.GetUnit(info, cache);
            return unit?.StatHandler[Type]??0;
        }
    }
    [NodeInfo(typeof(FuncNode), "技能取值", 100, "取值/技能")]
    public class SkillValueGetter : FuncNode
    {
        public override string GetText()
        {
            return "技能";
        }

        public override float GetResult(TrigInfo info, CombatCache cache)
        {
            throw new System.NotImplementedException();
        }
    }
}
