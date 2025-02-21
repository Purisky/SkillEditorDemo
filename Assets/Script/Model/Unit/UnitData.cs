using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;

namespace SkillEditorDemo.Model
{
    public class UnitData : IData<UnitData>
    {
        public string ID{ get; set; }
        public float Radius;
        public List<BuffInfo> Buffs;
        public List<SkillInfo> Skills;
        public Dictionary<StatType,float> Stats = new();
    }
    [Serializable]
    public struct BuffInfo
    {
        public string ID;
        public int Level;
        public int Degree;
        public float Param;
    }
    [Serializable]
    public struct SkillInfo
    {
        public string ID;
        public int Level;
    }
    [Serializable]
    public struct StatData
    {
        public string Type;
        public float Value;
    }
}
