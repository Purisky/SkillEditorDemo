using System.Collections.Generic;
using System.Threading;
using Unity.Properties;
using UnityEngine;

namespace SkillEditorDemo
{
    public interface IStatCarrier
    {
        StatManager StatManager { get; set; }
    }


    public class StatManager
    {
        public readonly IStatCarrier Carrier;
        readonly Dictionary<StatType, Stat> StatDic;
        public readonly (int value, bool)[] Values;




        public StatManager(IStatCarrier carrier)
        {
            Carrier = carrier;
            StatDic = new();
            Values = new (int, bool)[(int)StatClass.ResCount];
        }

        public Stat AddStat(StatType type, float baseValue)
        {
            Stat property = new (baseValue, type, this);
            StatDic.Add(type, property);
            return property;
        }
        public float this[StatType type]
        {
            get
            {
                if (type.IsStat())
                {
                    return StatDic.GetValueOrDefault(type, null)?.Value?? Stat.GetDefault(type);
                }
                else
                {
                    return Values[(int)type.GetStatClass()].value;
                }
            }
            set
            {
                Values[(int)type.GetStatClass()] = ((int)value,true);
            }
        }
    }

}
