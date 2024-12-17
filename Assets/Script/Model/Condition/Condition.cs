using System;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    [NodeInfo(typeof(Condition), "单位存在", 100, "条件/单位存在")]
    public class UnitAny : Condition
    {
        [Child(true), TitlePort]
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
    public class BuffExist : Condition
    {
        [Child(true), TitlePort]
        public UnitNode UnitNode;
        [ShowInNode, LabelInfo("同源", 60)]
        public bool SameSource;
        [ShowInNode, LabelInfo(Hide = true), Dropdown(nameof(Buffs))]
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
    [NodeInfo(typeof(Condition), "伤害检测", 120, "条件/伤害检测")]
    public class DmgCheck : Condition
    {
        [ShowInNode, LabelInfo(Hide = true)]
        public DmgType Type;
        [ShowInNode, LabelInfo(Hide = true), Group("Type_")]
        public DmgDirectType DirectType;
        [ShowInNode, LabelInfo(Hide = true), Group("Type_")]
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
