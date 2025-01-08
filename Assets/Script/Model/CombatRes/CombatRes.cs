using UnityEngine;

namespace SkillEditorDemo.Model
{
    public abstract class CombatRes
    {
        protected const TrigType Aft = STrigType.Aft;
        protected Unit Unit;
        public abstract StatType ValueType { get; }
        public abstract StatType MaxValueType { get; }
        public abstract StatType UpdateValueType { get; }
        public virtual StatType UpdateDelayType { get => StatType.None; }
        protected int updateTick;
        protected Stat maxStat;
        protected Stat updateValueStat;
        protected Stat updateDelayStat;
        protected readonly int valueIndex;
        public int MaxValue => (int)maxStat.Value;
        public int UpdateValue => (int)updateValueStat.Value;
        public int UpdateDelay => (int)(updateDelayStat?.Value??0);
        public ref int Value => ref Unit.StatHandler.Values[valueIndex].value;
        public CombatRes(Unit unit)
        {
            Unit = unit;
            valueIndex = (int)ValueType.GetStatClass();
            maxStat = Unit.StatHandler.GetStat(MaxValueType, true);
            updateValueStat = Unit.StatHandler.GetStat(UpdateValueType, true);
            updateDelayStat = UpdateDelayType == StatType.None ? null : Unit.StatHandler.GetStat(UpdateDelayType, true);
        }
        public virtual void Update()
        {
            updateTick++;
            int delay = UpdateDelay;
            if (updateTick >= Time.GameTickPerSec + delay)
            {
                updateTick = delay;
                Unit.StatHandler[ValueType] = Mathf.Min(Value+ UpdateValue, MaxValue);
            }
        }
        public void Reset() => updateTick = 0;
        public abstract bool TrigAdd(int trigCount, int value, CombatCache cache, Unit from);
        public abstract bool TrigCost(int trigCount, int value, CombatCache cache, Unit from);

    }
}
