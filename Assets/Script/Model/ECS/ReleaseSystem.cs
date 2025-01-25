using Leopotam.EcsLite;

namespace SkillEditorDemo.Model
{
    public class ReleaseSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Units;
        EcsFilter Projectiles;
        EcsFilter Hitboxes;
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            Units = world.Filter<UnitCmp>().Inc<ReleaseCmp>().End();
            Projectiles = world.Filter<ProjectileCmp>().Inc<ReleaseCmp>().End();
            Hitboxes = world.Filter<HitboxCmp>().Inc<ReleaseCmp>().End();
        }
        public void Run(IEcsSystems systems)
        {
            HandleUnits();
            HandleProjectiles();
            HandleHitboxes();
        }

        void HandleUnits()
        {
            foreach (int entity in Units)
            {
                ReleaseUnit(entity);
            }
        }
        void HandleProjectiles()
        {
            foreach (int entity in Projectiles)
            {
                ReleaseProjectile(entity);
            }
        }
        void HandleHitboxes()
        {
            foreach (int entity in Hitboxes)
            {
                ReleaseHitbox(entity);
            }
        }


        void ReleaseUnit(int entity)
        {

        }
        void ReleaseProjectile(int entity)
        {

        }
        void ReleaseHitbox(int entity)
        {



        }

    }
}
