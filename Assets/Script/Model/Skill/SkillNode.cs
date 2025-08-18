using SkillEditorDemo.Utility;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [NodeInfo(null, "技能", 400, "技能/技能"), AssetFilter(true, typeof(SkillAsset))]
    [Prompt(@"技能主要节点,用于描述一个技能的基本信息")]
    public partial class SkillNode : JsonNode, IUniqNode, IData<SkillNode>
    {
        [ShowInNode, LabelInfo("ID", 60)]
        [Prompt(@"技能的ID,不可重复")]
        public string ID { get; set; }
        [ShowInNode, LabelInfo("Name", 60)]
        [Prompt(@"技能的显示名称,可以在名称中添加'/'进行分类,如 负面效果/持续伤害/中毒")]
        public string Name { get; set; }
        [ShowInNode, LabelInfo("冷却时间", 60)]
        [Prompt(@"技能的冷却时间")]
        public TimeValue Time;
        [ShowInNode, LabelInfo("最大等级", 60)]
        [Prompt(@"技能的最大等级")]
        public FuncValue MaxLevel = 1;
        [ShowInNode, LabelInfo("最大充能", 60)]
        [Prompt(@"技能的最大充能次数,每次成功释放技能消耗一次充能,每次冷却时回复一次充能")]
        public FuncValue MaxCharge = 1;
        [ShowInNode, LabelInfo("监听类型", 60),Group("Condition")]
        [Prompt(@"技能的监听数值类型,决定技能是否可用,如技能消耗魔法则监听魔法值变化,当魔法值变化时,检测条件过滤,通过时允许释放技能")]
        public SkillWatchType SkillWatchType;
        [Child, LabelInfo("条件过滤", 60), Group("Condition")]
        [Prompt(@"技能的条件过滤,当不为空时对技能释放进行过滤,如魔法值不足,返回否则无法释放技能")]
        public Condition Condition;
        [Child(true), LabelInfo("效果组")]
        [Prompt(@"技能的效果组,用于描述技能的触发条件和效果")]
        public List<ActionNode> Actions;

        public override string GetText(int indent = 0)
        {
            string indentStr = new string(' ', indent * 2);
            string childIndentStr = new string(' ', (indent + 1) * 2);
            
            List<string> lines = new List<string>();
            
            string nameText = !string.IsNullOrEmpty(Name) ? Name : (!string.IsNullOrEmpty(ID) ? ID : "技能");
            
            // 主描述行
            lines.Add($"{indentStr}技能: {nameText}");
            
            // ID信息
            if (!string.IsNullOrEmpty(ID))
            {
                lines.Add($"{childIndentStr}ID: {ID}");
            }
            
            // 冷却时间
            string timeText = Time.GetText();
            if (!string.IsNullOrEmpty(timeText))
            {
                lines.Add($"{childIndentStr}冷却时间: {timeText}");
            }
            else
            {
                lines.Add($"{childIndentStr}冷却时间: 无");
            }
            
            // 最大等级
            string maxLevelText = MaxLevel?.GetText();
            if (!string.IsNullOrEmpty(maxLevelText) && maxLevelText != "1")
            {
                lines.Add($"{childIndentStr}最大等级: {maxLevelText}");
            }
            
            // 最大充能
            string maxChargeText = MaxCharge?.GetText();
            if (!string.IsNullOrEmpty(maxChargeText) && maxChargeText != "1")
            {
                lines.Add($"{childIndentStr}最大充能: {maxChargeText}");
            }
            
            // 监听类型
            if (SkillWatchType != SkillWatchType.None)
            {
                lines.Add($"{childIndentStr}监听类型: {SkillWatchType}");
            }
            
            // 条件过滤
            if (Condition != null)
            {
                string conditionText = Condition.GetText(indent + 1);
                lines.Add($"{childIndentStr}释放条件: {conditionText}");
            }
            
            // 效果组
            if (Actions != null && Actions.Count > 0)
            {
                if (Actions.Count == 1)
                {
                    lines.Add($"{childIndentStr}技能效果:");
                    string actionText = Actions[0].GetText(indent + 2);
                    lines.Add(actionText);
                }
                else
                {
                    lines.Add($"{childIndentStr}技能效果组({Actions.Count}个):");
                    for (int i = 0; i < Actions.Count; i++)
                    {
                        lines.Add($"{childIndentStr}  {i + 1}.");
                        string actionText = Actions[i].GetText(indent + 2);
                        lines.Add(actionText);
                    }
                }
            }
            else
            {
                lines.Add($"{childIndentStr}技能效果: 无");
            }
            
            return string.Join("\n", lines);
        }

        public void Cast(int trigCount,TrigInfo trigInfo,CombatCache cache)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Handle(trigCount, trigInfo, cache);
            }
        }

    }
}
