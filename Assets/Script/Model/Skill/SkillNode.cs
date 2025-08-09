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

        public void Cast(int trigCount,TrigInfo trigInfo,CombatCache cache)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Handle(trigCount, trigInfo, cache);
            }
        }

    }
}
