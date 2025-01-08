using SkillEditorDemo.Utility;
using System.Collections.Generic;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public class UnitMgr : Singleton<UnitMgr>
    {
        public override void Init()
        {


        }
        readonly List<int> deadUnits = new();
        public void Update()
        {
            deadUnits.Clear();
            foreach (Unit unit in IIndex<Unit>.Dic.Values)
            {
                unit.Update();
                if (unit.Dead)
                {
                    deadUnits.Add(unit.Index);
                }
            }
        }


        public void ReleaseDeadUnits()
        {
            for (int i = 0; i < deadUnits.Count; i++)
            {
                ReleaseDeadUnit(deadUnits[i]);
            }
        }

        void ReleaseDeadUnit(int index)
        {

        }



    }
}
