using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
namespace SkillEditorDemo.Model
{
    public class BuffPeriodicSystem : IEcsRunSystem, IEcsInitSystem
    {
        public static TimeWheel<int> TimeWheel = new();
        List<int> trigs;
        public void Init(IEcsSystems systems)
        {
            TimeWheel = new();
            trigs = TimeWheel.Output;
        }
        public void Run(IEcsSystems systems)
        {
            TimeWheel.StepNext();
            for (int i = 0; i < trigs.Count; i++)
            {
                BuffTrig trig = BuffTrig.Get(trigs[i]);
                if (trig != null)
                {
                    trig.Trig(5, new CombatCache(), trig.Buff.SourceID);
                    if (trig.ExistNextPeriodicTrig)
                    {
                        TimeWheel.Add(trig.TrigTickDelay, trigs[i]);
                    }
                }
            }
        }
    }
    public class BuffTimeOutSystem : IEcsRunSystem,IEcsInitSystem
    {
        public static TimeWheel<int> TimeWheel = new();
        List<int> buffs;

        public void Init(IEcsSystems systems)
        {
            TimeWheel = new();
            buffs = TimeWheel.Output;
        }

        public void Run(IEcsSystems systems)
        {
            TimeWheel.StepNext();
            for (int i = 0; i < buffs.Count; i++)
            {
                Buff buff = Buff.Get(buffs[i]);
                if (buff != null && buff.IsTimeOut())
                {
                    buff.CarrierUnit.BuffHandler.RemoveBuff(buffs[i], true);
                }
            }



        }
    }
}
