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
        [ShowInNode, LabelInfo("源内竞争", 60), Group("Compete")]
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

        public override string GetInfo()
        {
            return $"{nameof(BuffNode)}({ID})";
        }
    }

}

