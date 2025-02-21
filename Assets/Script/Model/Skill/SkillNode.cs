using SkillEditorDemo.Utility;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [NodeInfo(null, "技能", 400, "技能/技能"), AssetFilter(true, typeof(SkillAsset))]
    public class SkillNode : JsonNode, IUniqNode, IData<SkillNode>
    {
        [ShowInNode, LabelInfo("ID", 60)]
        public string ID { get; set; }
        [ShowInNode, LabelInfo("Name", 60)]
        public string Name { get; set; }
        [ShowInNode, LabelInfo("冷却时间", 60)]
        public TimeValue Time;
        [ShowInNode, LabelInfo("最大等级", 60)]
        public FuncValue MaxLevel = 1;
        [ShowInNode, LabelInfo("最大充能", 60)]
        public FuncValue MaxCharge = 1;
        [ShowInNode, LabelInfo("监听类型", 60),Group("Condition")]
        public SkillWatchType SkillWatchType;
        [Child, LabelInfo("条件过滤", 60), Group("Condition")]
        public Condition Condition;
        [Child(true), LabelInfo("效果组")]
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
