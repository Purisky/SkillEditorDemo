using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#40E0D0")]
    [Prompt(@"Condition是条件节点的基类,所有的条件节点都继承自Condition")]
    public abstract class Condition : JsonNode, IGrowID<Condition>
    {
        [JsonIgnore] public int GrowID { get; set; }
        public static T Get<T>(int id) where T : Condition => (T)IGrowID<Condition>.Get(id);
        public abstract bool GetResult(TrigInfo info, CombatCache cache);
        public bool GetResult(TrigInfo info) => GetResult(info, CombatCache._);
        public bool GetResult(CombatCache cache) => GetResult(TrigInfo.Empty, cache);
        public bool GetResult() => GetResult(TrigInfo.Empty);
    }
    [NodeInfo(typeof(Condition), "比较", 100, "条件/比较"), PortColor("#0000ff")]
    [Prompt(@"比较节点,用于比较两个值的关系")]
    public partial class Compare : Condition
    {
        [Child, LabelInfo(Hide = true)]
        [Prompt(@"左侧的值")]
        public FuncValue Left;
        [JsonProperty, ShowInNode, LabelInfo(Hide = true)]
        [Prompt(@"比较的类型")]
        public CompareType CompareType;
        [Child, LabelInfo(Hide = true)]
        [Prompt(@"右侧的值")]
        public FuncValue Right;
        public override string GetText(int indent = 0)
        {
            string left = Left?.GetText(0) ?? "0";
            string right = Right?.GetText(0) ?? "0";
            string compareText = CompareType switch
            {
                CompareType.GreaterThan => " > ",
                CompareType.GreaterThanOrEqual => " ≥ ",
                CompareType.LessThan => " < ",
                CompareType.LessThanOrEqual => " ≤ ",
                CompareType.Equal => " = ",
                CompareType.NotEqual => " ≠ ",
                _ => " ? "
            };
            return $"({left}{compareText}{right})";
        }
        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            float left = Left.GetResult(info, cache);
            float right = Right.GetResult(info, cache);
            return CompareType switch
            {
                CompareType.GreaterThan => left > right,
                CompareType.GreaterThanOrEqual => left >= right,
                CompareType.LessThan => left < right,
                CompareType.LessThanOrEqual => left <= right,
                CompareType.Equal => left == right,
                CompareType.NotEqual => left != right,
                _ => false
            };
        }
    }
    [NodeInfo(typeof(Condition), "与", 80, "条件/逻辑/与", "#000080"), PortColor("#0000ff")]
    [Prompt(@"与节点,用于判断多个条件是否同时成立")]
    public partial class And : Condition
    {
        [Child(true), TitlePort]
        [Prompt(@"条件列表")]
        public List<Condition> Conditions;
        public override string GetText(int indent = 0)
        {
            if (Conditions == null || Conditions.Count == 0) { return "真"; }
            if (Conditions.Count == 1) { return Conditions[0].GetText(0); }
            return $"({string.Join(" & ", Conditions.Select(n => n.GetText(0)))})";
        }
        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            if (Conditions == null || Conditions.Count == 0) { return true; }
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (!Conditions[i].GetResult(info, cache)) { return false; }
            }
            return true;

        }
    }
    [NodeInfo(typeof(Condition), "或", 80, "条件/逻辑/或", "#D2691E"), PortColor("#0000ff")]
    [Prompt(@"或节点,用于判断多个条件是否有至少一个成立")]
    public partial class Or : Condition
    {
        [Child(true), TitlePort]
        [Prompt(@"条件列表")]
        public List<Condition> Conditions;
        public override string GetText(int indent = 0)
        {
            if (Conditions == null || Conditions.Count == 0) { return "真"; }
            if (Conditions.Count == 1) { return Conditions[0].GetText(0); }
            return $"({string.Join(" | ", Conditions.Select(n => n.GetText(0)))})";
        }

        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            if (Conditions == null || Conditions.Count == 0) { return true; }
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i].GetResult(info, cache)) { return true; }
            }
            return false;

        }
    }
    [NodeInfo(typeof(Condition), "非", 80, "条件/逻辑/非", "#800000"), PortColor("#0000ff")]
    [Prompt(@"非节点,用于取反一个条件")]
    public partial class Not : Condition
    {
        [Child(true), TitlePort]
        [Prompt(@"用于取反的条件")]
        public Condition Condition;
        public override string GetText(int indent = 0)
        {
            if (Condition == null) { return "真"; }
            return $"(!{Condition.GetText(0)})";
        }

        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            return !Condition.GetResult(info, cache);
        }
    }

}