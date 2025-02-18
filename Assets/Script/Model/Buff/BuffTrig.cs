using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Collections.Generic;

namespace SkillEditorDemo.Model
{
    public class BuffTrig : IIndex<BuffTrig>,IGrowID
    {
        public int Index { get ; set ; }
        public int GrowID { get; set; }

        public Buff Buff;
        public int LastTrigTick = int.MinValue;
        public List<ActionNode> Actions=>TrigNode.Actions;
        TrigInfo TrigInfo;
        public CombatCache LastCombatCache;
        public int TrigTickDelay;
        public int CurrentTrigCount;

        public static BuffTrig Get(int id) => IIndex<BuffTrig>.Get(id);
        public TrigNode TrigNode => IGrowID<TrigNode>.Get(GrowID);
        public bool ExistNextPeriodicTrig => Buff.TotalTick < 0|| (Buff.TotalTick + Buff.FreshTick >= TrigTickDelay + Time.Tick);

        public BuffTrig(TrigNode node, Buff buff)
        {
            Index = IIndex<BuffTrig>.Add(this);
            GrowID = node.GrowID;
            Buff = buff;
            TrigInfo = buff.TrigInfo;
            
            TrigInfo.TrigID = Index;
            TrigNode data = TrigNode;
            if (data.TrigType is >= TrigType.ByTime_1 and <= TrigType.ByTime_4)
            {
                TrigTickDelay = (data.TrigType switch
                {
                    TrigType.ByTime_1 => 60,
                    TrigType.ByTime_2 => 30,
                    TrigType.ByTime_3 => 20,
                    TrigType.ByTime_4 => 15,
                    _ => 60,
                });
            }
        }



        public float GetValue(int actionId)
        {
            return ActionNode.Get<StatModify>(actionId).GetRuntimeValue(TrigInfo, LastCombatCache);
        }
        public bool Trig(int trigCount, CombatCache cache, EcsPackedEntity from)
        {
            LastCombatCache = cache;
            TrigInfo.TriggerID = from;
            if (TrigNode.CheckCondition(TrigInfo, cache) && CDPass())
            {
                return internalTrig(trigCount, TrigInfo, cache);
            }
            return true;
        }


        bool internalTrig(int trigCount, TrigInfo info,CombatCache cache)
        {
            bool trigContinue = true;
            for (int i = 0; i < Actions.Count; i++)
            {
                trigContinue &= Actions[i].Handle(trigCount, info, cache);
            }
            int removeOnTrig = (int)TrigNode.RemoveOnTrig.GetResult(info, cache);
            if (removeOnTrig != 0)
            {
                if (removeOnTrig > 0)
                {
                    Buff.CurrentDegree -= removeOnTrig;

                }
                if (Buff.CurrentDegree <= 0 || removeOnTrig == -1)
                {
                    Buff.Disposed = true;
                    Buff.CarrierUnit.BuffHandler.RemoveBuff(Buff.Index, false);
                }
            }
            CurrentTrigCount++;
            return trigContinue;
        }

        public bool CDPass()
        {
            int tick = TrigNode.CD.GetTick(TrigInfo, LastCombatCache);
            if (tick > 0)
            {
                if (Time.Tick >= tick + LastTrigTick)
                {
                    LastTrigTick = Time.Tick;
                    return true;
                }
                return false;
            }
            return true;
        }




        public void Dispose()
        {
            IIndex<BuffTrig>.Remove(Index);
        }
    }
}
