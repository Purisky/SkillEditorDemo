using TreeNode.Utility;
using Leopotam.EcsLite;

namespace SkillEditorDemo.Model
{
    /// <summary>
    /// AI系统
    /// </summary>
    public class AISystem : IEcsInitSystem, IEcsRunSystem
    {
        EcsFilter Filter;

        public void Init(IEcsSystems systems)
        {
            Filter = systems.GetWorld().Filter<UnitCmp>().Inc<AICmp>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in Filter)
            {
                ref AICmp ai = ref entity.Get<AICmp>();

                Unit unit = Unit.Get(entity);


            }

        }
    }
}
