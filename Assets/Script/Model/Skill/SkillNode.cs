using SkillEditorDemo.Utility;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo.Model
{
    [NodeInfo(null, "技能", 400, "技能/技能"), AssetFilter(true, typeof(SkillAsset))]
    public class SkillNode : JsonNode, IUniqNode, IData<SkillNode>
    {
        [SerializeField, ShowInNode, LabelInfo("ID", 60)]
        public string ID { get; set; }
        [ShowInNode, LabelInfo("Name", 60)]
        public string Name { get; set; }
    }
}
