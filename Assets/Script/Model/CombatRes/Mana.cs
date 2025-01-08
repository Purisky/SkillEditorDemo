using System;

namespace SkillEditorDemo.Model
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
            if (value <= 0) { return true; }
            cache[CombatCacheType.ManaAdd] = value;
            if (!Unit.Trig(trigCount, TrigType.ManaAdd.ed(), cache, from)) { return false; }
            int newValue = (int)(Value + cache[CombatCacheType.ManaAdd]);
            int max = MaxValue;
            cache[CombatCacheType.OverMana] = MathF.Max(0, newValue - max);
            Value = (int)MathF.Min(max, newValue);
            if (!Unit.Trig(trigCount, Aft | TrigType.ManaAdd.ed(), cache, from)) { return false; }
            return true;
        }

        public override bool TrigCost(int trigCount, int value, CombatCache cache, Unit from)
        {
            if (value <= 0) { return true; }
            cache[CombatCacheType.ManaCost] = value;
            if (!Unit.Trig(trigCount, TrigType.ManaCost.ed(), cache, from)) { return false; }//may modify cost
            if (cache[CombatCacheType.ManaCost] > Value) { return false; }
            Value -= (int)cache[CombatCacheType.ManaCost];
            if (!Unit.Trig(trigCount, TrigType.ManaCost.ed(), cache, from)) { return false; }
            return true;
        }
    }
}
