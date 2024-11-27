using Newtonsoft.Json;
using TreeNode.Runtime;
using UnityEngine;

namespace SkillEditorDemo
{
    [NodeInfo(typeof(NumNode), "取值", 100, "计算/取值"), PortColor("#00ff00")]
    public class ValueGetter : NumNode
    {
        [Child,LabelInfo("目标")]
        public Entity Entity;
        [Child, LabelInfo(Hide = true)]
        public NumValue Left;
        [JsonProperty, ShowInNode, LabelInfo(Hide = true)]
        public CalculateType CalculateType;
        [Child, LabelInfo(Hide = true)]
        public NumValue Right;
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
                CalculateType.DivInt => "//",
                CalculateType.Random => "~",
                _ => "?"
            };
            return $"({left}{calculateText}{right})";
        }
    }

}
