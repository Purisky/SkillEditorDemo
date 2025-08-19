using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;
namespace SkillEditorDemo.Model
{
    [NodeInfo(null, "Buff", 400, "Buff/Buff"), AssetFilter(true, typeof(BuffAsset))]
    [Prompt(@"Buff主要节点,用于描述一个Buff的基本信息")]
    public partial class BuffNode : JsonNode, IUniqNode, IData<BuffNode>
    {
        [ShowInNode, LabelInfo("ID", 60)]
        [Prompt(@"Buff的ID,不可重复")]
        public string ID { get; set; }
        [ShowInNode, LabelInfo("Name", 60), JsonProperty]
        [Prompt(@"Buff的显示名称,可以在名称中添加'/'进行分类,如 负面效果/持续伤害/中毒")]
        public string Name { get; set; }
        [ShowInNode, LabelInfo("竞争机制", 60), Group("Compete")]
        [Prompt(@"Buff的竞争机制,当添加同ID Buff时,决定如何处理")]
        public CompeteType CompeteType;
        [ShowInNode, LabelInfo("源内竞争", 60), Group("Compete"),Style]
        [Prompt(@"Buff的竞争机制的补充,当为true时,只有相同来源的同ID Buff才会引发竞争机制")]
        public bool CompeteInSource;
        [ShowInNode, LabelInfo("最大等级", 60), Group("level")]
        [Prompt(@"Buff的最大等级")]
        public int MaxLevel = 1;
        [ShowInNode, LabelInfo("最大层", 60), Group("level")]
        [Prompt(@"Buff的最大层数")]
        public FuncValue MaxDegree = new() { Value = 1 };
        [ShowInNode, LabelInfo("持续时间", 60)]
        [Prompt(@"Buff的持续时间")]
        public TimeValue Time;
        [Child, LabelInfo("触发器组")]
        [Prompt(@"Buff的触发器组,用于描述Buff的触发条件和效果")]
        public List<TrigNode> Triggers;

        public override string GetText(int indent = 0)
        {
            string indentStr = new string('\t', indent);
            string childIndentStr = new string('\t', indent + 1);
            
            List<string> lines = new List<string>();
            
            string nameText = !string.IsNullOrEmpty(Name) ? Name : (!string.IsNullOrEmpty(ID) ? ID : "Buff");
            
            // 主描述行
            lines.Add($"{indentStr}Buff: {nameText}({ID})");
            // 持续时间
            string timeText = Time.GetText();
            if (!string.IsNullOrEmpty(timeText))
            {
                lines.Add($"{childIndentStr}持续时间: {timeText}");
            }
            else
            {
                lines.Add($"{childIndentStr}持续时间: 无限");
            }
            
            // 最大等级
            if (MaxLevel > 1)
            {
                lines.Add($"{childIndentStr}最大等级: {MaxLevel}");
            }
            
            // 最大层数
            string maxDegreeText = MaxDegree?.GetText();
            if (!string.IsNullOrEmpty(maxDegreeText) && maxDegreeText != "1")
            {
                lines.Add($"{childIndentStr}最大层数: {maxDegreeText}");
            }
            
            // 竞争机制
            string competeText = CompeteType.GetLabel();
            if (CompeteInSource)
            {
                competeText += "(源内竞争)";
            }
            lines.Add($"{childIndentStr}竞争机制: {competeText}");
            
            // 触发器信息
            if (Triggers != null && Triggers.Count > 0)
            {
                if (Triggers.Count == 1)
                {
                    string triggerText = Triggers[0].GetText(indent + 1);
                    lines.Add(triggerText);
                }
                else
                {
                    for (int i = 0; i < Triggers.Count; i++)
                    {
                        string triggerText = Triggers[i].GetText(indent + 1).TrimStart('\t');
                        lines.Add($"{childIndentStr}{i + 1}.{triggerText}");
                    }
                }
            }
            return string.Join("\n", lines);
        }

        public override string GetInfo()
        {
            return $"{nameof(BuffNode)}({ID})";
        }
    }

}

