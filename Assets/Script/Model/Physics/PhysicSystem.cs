using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System;
using System.Numerics;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public class VelocitySystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Filter;

        
        public void Init(IEcsSystems systems)
        {
            Filter = systems.GetWorld().Filter<TransformCmp>().Inc<VelocityCmp>().End();
        }



        public void Run(IEcsSystems systems)
        {
            foreach (var entity in Filter)
            {
                ref TransformCmp transform = ref entity.Get<TransformCmp>();
                ref VelocityCmp velocity = ref entity.Get<VelocityCmp>();
                transform.Pos += velocity.Speed * Time.GameTickDuration;
                transform.Rot += velocity.Rot * Time.GameTickDuration;
                ref ColliderCmp collider = ref entity.Get<ColliderCmp>();
                collider.Shape.SetDirty(ref transform);
                Events.OnChange.Transform?.Invoke(entity);
            }
        }





    }



}
