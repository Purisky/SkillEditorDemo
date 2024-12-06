using Newtonsoft.Json;
using SkillEditorDemo;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using UnityEngine;

namespace TreeNode.Runtime
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
    }



}

