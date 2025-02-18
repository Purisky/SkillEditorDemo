using SkillEditorDemo.Utility;
using System;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public class Skill : IIndex<Skill>
    {
        public int Index { get; set; }
        public string ID;
        public SkillNode SkillData;
        public int ColdDownTick;
        public int Charge;


        public bool TryCast()
        {
            return true;
        }
    }

    [Flags]
    public enum SkillWatchType
    {
        [LabelInfo("血量变化")]
        HpChange = 1,
        [LabelInfo("魔法变化")]
        MpChange = 1 << 1,
    }

}
