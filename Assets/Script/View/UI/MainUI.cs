using Leopotam.EcsLite;
using SkillEditorDemo.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkillEditorDemo.View
{
    [UxmlElement]
    public partial class MainUI : VisualElement
    {
        public SkillBtn[] SkillBtns;




        public MainUI()
        {
            Resources.Load<VisualTreeAsset>("UI/MainUI").CloneTree(this);
            SkillBtns = this.Query<SkillBtn>().ToList().ToArray(); 
            Events.OnChange.Skill += SkillUpdate;

            Events.OnCreate.Player += InitPlayer;
        }

        void SkillUpdate(int entity, int index)
        {
            SkillHandler skillHandler = Unit.Get(entity).SkillHandler;
            SkillBtns[index].SetSkill(skillHandler.Skills[index]);
        }
        void InitPlayer(int entity)
        {
            Unit unit = Unit.Get(entity);
            InitSkills(unit);

        }
        void InitSkills(Unit unit)
        {
            for (int i = 0; i < unit.SkillHandler.Skills.Length; i++)
            {
                SkillBtns[i].SetSkill(unit.SkillHandler.Skills[i]);
            }
        }
    }
}
