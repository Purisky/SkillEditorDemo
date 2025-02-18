using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;
namespace SkillEditorDemo.Model
{
    [NodeInfo(null, "Buff", 400, "Buff/Buff"), AssetFilter(true, typeof(BuffAsset))]
    public class BuffNode : JsonNode, IUniqNode, IData<BuffNode>
    {
        [ShowInNode, LabelInfo("ID", 60)]
        public string ID { get; set; }
        [ShowInNode, LabelInfo("Name", 60), JsonProperty]
        public string Name { get; set; }
        [ShowInNode, LabelInfo("竞争机制", 60), Group("Compete")]
        public CompeteType CompeteType;
        [ShowInNode, LabelInfo("源内竞争", 60), Group("Compete")]
        public bool CompeteInSource;
        [ShowInNode, LabelInfo("最大等级", 60), Group("level")]
        public int MaxLevel = 1;
        [ShowInNode, LabelInfo("最大层", 60), Group("level")]
        public FuncValue MaxDegree = new() { Value = 1 };
        [ShowInNode, LabelInfo("持续时间", 60)]
        public TimeValue Time;
        [Child(true), LabelInfo("触发器组")]
        public List<TrigNode> Triggers;
    }

}

