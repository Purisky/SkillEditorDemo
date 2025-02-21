using SkillEditorDemo.Model;
using System;
using System.Collections.Generic;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo.View
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
    public class UnitData : ScriptableObject
    {
        public float Radius;
        public List<StatData> Stats = new();
        public List<BuffInfo> Buffs = new();
        public List<SkillInfo> Skills = new();
        public Model.UnitData GenData()
        {
            Model.UnitData unitData = new()
            {
                ID = name,
                Radius = Radius,
                Buffs = Buffs,
                Skills = Skills
            };
            for (int i = 0; i < Stats.Count; i++)
            {
                StatType statType = (StatType)Enum.Parse(typeof(StatType), Stats[i].Type);
                if (statType != StatType.None)
                {
                    unitData.Stats.Add(statType, Stats[i].Value);
                }
            }
            return unitData;
        }


    }
}
