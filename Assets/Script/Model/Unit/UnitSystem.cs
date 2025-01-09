using Leopotam.EcsLite;

namespace SkillEditorDemo.Model
{
    public class UnitCombatResSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Filter;
        EcsPool<UnitComp> Pool;
        EcsPool<UnitDeadComp> DeadPool;
        public void Init(IEcsSystems systems)
        {
            Filter = systems.GetWorld().Filter<UnitComp>().Exc<UnitDeadComp>().End();
            Pool = systems.GetWorld().GetPool<UnitComp>();
            DeadPool = systems.GetWorld().GetPool<UnitDeadComp>();
        }
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in Filter)
            {
                Unit unit = Pool.Get(entity).Unit;
                unit.Update();
                if (unit.HP.Value <= 0)
                {
                    DeadPool.Add(entity);
                }
            }
        }
    }
    public class UnitDeadSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Filter;
        EcsPool<UnitComp> Pool;
        public void Init(IEcsSystems systems)
        {
            Filter = systems.GetWorld().Filter<UnitDeadComp>().End();
            Pool = systems.GetWorld().GetPool<UnitComp>();
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
            Unit unit = Pool.Get(entity).Unit;

        }



    }


}
