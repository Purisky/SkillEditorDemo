using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using TreeNode.Runtime;

namespace SkillEditorDemo.Model
{
    public partial class Stat : IIndex<Stat>
    {
        public int Index { get; set; }
        public readonly StatType StatType;
        public readonly StatHandler StatHandler;
        public Unit Carrier=> StatHandler.Unit;
        public ModifyValue BaseValue;
        decimal ValueCache;
        int CacheTick;
        List<ActionCache>[] ValueList = new List<ActionCache>[3];
        public float Value => UpdateValue();

        public float ValueWithParent
        {
            get {
                float value = Value;
                if (ParentType == StatType.None)
                {
                    return value;
                }
                float parentValue = StatHandler.GetStat(ParentType, true).ValueWithParent;
                return MultParent ? value * parentValue : value + parentValue;
            }
        }

        public StatType ParentType;
        public bool MultParent;
        public static float GetDefault(StatType type) => DefaultDic.GetValueOrDefault(type, 0);
        public static Stat Get(int id) => IIndex<Stat>.Get(id);
        static readonly Dictionary<StatType, float> DefaultDic = GetDefault();
        static readonly Dictionary<StatType, (StatType type, bool mult)> ParentDic = GetParent();
        static Dictionary<StatType, float> GetDefault()
        {
            Dictionary<StatType, float> dic = new();
            foreach (var item in EnumAttributeGetter.Get<StatType, DefaultValueAttribute>())
            {
                dic.Add(item.Key, item.Value.Value);
            }
            return dic;
        }
        static Dictionary<StatType, (StatType,bool)> GetParent()
        {
            Dictionary<StatType, (StatType type, bool mult)> dic = new();
            foreach (var item in EnumAttributeGetter.Get<StatType, StatParentAttribute>())
            {

                if (item.Key.IsStat() && item.Value.Type.IsStat())
                {
                    StatType currentParent = item.Value.Type;
                    while (true)
                    {
                        if (currentParent == item.Key)
                        {
                            throw new Exception("父属性设置形成回环:" + item.Key);
                        }
                        if (dic.ContainsKey(currentParent))
                        {
                            currentParent = dic[currentParent].type;
                        }
                        else
                        {
                            break;
                        }
                    }
                    dic.Add(item.Key, (item.Value.Type, item.Value.Mult));
                }
            }
            return dic;
        }
        public Stat(float baseValue, StatType type, StatHandler manager)
        {
            IIndex<Stat>.Add(this);
            StatType = type;
            ValueCache = (decimal)baseValue;
            StatHandler = manager;
            (ParentType, MultParent) = ParentDic.GetValueOrDefault(StatType, (StatType.None, false));
        }
        public void SetBase(ModifyValue value)
        {
            BaseValue = value;
            ValueCache = BaseValue.Value;
        }
        public void AddBase(ModifyValue value)
        {
            BaseValue += value;
            ValueCache = BaseValue.Value;
        }

        public float UpdateValue(bool ignoreTick = false)
        {
            if (Time.Tick > CacheTick || ignoreTick)
            {
                CacheTick = Time.Tick;
                ValueCache = BaseValue.Base;
                List<ActionCache> List;
                for (int j = 0; j < 3; j++)
                {
                    List = ValueList[j];
                    HandleValue(j, List);
                }
            }
            return (float)ValueCache;
        }
        private void HandleValue(int j, List<ActionCache> List)
        {
            if (List != null && List.Count > 0)
            {
                decimal oldValue = ValueCache;
                List<ActionCache > delete = new();
                decimal multValue = 1 + BaseValue.Mult;
                //Debug.Log($"Current:{Current}",false);
                for (int i = 0; i < List.Count; i++)
                {
                    BuffTrig trig = BuffTrig.Get(List[i].TrigIndex);
                    if (trig != null && trig.Buff != null && trig.Buff.Alive)
                    {
                        decimal value = (decimal)(trig.GetValue(List[i].ActionIndex));
                        switch ((StatModType)j)
                        {
                            case StatModType.BaseAdd:
                            case StatModType.FinalAdd:
                                ValueCache += value;
                                break;
                            case StatModType.Multiply:
                                multValue += value;
                                break;
                        }
                    }
                    else
                    {
                        delete.Add(List[i]);
                    }
                }
                if ((StatModType)j == StatModType.Multiply)
                {
                    ValueCache *= Math.Max(multValue, 0);
                }
                for (int i = 0; i < delete.Count; i++)
                {
                    List.Remove(delete[i]);
                }
            }
        }


        public void Add(ActionCache action)
        {
            StatModify statMod = ActionNode.Get<StatModify>(action.ActionIndex);
            int index = (int)statMod.StatModType;
            ValueList[index] ??= new List<ActionCache>();
            BuffTrig trig = BuffTrig.Get(action.TrigIndex);
            ValueList[index].Add(new(trig.Index, action.ActionIndex));
            //JSON.Log(ValueList);
        }
        public void Dispose()
        {
            IIndex<Stat>.Remove(Index);
        }
    }

    public readonly struct ActionCache
    { 
        public readonly int TrigIndex;
        public readonly int ActionIndex;

        public ActionCache(int trigIndex, int actionIndex)
        {
            TrigIndex = trigIndex;
            ActionIndex = actionIndex;
        }

    }

}
