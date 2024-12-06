using System;
using System.Collections.Generic;
using TreeNode.Runtime;

namespace SkillEditorDemo
{
    public partial class Stat : IIndex<Stat>
    {
        public int Index { get; set; }
        public readonly StatType StatType;
        public readonly StatManager Manager;
        public IStatCarrier Carrier=> Manager.Carrier;
        public ModifyValue BaseValue;
        decimal ValueCache;
        int CacheFrame;
        List<ActionCache>[] ValueList;
        public float Value => UpdateValue();
        public StatType? ParentType => ParentDic.GetValueOrDefault(StatType, StatType.None);
        public static float GetDefault(StatType type) => DefaultDic.GetValueOrDefault(type, 0);
        public static Stat Get(int id) => IIndex<Stat>.Get(id);
        static readonly Dictionary<StatType, float> DefaultDic = GetDefault();
        static readonly Dictionary<StatType, StatType> ParentDic = GetParent();
        static Dictionary<StatType, float> GetDefault()
        {
            Dictionary<StatType, float> dic = new();
            foreach (var item in EnumAttributeGetter.Get<StatType, DefaultValueAttribute>())
            {
                dic.Add(item.Key, item.Value.Value);
            }
            return dic;
        }

        static Dictionary<StatType, StatType> GetParent()
        {
            Dictionary<StatType, StatType> dic = new();
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
                            currentParent = dic[currentParent];
                        }
                        else
                        {
                            break;
                        }
                    }
                    dic.Add(item.Key, item.Value.Type);
                }
            }
            return dic;
        }
        public Stat(float baseValue, StatType type, StatManager manager)
        {
            IIndex<Stat>.Add(this);
            StatType = type;
            ValueCache = (decimal)baseValue;
            Manager = manager;
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

        public float UpdateValue(bool ignoreFrame = false)
        {
            if (Time.CurrentFrame > CacheFrame || ignoreFrame)
            {
                CacheFrame = Time.CurrentFrame;
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
                decimal oldvalue = ValueCache;
                List<ActionCache > delete = new();
                decimal multValue = 1 + BaseValue.Mult;
                //Debug.Log($"Current:{Current}",false);
                for (int i = 0; i < List.Count; i++)
                {
                    BuffTrig trig = BuffTrig.Get(List[i].TrigIndex);
                    if (trig != null && trig.Buff != null && trig.Buff.Alive)
                    {
                        decimal value =(decimal) (
                                       //trig.IsActionRefValue(List[i].Item1.Item2) ? trig.GetValue(List[i].Item1.Item2) :
                            List[i].Value);

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
            if (ValueList[index] == null)
            {
                ValueList[index] = new List<ActionCache>();
            }
            BuffTrig trig = BuffTrig.Get(action.TrigIndex);
            ValueList[index].Add(new(trig.Index, action.ActionIndex,
                //statMod.MainValue != null ? 0 : 
                trig.GetValue(action.ActionIndex)));
            //JSON.Log(ValueList);
        }
        public void Dispose()
        {
            IIndex<Stat>.Remove(Index);
        }
    }

    public struct ActionCache
    { 
        public readonly int TrigIndex;
        public readonly int ActionIndex;

        public float Value;
        public ActionCache(int trigIndex, int actionIndex,float value = 0)
        {
            TrigIndex = trigIndex;
            ActionIndex = actionIndex;
            Value = value;
        }

    }

}
