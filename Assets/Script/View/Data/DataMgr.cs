using SkillEditorDemo.Model;
using SkillEditorDemo.Utility;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo.View
{
    public class DataMgr : Singleton<DataMgr>
    {
        public override void Init()
        {
            InitUnits();








        }

        void InitUnits()
        {
            UnitData[] unitDatas = Resources.LoadAll<UnitData>("Data");
            for (int i = 0; i < unitDatas.Length; i++)
            {
                IData<Model.UnitData>.Add(unitDatas[i].name, unitDatas[i].GenData());
            }
        }

    }
}
