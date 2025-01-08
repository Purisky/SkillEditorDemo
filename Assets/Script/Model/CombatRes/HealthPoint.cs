using UnityEngine;

namespace SkillEditorDemo.Model
{
    public class HealthPoint : CombatRes
    {
        public HealthPoint(Unit unit) : base(unit)
        {
        }
        public override StatType ValueType =>StatType.HP;
        public override StatType MaxValueType => StatType.MaxHP;
        public override StatType UpdateValueType =>  StatType.HPRecover;

        public override bool TrigAdd(int trigCount, int value, CombatCache cache, Unit from)
        {
            if (value <= 0) { return true; }
            cache[CombatCacheType.TotalHeal] = value;
            if (!Unit.Trig(trigCount, TrigType.Heal.ed(), cache, from)) { return false; }
            int newValue = (int)(Value + cache[CombatCacheType.TotalHeal]);
            int max = MaxValue;
            if (newValue > max)
            {
                cache[CombatCacheType.OverHeal] = newValue - max;
                newValue = max;
            }
            cache[CombatCacheType.RealHeal] = max - Value;
            Value = newValue;
            if (!Unit.Trig(trigCount, Aft | TrigType.Heal.ed(), cache, from)) { return false; }
            return true;
        }

        public override bool TrigCost(int trigCount, int value, CombatCache cache, Unit from)
        {
            if (value <= 0) { return true; }
            cache[CombatCacheType.LifeDmg] = Mathf.Min(value, Value);
            if(!Unit.Trig(trigCount, TrigType.HPDmg.ed(), cache, from)) { return false; }
            int newValue = Value - (int)cache[CombatCacheType.LifeDmg];
            if (newValue <= 0)
            {
                cache[CombatCacheType.OverDmg] = -newValue;
            }
            Value = Mathf.Max(newValue, 0);
            if(!Unit.Trig(trigCount, Aft | TrigType.HPDmg.ed(), cache, from)) { return false; }
            return true;
        }
    }
}
