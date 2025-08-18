using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [NodeInfo(typeof(Condition), "单位存在", 100, "条件/单位存在")]
    [Prompt(@"单位存在判断节点,判断输入的单位列表是否存在至少一个单位")]
    public partial class UnitAny : Condition
    {
        [Child(true), TitlePort]
        [Prompt(@"单位列表")]
        public List<UnitNode> UnitList;

        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            if (UnitList == null || UnitList.Count == 0) { return false; }
            for (int i = 0; i < UnitList.Count; i++)
            {
                if (UnitList[i].GetUnits(info, cache).Any()) { return true; }
            }
            return false;
        }

        public override string GetText(int indent = 0)
        {
            if (UnitList == null || UnitList.Count == 0)
            {
                return "真";
            }
            string unitText = string.Join(" | ", UnitList.Select(n => n.GetText(0)));
            return $"存在任意 ({unitText}) 单位";
        }
    }

    [NodeInfo(typeof(Condition), "Buff检测", 120, "条件/Buff检测")]
    [Prompt(@"Buff检测节点,判断单位是否存在指定ID的Buff")]
    public partial class BuffExist : Condition
    {
        [Child(true), TitlePort]
        [Prompt(@"检测的单位")]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo("同源", 60)]
        [Prompt(@"额外的判断条件,同时检测该Buff是否与本节点的源相同")]
        public bool SameSource;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        [Prompt(@"Buff的ID")]
        public string ID;
        static DropdownList<string> Buffs => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns;

        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            if (UnitNode == null) { return false; }
            List<Unit> units = UnitNode.GetUnits(info, cache);
            if (!units.Any()) { return false; }
            Unit unit = units[0];
            //Todo
            return false;

        }

        public override string GetText(int indent = 0)
        {
            string unit = UnitNode?.GetText(0) ?? "目标";
            string buffId = string.IsNullOrEmpty(ID) ? "指定Buff" : ID;
            string sourcePrefix = SameSource ? "同源的 " : "";
            return $"{unit} 存在 {sourcePrefix}{buffId}";
        }
    }
    [NodeInfo(typeof(Condition), "伤害检测", 120, "条件/伤害检测"), AssetFilter(true, typeof(BuffAsset))]
    [Prompt(@"伤害检测节点,判断伤害的类型,这个节点仅适用于能够产生战斗缓存的触发器类型之后,如Dodge/Heal/Dmg/SPDmg等")]
    public partial class DmgCheck : Condition
    {
        [ShowInNode, LabelInfo(Hide = true)]
        [Prompt(@"伤害类型,设为任意(Any)则跳过该检测")]
        public DmgType Type;
        [ShowInNode, LabelInfo(Hide = true), Group("Type_")]
        [Prompt(@"伤害是否为主动伤害,设为任意(Any)则跳过该检测")]
        public DmgDirectType DirectType;
        [ShowInNode, LabelInfo(Hide = true), Group("Type_")]
        [Prompt(@"伤害是否为暴击伤害,设为任意(Any)则跳过该检测")]
        public CritType CritType;

        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            if ((Type != DmgType.Any) && cache.DmgType != Type) { return false; }
            if (DirectType != DmgDirectType.Any && (DirectType == DmgDirectType.Direct) != cache.DirectDmg) { return false; }
            if (CritType != CritType.Any && (CritType == CritType.Crit) != cache.IsCrit) { return false; }
            return true;
        }

        public override string GetText(int indent = 0)
        {
            List<string> conditions = ListPool<string>.GetList();
            
            if (Type != DmgType.Any)
            {
                string dmgTypeName = Type switch
                {
                    DmgType.Physic => "物理",
                    DmgType.Fire => "火焰", 
                    DmgType.Frost => "冰霜",
                    DmgType.Lightning => "闪电",
                    _ => Type.ToString()
                };
                conditions.Add($"{dmgTypeName}伤害");
            }
            
            if (DirectType != DmgDirectType.Any)
            {
                conditions.Add(DirectType == DmgDirectType.Direct ? "直接伤害" : "间接伤害");
            }
            
            if (CritType != CritType.Any)
            {
                conditions.Add(CritType == CritType.Crit ? "暴击伤害" : "非暴击伤害");
            }
            
            string result;
            if (conditions.Count == 0)
            {
                result = "真";
            }
            else if (conditions.Count == 1)
            {
                result = conditions[0];
            }
            else
            {
                result = $"(伤害为 {string.Join(" | ", conditions)})";
            }
            
            conditions.Release();
            return result;
        }
    }



}
