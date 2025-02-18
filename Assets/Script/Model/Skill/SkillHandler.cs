

using System.Collections.Generic;

namespace SkillEditorDemo.Model
{
    public class SkillHandler
    {
        public readonly Unit Unit;
        public Skill[] Skills = new Skill[6];





        public bool TryCast(int index)
        {
            Skill skill = Skills[index];
            if (skill == null)
            {
                return false;
            }
            return skill.TryCast();
        }

    }
}
