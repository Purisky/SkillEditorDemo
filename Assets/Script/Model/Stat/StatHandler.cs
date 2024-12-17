using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Properties;
using UnityEngine;

namespace SkillEditorDemo
{

    public class StatHandler
    {
        public readonly Unit Unit;
        readonly Dictionary<StatType, Stat> StatDic;
        public readonly (int value, bool inited)[] Values;
        public const TrigType Aft = STrigType.Aft;
        public StatHandler(Unit unit)
        {
            Unit = unit;
            StatDic = new();
            Values = new (int, bool)[(int)StatClass.ResCount];
        }
        public Stat AddStat(StatType type, float baseValue)
        {
            Stat stat = new(baseValue, type, this);
            StatDic.Add(type, stat);
            return stat;
        }
        public Stat GetStat(StatType type, bool add = false)
        {
            if (StatDic.TryGetValue(type, out Stat stat))
            { 
                return stat;
            }
            if (add)
            {
                stat = AddStat(type, Stat.GetDefault(type));
            }
            return stat;
        }

        public float this[StatType type]
        {
            get
            {
                if (type.IsStat())
                {
                    return GetStat(type)?.Value?? Stat.GetDefault(type);
                }
                else
                {
                    return Values[(int)type.GetStatClass()].value;
                }
            }
            set
            {
                SetValue(type, value);
            }
        }
        public void SetValue(StatType type, float value,ValueModType modType = ValueModType.Set)
        {
            int index = (int)type.GetStatClass();
            switch (modType)
            {
                case ValueModType.Set:
                    Values[index] = ((int)value, true);
                    break;
                case ValueModType.Add:
                    Values[index] = ((int)value + Values[index].value, true);
                    break;
                case ValueModType.Multiply:
                    Values[index] = ((int)(value * Values[index].value), true);
                    break;
            }
        }
        public float GetDmgMod(DmgType dmgType, bool ed = false) => GetStat(dmgType.StatType(ed), true).ValueWithParent;
        public void Add(StatType type, ActionCache actionCache)
        {
            Stat stat = GetStat(type, true);
            stat!.Add(actionCache);
            stat.UpdateValue(true);
        }
        public void Dispose()
        {
            foreach (var item in StatDic)
            {
                item.Value.Dispose();
            }
        }

    }
}
