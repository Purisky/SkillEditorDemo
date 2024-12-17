using UnityEngine;

namespace SkillEditorDemo
{
    public class Mana : CombatRes
    {
        public Mana(Unit unit) : base(unit)
        {
        }
        public override StatType ValueType =>  StatType.Mana;
        public override StatType MaxValueType =>  StatType.ManaMax;
        public override StatType UpdateValueType => StatType.ManaRecover;

        public override bool TrigAdd(int trigCount, int value, CombatCache cache, Unit from)
        {
            throw new System.NotImplementedException();
        }

        public override bool TrigCost(int trigCount, int value, CombatCache cache, Unit from)
        {
            throw new System.NotImplementedException();
        }
    }
}
