using Newtonsoft.Json;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    [PortColor("#FC9DE1")]
    public abstract class BaseAction : JsonNode
    {

    }
    [NodeInfo(typeof(BaseAction), "分支执行", 100, "执行/分支执行")]
    public class ConditionAction : BaseAction
    {
        [Child(true), TitlePort]
        public Condition Condition;
        [Child(true), LabelInfo(Text = "真", Width = 10)]
        public BaseAction True;
        [Child(true), LabelInfo(Text = "假", Width = 10)]
        public BaseAction False;
    }



    [NodeInfo(typeof(BaseAction), "实体属性修改", 200, "执行/实体属性修改")]
    public class StatModify : BaseAction
    {
        [Child(true), TitlePort]
        public Entity Entity;

        [ShowInNode, LabelInfo(Hide = true), Group("Stat")]
        public StatType StatType;
        [ShowInNode(ShowIf = nameof(isStat)), LabelInfo(Hide = true), Group("Stat")]
        public StatModType StatModType;
        [ShowInNode(ShowIf = nameof(isRes)), LabelInfo(Hide = true), Group("Stat")]
        public ValueModType ResModType;
        [Child, LabelInfo(Hide = true)]
        public NumValue Value;

        [JsonIgnore]
        bool isStat => StatType != StatType.None && StatType.IsStat();
        [JsonIgnore]
        bool isRes => StatType != StatType.None && !StatType.IsStat();



    }
    [NodeInfo(typeof(BaseAction), "伤害", 200, "执行/伤害")]
    public class Damage : BaseAction
    {
        [Child(true), TitlePort]
        public Entity Entity;
        [ShowInNode, LabelInfo(Hide = true)]
        public DmgType DmgType;
        [ShowInNode, LabelInfo("直接"), Group("Type")]
        public bool Direct;
        [ShowInNode, LabelInfo("闪避"), Group("Type")]
        public bool Dodge;
        [ShowInNode, LabelInfo("暴击"), Group("Type")]
        public bool Crit;
        [Child, LabelInfo(Hide = true)]
        public NumValue Value;
    }
    [NodeInfo(typeof(BaseAction), "存储临时数据", 160, "执行/存储临时数据"), AssetFilter(true, typeof(BuffAsset))]
    public class TempData : BaseAction
    {
        [ShowInNode, LabelInfo("Key", 30)]
        public string Key;
        [ShowInNode, LabelInfo(Hide = true), Group("Value", Width = 50)]
        public ValueModType ResModType;
        [Child, LabelInfo(Hide = true), Group("Value")]
        public NumValue Value;
    }
    [NodeInfo(typeof(BaseAction), "添加Buff", 160, "执行/添加Buff")]
    public class AddBuff : BaseAction
    {
        [Child(true), TitlePort]
        public Entity Entity;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        public string ID;



        static DropdownList<string> Buffs => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns;

    }




}
