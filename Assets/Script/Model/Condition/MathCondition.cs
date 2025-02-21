using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [PortColor("#40E0D0")]
    public abstract class Condition : JsonNode, IGrowID<Condition>
    {
        [JsonIgnore] public int GrowID { get; set; }
        public static T Get<T>(int id) where T : Condition => (T)IGrowID<Condition>.Get(id);
        public abstract bool GetResult(TrigInfo info, CombatCache cache);
        public bool GetResult(TrigInfo info) => GetResult(info, CombatCache._);
        public bool GetResult(CombatCache cache) => GetResult(TrigInfo.Empty, cache);
        public bool GetResult() => GetResult(TrigInfo.Empty);
        public abstract string GetText();
    }
    [NodeInfo(typeof(Condition), "比较", 100, "条件/比较"), PortColor("#0000ff")]
    public class Compare : Condition
    {
        [Child, LabelInfo(Hide = true)]
        public FuncValue Left;
        [JsonProperty, ShowInNode, LabelInfo(Hide = true)]
        public CompareType CompareType;
        [Child, LabelInfo(Hide = true)]
        public FuncValue Right;



        public override string GetText()
        {
            string left = Left == null ? "0" : Left.GetText();
            string right = Right == null ? "0" : Right.GetText();
            string compareText = CompareType switch
            {
                CompareType.GreaterThan => ">",
                CompareType.GreaterThanOrEqual => "≥",
                CompareType.LessThan => "<",
                CompareType.LessThanOrEqual => "≤",
                CompareType.Equal => "=",
                CompareType.NotEqual => "≠",
                _ => "?"
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
    public class And : Condition
    {
        [Child(true), TitlePort]
        public List<Condition> Conditions;
        public override string GetText()
        {
            if (Conditions.Count == 0) { return "true"; }
            return $"({string.Join("&", Conditions.Select(n => n.GetText()))})";
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
    public class Or : Condition
    {
        [Child(true), TitlePort]
        public List<Condition> Conditions;
        public override string GetText()
        {
            if (Conditions == null || Conditions.Count == 0) { return "true"; }
            return $"({string.Join("|", Conditions.Select(n => n.GetText()))})";
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
    public class Not : Condition
    {
        [Child(true), TitlePort]
        public Condition Condition;
        public override string GetText()
        {
            if (Condition == null) { return "true"; }
            return $"(!{Condition.GetText()})";
        }

        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            return !Condition.GetResult(info, cache);
        }
    }

}