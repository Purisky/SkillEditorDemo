using Leopotam.EcsLite;

namespace SkillEditorDemo.Model
{
    public class UnitCombatResSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Filter;
        public void Init(IEcsSystems systems)
        {
            Filter = systems.GetWorld().Filter<UnitCmp>().Exc<ReleaseCmp>().End();
        }
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in Filter)
            {
                Unit unit = Unit.Get(entity);
                unit.Update();
                if (unit.HP.Value <= 0)
                {
                    entity.Add<ReleaseCmp>();
                }
            }
        }
    }
}
