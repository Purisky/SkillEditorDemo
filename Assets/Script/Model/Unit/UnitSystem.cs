using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Numerics;

namespace SkillEditorDemo.Model
{
    public class UnitSystem : IEcsRunSystem, IEcsInitSystem
    {
        public static EcsFilter Filter;
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
