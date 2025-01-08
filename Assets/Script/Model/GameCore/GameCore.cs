using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo.Model
{
    public class GameCore : Singleton<GameCore>
    {

        public override void Init()
        {

        }



        public void Update()
        {
            if (Time.Pause) { return; }
            Time.StepNext();
            TimeWheels.StepNext();
            UnitMgr.Inst.Update();
            BuffMgr.Inst.Update();




            UnitMgr.Inst.ReleaseDeadUnits();
        }







    }
}
