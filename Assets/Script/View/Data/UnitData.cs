using SkillEditorDemo.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillEditorDemo.View
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
    public class UnitData : ScriptableObject
    {
        public float Radius;
        public List<StatData> Stats = new();
        public Model.UnitData GenData()
        {
            Model.UnitData unitData = new()
            {
                ID = name,
                Radius = Radius,
            };
            for (int i = 0; i < Stats.Count; i++)
            {
                unitData.Stats[Stats[i].Type] = Stats[i].Value;
            }
            return unitData;
        }

        [Serializable]
        public struct StatData
        {
            public StatType Type;
            public float Value;
        }
    }
}
