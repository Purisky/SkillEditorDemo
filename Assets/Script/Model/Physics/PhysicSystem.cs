using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    /// <summary>
    /// 碰撞检测
    /// </summary>
    public class PhysicSystem : IEcsRunSystem, IEcsInitSystem
    {


        public void Init(IEcsSystems systems)
        {
            throw new System.NotImplementedException();
        }

        public void Run(IEcsSystems systems)
        {
            throw new System.NotImplementedException();
        }
    }

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




            }
        }
    }



    public class CollisionDetectionSystem : IEcsRunSystem, IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
        }

        public void Run(IEcsSystems systems)
        {
            throw new System.NotImplementedException();
        }
    }


}
