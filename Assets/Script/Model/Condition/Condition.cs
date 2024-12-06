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
        public override string GetText() => $"(Dmg=>{Type.GetLabel()},{DirectType.GetLabel()},{CritType.GetLabel()})";
    }



}
