using Leopotam.EcsLite;
using SkillEditorDemo.Model;
using SkillEditorDemo.Utility;
using System;
using TreeNode.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
namespace SkillEditorDemo.View
{
    public class InputSystem : IEcsInitSystem, IEcsRunSystem
    {
        public InputAction MoveAction;
        public InputAction LookAction;
        public InputAction[] Skills;//{lm,rm,space,q,e,r}

        EcsFilter InputFilter;

        public void Init(IEcsSystems systems)
        {
            MoveAction = UnityEngine.InputSystem.InputSystem.actions.FindAction("Move");
            LookAction = UnityEngine.InputSystem.InputSystem.actions.FindAction("Look");
            Skills = new InputAction[6];
            for (int i = 0; i < Skills.Length; i++)
            {
                Skills[i] = UnityEngine.InputSystem.InputSystem.actions.FindAction($"Skill{i}");
            }
            InputFilter = systems.GetWorld().Filter<TransformCmp>().Inc<UnitCmp>().Inc<InputCmp>().End();
            

        }
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in InputFilter)
            {
                UpdateMove_Look(entity);
                UpdateSkills(entity);
            }
        }
        void UpdateMove_Look(int entity)
        { 
            ref TransformCmp transform = ref entity.Get<TransformCmp>();
            Vector2 move = MoveAction.ReadValue<Vector2>();
            if (move.x == 0 && move.y == 0)
            {
                entity.Del<VelocityCmp>();
            }
            else
            {
                Unit unit = Unit.Get(entity);
                ref VelocityCmp velocity = ref entity.GetAdd<VelocityCmp>();
                velocity.Speed = move.normalized.ToNumerics() * unit.StatHandler[StatType.MoveSpeed];
            }
            Ray ray = Camera.main.ScreenPointToRay(LookAction.ReadValue<Vector2>());
            float y = ray.origin.y;
            float t = -y / ray.direction.y;
            Vector3 pos = ray.origin + t * ray.direction;
            System.Numerics.Vector2 mousePos = new (pos.x, pos.z);
            System.Numerics.Vector2 dir = mousePos - transform.Pos;
            transform.Rot =dir;
        }
        void UpdateSkills(int entity)
        {
            ref UnitCmp unitCmp = ref entity.Get<UnitCmp>();
            ref Unit unit = ref unitCmp.Unit;
            for (int i = 0; i < Skills.Length; i++)
            {
                if (Skills[i].triggered)
                {
                    unit.SkillHandler.TryCast(i);
                }
            }
        }
    }
}
