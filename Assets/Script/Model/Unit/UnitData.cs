using SkillEditorDemo.Utility;
using System.Collections.Generic;

namespace SkillEditorDemo.Model
{
    public class UnitData : IData<UnitData>
    {
        public string ID{ get; set; }
        public float Radius;
        public Dictionary<StatType,float> Stats = new();
    }



}
