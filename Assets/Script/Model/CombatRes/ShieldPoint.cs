using System;

namespace SkillEditorDemo.Model
{
    public class ShieldPoint : CombatRes
    {
        public ShieldPoint(Unit unit) : base(unit)
        {
        }
        public override StatType ValueType => StatType.SP;
        public override StatType MaxValueType => StatType.SPMax;
        public override StatType UpdateValueType => StatType.SPRecover;
        public override StatType UpdateDelayType => StatType.SPRecoverDelay;

        public override bool TrigAdd(int trigCount, int value, CombatCache cache, Unit from)
        {
            if (value <= 0) { return true; }
            cache[CombatCacheType.SPAdd] = value;
            if (!Unit.Trig(trigCount, TrigType.SPAdd.ed(), cache, from)) { return false; }
            int newValue = (int)(Value + cache[CombatCacheType.SPAdd]);
            int max = MaxValue;
            cache[CombatCacheType.OverSP] =MathF.Max(0, newValue - max);
            Value = (int)MathF.Min(max, newValue);
            if (!Unit.Trig(trigCount, Aft | TrigType.SPAdd.ed(), cache, from)) { return false; }
            return true;
        }

        public override bool TrigCost(int trigCount, int value, CombatCache cache, Unit from)
        {
            if (value <= 0 || Value <= 0) { return true; }
            Reset();
            cache[CombatCacheType.SPDmg] = MathF.Min(value, Value);
            cache[CombatCacheType.LeftDmg] = MathF.Max(value - Value, 0);
            if (!Unit.Trig(trigCount, TrigType.SPDmg.ed(), cache, from)) { return false; }
            Value = (int)MathF.Max(Value - (int)cache[CombatCacheType.SPDmg], 0);
            cache[CombatCacheType.SPDmg] = MathF.Min(cache[CombatCacheType.SPDmg], Value);
            if (!Unit.Trig(trigCount, Aft | TrigType.SPDmg.ed(), cache, from)) { return false; }
            if (Value == 0)
            {
                if (!Unit.Trig(trigCount, TrigType.SPBreak.ed(), cache, from)) { return false; }
            }
            return true;
        }
    }
}
