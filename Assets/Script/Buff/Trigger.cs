using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using UnityEngine;
namespace SkillEditorDemo
{
    [NodeInfo(typeof(Trigger), "触发器", 300, "Buff/触发器"), AssetFilter(true, typeof(BuffAsset)), PortColor("#7B68EE")]
    public class Trigger : JsonNode
    {
        [ShowInNode(ShowIf = nameof(ShowPassive)), LabelInfo("被", 20), Group("Trig", Width = 45)]
        public bool Passive;
        [ShowInNode, LabelInfo(Hide =true), Group("Trig")]
        public TrigType TrigType;
        [ShowInNode(ShowIf = nameof(ShowSeq)), LabelInfo(Hide = true), Group("Trig", Width = 40)]
        public TriggerSequence TriggerSequence;
        [Child, LabelInfo("过滤条件"),TitlePort]
        public Condition Condition;
        [Child, LabelInfo("效果组"), Group("Trig", Width = 70)]
        public List<BaseAction> Actions;
        [Child, LabelInfo("冷却时间")]
        public NumValue CD;
        [Child, LabelInfo("触发移除")]
        public NumValue RemoveOnTrig;


        [JsonIgnore]
        public bool ShowPassive => !NoPassive.Contains(TrigType);
        [JsonIgnore]
        public bool ShowSeq => !NoSeq.Contains(TrigType);
        public static readonly HashSet<TrigType> NoPassive = EnumAttributeGetter.Get<TrigType, NoPassiveAttribute>().Keys.ToHashSet();
        public static readonly HashSet<TrigType> NoSeq = EnumAttributeGetter.Get<TrigType, NoSeqAttribute>().Keys.ToHashSet();
    }
}