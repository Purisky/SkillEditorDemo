using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    [NodeInfo(typeof(Condition), "单位存在", 100, "条件/单位存在")]
    [RagDoc(@"单位存在判断节点,判断输入的单位列表是否存在至少一个单位")]
    public class UnitAny : Condition
    {
        [Child(true), TitlePort]
        [RagDoc(@"单位列表")]
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

        public override string GetText()
        {
            string unitText = "";
            if (UnitList != null)
            {
                unitText = string.Join(",", UnitList.Select(n => n.GetText()));
            }
            return $"([{unitText}].存在)";
        }
    }

    [NodeInfo(typeof(Condition), "Buff检测", 120, "条件/Buff检测")]
    [RagDoc(@"Buff检测节点,判断单位是否存在指定ID的Buff")]
    public class BuffExist : Condition
    {
        [Child(true), TitlePort]
        [RagDoc(@"检测的单位")]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo("同源", 60)]
        [RagDoc(@"额外的判断条件,同时检测该Buff是否与本节点的源相同")]
        public bool SameSource;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
        [RagDoc(@"Buff的ID")]
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

        public override string GetText()=> $"{UnitNode?.GetText() ?? "Null"}.ExistBuff({ID})";
    }
    [NodeInfo(typeof(Condition), "伤害检测", 120, "条件/伤害检测"), AssetFilter(true, typeof(BuffAsset))]
    [RagDoc(@"伤害检测节点,判断伤害的类型,这个节点仅适用于能够产生战斗缓存的触发器类型之后,如Dodge/Heal/Dmg/SPDmg等")]
    public class DmgCheck : Condition
    {
        [ShowInNode, LabelInfo(Hide = true)]
        [RagDoc(@"伤害类型,设为任意(Any)则跳过该检测")]
        public DmgType Type;
        [ShowInNode, LabelInfo(Hide = true), Group("Type_")]
        [RagDoc(@"伤害是否为主动伤害,设为任意(Any)则跳过该检测")]
        public DmgDirectType DirectType;
        [ShowInNode, LabelInfo(Hide = true), Group("Type_")]
        [RagDoc(@"伤害是否为暴击伤害,设为任意(Any)则跳过该检测")]
        public CritType CritType;

        public override bool GetResult(TrigInfo info, CombatCache cache)
        {
            if ((Type != DmgType.Any) && cache.DmgType != Type) { return false; }
            if (DirectType != DmgDirectType.Any && (DirectType == DmgDirectType.Direct) != cache.DirectDmg) { return false; }
            if (CritType != CritType.Any && (CritType == CritType.Crit) != cache.IsCrit) { return false; }
            return true;
        }

        public override string GetText() => $"(Dmg=>{Type.GetLabel()},{DirectType.GetLabel()},{CritType.GetLabel()})";
    }



}
