using Leopotam.EcsLite;
using SkillEditorDemo.Utility;

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

        static void RemoveCollider(int entity)
        {
            ref ColliderCmp collider = ref entity.Get<ColliderCmp>();
            CollisionDetectionSystem.Quadtree.Remove(collider.Shape);
            entity.Del<ColliderCmp>();
        }
        void ReleaseUnit(int entity)
        {
            Debug.Log($"ReleaseUnit{entity}");
            RemoveCollider(entity);
            Events.OnDestroy.Unit?.Invoke(entity);
            EcsWorld.Inst.DelEntity(entity);
        }
        void ReleaseProjectile(int entity)
        {
            RemoveCollider(entity);
            Events.OnDestroy.Projectile?.Invoke(entity);
            EcsWorld.Inst.DelEntity(entity);
        }
        void ReleaseHitbox(int entity)
        {
            RemoveCollider(entity);
            Events.OnDestroy.Hitbox?.Invoke(entity);
            EcsWorld.Inst.DelEntity(entity);
        }

    }
}
