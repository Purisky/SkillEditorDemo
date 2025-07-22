using Newtonsoft.Json;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;
namespace SkillEditorDemo.Model
{
    [NodeInfo(typeof(TrigNode), "触发器", 300, "Buff/触发器"), AssetFilter(true, typeof(BuffAsset)), PortColor("#7B68EE")]
    [Prompt(@"触发器主要节点,用于描述一个触发器的基本信息,包括触发条件和触发效果,触发器仅由Buff携带者触发效果")]
    public class TrigNode : JsonNode , IGrowID<TrigNode>
    {
        [ShowInNode(ShowIf = nameof(ShowPassive)), LabelInfo("被", 20), Group("Trig", Width = 45)]
        [Prompt(@"决定出触发条件是否为被动,在定义该字段时需严格判断所涉及的场景是否为BUff的携带者主动发起的还是被动承受的")]
        public bool Passive;
        [ShowInNode, LabelInfo(Hide =true), Group("Trig")]
        [Prompt(@"触发器的类型,决定触发器的触发方式和触发时机")]
        public TrigType TrigType;
        [ShowInNode(ShowIf = nameof(ShowSeq)), LabelInfo(Hide = true), Group("Trig", Width = 40)]
        [Prompt(@"触发器的触发顺序,当一个触发行为存在完成状态时,用于区分在完成前还是完成后触发效果 以下是其在代码中的执行顺序:[On]->Event->[Aft]
完成前触发可用于修改或者打断事件,完成后触发则可以获得事件执行后的完整数据")]
        public TriggerSequence TriggerSequence;
        [Child, TitlePort]
        [Prompt(@"触发过滤器,当不为空时对触发状态进行过滤,如造成伤害时,过滤伤害类型/值,返回否会跳过触发")]
        public Condition Condition;
        [Child(true), LabelInfo("效果组"), Group("Trig", Width = 70)]
        [Prompt(@"触发器的效果组,按数组顺序依次触发")]
        public List<ActionNode> Actions;
        [ShowInNode, LabelInfo("冷却时间")]
        [Prompt(@"触发器的冷却时间,在冷却时间内不会再次触发,当成功执行效果组后才会进入冷却")]
        public TimeValue CD;
        [Child, LabelInfo("触发移除")]
        [Prompt(@"当触发器被成功触发时,移除的层数,-1为直接移除Buff,0则代表无效果,>0代表对应层数")]
        public FuncValue RemoveOnTrig;


        [JsonIgnore]
        public bool ShowPassive => !NoPassive.Contains(TrigType);
        [JsonIgnore]
        public bool ShowSeq => !NoSeq.Contains(TrigType);
        [JsonIgnore]
        public int GrowID { get; set; }

        public static readonly HashSet<TrigType> NoPassive = EnumAttributeGetter.Get<TrigType, NoPassiveAttribute>().Keys.ToHashSet();
        public static readonly HashSet<TrigType> NoSeq = EnumAttributeGetter.Get<TrigType, NoSeqAttribute>().Keys.ToHashSet();
        public bool CheckCondition(TrigInfo  info,CombatCache cache ) => Condition == null || Condition.GetResult(info, cache);
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