using Leopotam.EcsLite;

namespace SkillEditorDemo.Model
{
    public class UnitCombatResSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Filter;
        public void Init(IEcsSystems systems)
        {
            Filter = systems.GetWorld().Filter<UnitCmp>().Exc<UnitDeadComp>().End();
        }
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in Filter)
            {
                Unit unit = Unit.Get(entity);
                unit.Update();
                if (unit.HP.Value <= 0)
                {
                    entity.Add<UnitDeadComp>();
                }
            }
        }
    }
    public class UnitDeadSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Filter;
        public void Init(IEcsSystems systems)
        {
            Filter = systems.GetWorld().Filter<UnitDeadComp>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in Filter)
            {
                ReleaseDeadUnit(entity);
            }
        }
        void ReleaseDeadUnit(int entity)
        {
            Unit unit = Unit.Get(entity);

        }



    }


}
