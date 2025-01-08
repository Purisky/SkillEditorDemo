using System.Collections.Generic;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public class BuffMgr : Singleton<BuffMgr>
    {
        public TimeWheel<int> PeriodicTrigs = new();
        public TimeWheel<int> TimeOutBuffs = new();

        public void Update()
        {
            HandlePeriodic();
            HandleTimeOut();
        }

        void HandlePeriodic()
        {
            List<int> trigs = PeriodicTrigs.Output;
            for (int i = 0; i < trigs.Count; i++)
            {
                BuffTrig trig = BuffTrig.Get(trigs[i]);
                if (trig != null)
                {
                    trig.Trig(5, new CombatCache(), trig.Buff.SourceID);
                    if (trig.ExistNextPeriodicTrig)
                    {
                        PeriodicTrigs.Add(trig.TrigTickDelay, trigs[i]);
                    }
                }
            }
        }
        void HandleTimeOut()
        {
            List<int> buffs = TimeOutBuffs.Output;
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
