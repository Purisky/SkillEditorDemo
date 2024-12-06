using UnityEngine;

namespace SkillEditorDemo
{
    public class Unit : IIndexData<Unit, UnitData>
    {
        public int Index { get; set; }
        public string ID { get; set; }
    }
}
