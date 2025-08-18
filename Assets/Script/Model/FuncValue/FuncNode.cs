using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#7CFC00")]
    [Prompt(@"FuncNode是数值节点的基类,所有的数值节点都继承自FuncNode")]
    public abstract class FuncNode : NumNode, IGrowID<FuncNode>
    {
        [JsonIgnore] public int GrowID { get; set; }
        public static T Get<T>(int id) where T : FuncNode => (T)IGrowID<FuncNode>.Get(id);
        public abstract float GetResult(TrigInfo info, CombatCache cache);
    }
    [Prompt(@"FuncValue是特殊的数值节点,同时包含一个float:Value和一个FuncNode:Node,既可以被视为一个常数,也可以添加其他数值节点用于复杂计算,是数值字段的主要类型,
常数和节点是互斥的存在,节点会优先被计算,当涉及到数值计算时需使用ListNodes(FuncNode)获取数值节点信息")]
    public partial class FuncValue : NumValue<FuncNode>
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
        /// <summary>
        /// 在正式项目中,需要在初始化时对该函数进行预编译,将嵌套调用转化为单次调用以获得性能提升
        /// </summary>
        /// <param name="info"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public float GetResult(TrigInfo info, CombatCache cache)
        {
            return FuncNode.GetResult(info, cache);
        }
        public float GetResult(TrigInfo info) => GetResult(info, CombatCache._);
        public float GetResult(CombatCache cache) => GetResult(TrigInfo.Empty, cache);
        public float GetResult() => GetResult(TrigInfo.Empty);

        public static implicit operator FuncValue(float value) => new () { Value = value };

        public override string ToString()
        {
            return FuncNode.GetText();
        }
    }

    /// <summary>
    /// 这个类是FuncNode的一个特殊实现,用于表示一个常数值,仅在运行时使用
    /// </summary>
    public partial class ConstValue : FuncNode
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
    [Prompt(@"时间类数值的主要类型,-1在大部分情况下被视为永久")]
    public partial struct TimeValue:IText
    {

        [ShowInNode,Group("type",Width = 60),LabelInfo(Hide =true)]
        [Prompt(@"时间单位的类型:Second(秒),GameTick(游戏帧)")]
        public TimeType Type;
        [Child,LabelInfo(Hide =true), Group("type")]
        [Prompt(@"时间单位的数值")]
        public FuncValue Value;

        public readonly int GetTick(TrigInfo info, CombatCache cache)
        {
            float value = Value.GetResult(info, cache);
            return Type== TimeType.GameTick ? (int)value : (int)(value * Time.GameTickPerSec);
        }

        public static implicit operator TimeValue(float value) => new() { Type = TimeType.Second, Value = value };

        public override readonly string ToString()
        {
            if (Type == TimeType.GameTick)
            {
                return $"{Value} 帧";
            }
            return $"{Value} 秒";
        }

        public string GetText()=> ToString();
    }
    [NodeInfo(typeof(FuncNode), "Buff取值", 100, "取值/Buff取值"),AssetFilter(true,typeof(BuffAsset))]
    [Prompt(@"从当前Buff中取值,如当前Buff的等级/层数以及BuffRuntimeData所存储的数值等")]
    public partial class BuffValueGetter : FuncNode
    {
        [ShowInNode,LabelInfo(Hide =true)]
        [Prompt(@"Buff数值的类型")]
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
    [NodeInfo(typeof(FuncNode), "战斗数值缓存", 120, "取值/战斗数值缓存")]
    [Prompt(@"从当前战斗数值缓存中取值")]
    public partial class CombatCacheGetter : FuncNode
    {
        [ShowInNode, LabelInfo(Hide = true)]
        [Prompt(@"战斗缓存的类型")]
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
    [Prompt(@"从指定单位的属性中取值")]
    public partial class StatGetter : FuncNode
    {
        [Child(true), TitlePort]
        [Prompt(@"获取指定单位的单位节点")]
        public UnitNode UnitNode;
        [ShowInNode,LabelInfo(Hide = true)]
        [Prompt(@"属性的类型")]
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
    [NodeInfo(typeof(FuncNode), "技能取值", 100, "取值/技能"), AssetFilter(true, typeof(SkillAsset))]
    [Prompt(@"从当前的技能中取值,如当前技能的等级/冷却时间/充能等")]
    public partial class SkillValueGetter : FuncNode
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
