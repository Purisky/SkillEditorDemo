using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{

    [NodeInfo(typeof(FuncNode), "二元运算", 100, "计算/二元运算"), PortColor("#00ff00")]
    public class BinaryCal: FuncNode
    {
        [Child, LabelInfo(Hide = true)]
        public FuncValue Left;
        [JsonProperty,ShowInNode,LabelInfo(Hide =true)]
        public CalculateType CalculateType;
        [Child, LabelInfo(Hide = true)]
        public FuncValue Right;

        public override string GetText()
        {
            string left = Left == null ? "0" : Left.GetText();
            string right = Right == null ? "0" : Right.GetText();
            string calculateText = CalculateType switch
            {
                CalculateType.Add => "+",
                CalculateType.Sub => "-",
                CalculateType.Mul => "*",
                CalculateType.Div => "/",
                CalculateType.Mod => "%",
                CalculateType.Random => "~",
                _=>"?"
            };
            return $"({left}{calculateText}{right})";
        }

        public override float GetResult(TrigInfo info, CombatCache cache)
        {
            float left = Left.GetResult(info, cache);
            float right = Right.GetResult(info, cache);
            return CalculateType switch
            {
                CalculateType.Add => left + right,
                CalculateType.Sub => left - right,
                CalculateType.Mul => left * right,
                CalculateType.Div => right == 0 ? 0 : left / right,
                CalculateType.Mod => right == 0 ? 0 : left % right,
                CalculateType.Random =>  FixRandom.Range(left, right),
                _ => 0
            };
        }
    }


    [NodeInfo(typeof(FuncNode), "三元运算", 100, "计算/三元运算"), PortColor("#00ff00")]
    public class ConditionCal : FuncNode
    {
        [Child(true), TitlePort]
        public Condition Condition;
        [Child, LabelInfo(Text = "真",Width =10)]
        public FuncValue True;
        [Child, LabelInfo(Text = "假", Width = 10)]
        public FuncValue False;
        public override string GetText()
        {
            string _true = True == null ? "0" : True.GetText();
            string _false = False == null ? "0" : False.GetText();
            if (Condition == null)
            {
                return _true;
            }
            return $"({Condition.GetText()}?{_true}:{_false})";
        }
        public override float GetResult(TrigInfo info, CombatCache cache)
        {
            bool condition = Condition.GetResult(info, cache);
            return condition ? True.GetResult(info, cache) : False.GetResult(info, cache);
        }
    }



}

