using Leopotam.EcsLite;
namespace SkillEditorDemo.Model
{
    public class GameTickStartSystem : IEcsRunSystem, IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
        }

        public void Run(IEcsSystems systems)
        {
            Time.StepNext();
        }
    }
    public class GameTickEndSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
        }
    }
}
