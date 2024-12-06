using UnityEngine;

namespace SkillEditorDemo
{
    public class BuffTrig : IIndex<BuffTrig>,IGrowID
    {
        public int Index { get ; set ; }
        public int GrowID { get; set; }

        public Buff Buff;
        public int LastTrigFrame = int.MinValue;
        public ActionNode[] Actions;
        TrigInfo TrigInfo;
        public int[] LastSearchResult;
        public CombatCache LastCombatCache;
        public int TrigFrameDelay;
        public int CurrentTrigCount;

        public static BuffTrig Get(int id) => IIndex<BuffTrig>.Get(id);
        public TriggerNode TriggerNode => IGrowID<TriggerNode>.Get(GrowID);


        public float GetValue(int actionId)
        {
            return ActionNode.Get<StatModify>(actionId).GetRuntimeValue(TrigInfo, LastCombatCache);
        }
    }
}
