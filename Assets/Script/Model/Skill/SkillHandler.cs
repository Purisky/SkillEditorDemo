

using Leopotam.EcsLite;
using System.Collections.Generic;

namespace SkillEditorDemo.Model
{
    public class SkillHandler
    {
        public readonly Unit Unit;
        public Skill[] Skills = new Skill[6];


        public SkillHandler(Unit unit)
        {
            Unit = unit;
        }
        public void AddSkill(int index, string id, int level)
        {
            Skills[index] = new Skill(id, level, Unit);
        }

        public void Update()
        {
            bool player = Unit.Entity.Id.Exist<InputCmp>();
            for (int i = 0; i < Skills.Length; i++)
            {
                if (Skills[i] != null)
                {
                    if (Skills[i].Update() && player)
                    { 
                        Events.OnChange.Skill?.Invoke(Unit.Entity.Id, i);
                    }
                }
            }
        }

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
