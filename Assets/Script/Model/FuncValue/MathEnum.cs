using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public enum CalculateType
    {
        [LabelInfo("+")]
        Add,
        [LabelInfo("-")]
        Sub,
        [LabelInfo("*")]
        Mul,
        [LabelInfo("∕")]
        Div,
        [LabelInfo("%")]
        Mod,
        [LabelInfo("~")]
        Random
    }

    public enum LogicType
    {
        [LabelInfo("&")]
        And,
        [LabelInfo("|")]
        Or,
        [LabelInfo("!")]
        Not
    }
    public enum CompareType
    {
        [LabelInfo(">")]
        GreaterThan,
        [LabelInfo("≥")]
        GreaterThanOrEqual,
        [LabelInfo("<")]
        LessThan,
        [LabelInfo("≤")]
        LessThanOrEqual,
        [LabelInfo("=")]
        Equal,
        [LabelInfo("≠")]
        NotEqual,
    }

}
