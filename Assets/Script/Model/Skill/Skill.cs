using SkillEditorDemo.Utility;
using System;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public class Skill : IIndex<Skill>
    {
        public int Index { get; set; }
        public int Level;
        public string ID;
        public int ColdDownTick;
        public int Charge;

        public Unit Owner;
        public TrigInfo TrigInfo;
        public SkillNode Data => IData<SkillNode>.Get(ID);

        public bool Update()
        {
            bool change = false;
            if (ColdDownTick > 0)
            {
                ColdDownTick--;
                change = true;
                if (ColdDownTick == 0)
                {
                    int maxCharge = (int)Data.MaxCharge.GetResult(TrigInfo);
                    if (maxCharge > Charge)
                    {
                        Charge++;
                        if (Charge < maxCharge)
                        {
                            ColdDownTick = Data.Time.GetTick(TrigInfo, CombatCache._);
                        }
                    }
                }
            }
            return change;
        }

        public bool TryCast()
        {
            if (Charge <= 0) { return false; }
            if (Data.Condition != null && !Data.Condition.GetResult(TrigInfo)) { return false; }
            ColdDownTick = Data.Time.GetTick(TrigInfo, CombatCache._);
            if (ColdDownTick > 0)
            {
                Charge--;
            }
            Data.Cast(5, TrigInfo, CombatCache._);
            return true;
        }

        public Skill(string id, int level, Unit owner)
        {
            ID = id;
            Index =  IIndex<Skill>.Add(this);
            Level = level;
            Owner = owner;
            TrigInfo = new (owner.Entity)
            {
                SkillID = Index,
            };

            Charge = (int)Data.MaxCharge.GetResult(TrigInfo);
        }

    }

    [Flags]
    public enum SkillWatchType
    {
        [LabelInfo("无监听")]
        None = 0,
        [LabelInfo("血量变化")]
        HpChange = 1,
        [LabelInfo("魔法变化")]
        MpChange = 1 << 1,
    }

}
