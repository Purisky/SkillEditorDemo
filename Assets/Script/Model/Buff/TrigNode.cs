using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;
namespace SkillEditorDemo.Model
{
    [NodeInfo(typeof(TrigNode), "触发器", 300, "Buff/触发器"), AssetFilter(true, typeof(BuffAsset)), PortColor("#7B68EE")]
    public class TrigNode : JsonNode , IGrowID<TrigNode>
    {
        [ShowInNode(ShowIf = nameof(ShowPassive)), LabelInfo("被", 20), Group("Trig", Width = 45)]
        public bool Passive;
        [ShowInNode, LabelInfo(Hide =true), Group("Trig")]
        public TrigType TrigType;
        [ShowInNode(ShowIf = nameof(ShowSeq)), LabelInfo(Hide = true), Group("Trig", Width = 40)]
        public TriggerSequence TriggerSequence;
        [Child, TitlePort]
        public Condition Condition;
        [Child(true), LabelInfo("效果组"), Group("Trig", Width = 70)]
        public List<ActionNode> Actions;
        [ ShowInNode, LabelInfo("冷却时间")]
        public TimeValue CD;
        [Child, LabelInfo("触发移除")]
        public FuncValue RemoveOnTrig;


        [JsonIgnore]
        public bool ShowPassive => !NoPassive.Contains(TrigType);
        [JsonIgnore]
        public bool ShowSeq => !NoSeq.Contains(TrigType);
        [JsonIgnore]
        public int GrowID { get; set; }

        public static readonly HashSet<TrigType> NoPassive = EnumAttributeGetter.Get<TrigType, NoPassiveAttribute>().Keys.ToHashSet();
        public static readonly HashSet<TrigType> NoSeq = EnumAttributeGetter.Get<TrigType, NoSeqAttribute>().Keys.ToHashSet();
        public bool CheckCondition(ObjInfo  info,CombatCache cache ) => Condition == null || Condition.GetResult(info, cache);
        [JsonIgnore]
        public TrigType CombinedTrigType
        {
            get {
                TrigType type = TrigType;
                if (Passive) { type = type.ed(); }
                if (TriggerSequence == TriggerSequence.Aft)
                { 
                    type |= STrigType.Aft;
                }
                return type;
            }
        }
    }
}