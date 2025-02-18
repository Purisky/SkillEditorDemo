using Leopotam.EcsLite;
using System;

namespace SkillEditorDemo.Model
{
    public class CreateSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Units;
        EcsFilter Projectiles;
        EcsFilter Hitboxes;
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            Units = world.Filter<UnitCreateCmp>().End();
            Projectiles = world.Filter<ProjectileCreateCmp>().End();
            Hitboxes = world.Filter<HitboxCreateCmp>().End();
        }

        public void Run(IEcsSystems systems)
        {
            CreateUnits();
            CreateProjectiles();
            CreateHitboxes();
        }
        void CreateUnits()
        {
            foreach (int entity in Units)
            {
                CreateUnit(entity);
            }
        }
        void CreateProjectiles()
        {
            foreach (int entity in Projectiles)
            {
                CreateProjectile(entity);
            }
        }
        void CreateHitboxes()
        {
            foreach (int entity in Hitboxes)
            {
                CreateHitbox(entity);
            }
        }
        void CreateUnit(int entity)
        {

        }
        void CreateProjectile(int entity)
        {
            ref ProjectileCreateCmp projectile = ref entity.Get<ProjectileCreateCmp>();
            ProjectileCmp projectileCmp = projectile.ProjectileCmp;
            projectileCmp.Life = projectile.ProjectileNode.Time.GetTick(projectile.TrigInfo, projectile.Cache);
            float radius = projectile.ProjectileNode.Radius.GetResult(projectile.TrigInfo, projectile.Cache);
            Unit unit = Unit.Get(projectile.TrigInfo.SourceID);
            ColliderCmp collider = new(new Circle() { Radius = radius, Entity = entity, Dirty = true }, unit.Faction, ColliderType.Projectile);
            entity.Add(collider);
            entity.Add(projectileCmp);
            projectileCmp.TrigInfo.ColliderID = EcsWorld.Inst.PackEntity(entity);
            float speed = projectile.ProjectileNode.Speed.GetResult(projectile.TrigInfo, projectile.Cache);
            ref TransformCmp transform = ref entity.Get<TransformCmp>();
            VelocityCmp velocity = new()
            {
                Speed = speed * transform.Rot,
            };
            entity.Add(velocity);
            Events.OnCreate.Projectile?.Invoke(entity);


        }

        void CreateHitbox(int entity)
        {
            ref HitboxCreateCmp create = ref entity.Get<HitboxCreateCmp>();
            HitboxCmp hitboxCmp = create.HitboxCmp;
            Unit unit = Unit.Get(create.TrigInfo.SourceID);
            IAABB aABB = create.HitboxNode.Shape.GetShape(create.TrigInfo, create.Cache);
            aABB.Entity = entity;
            ColliderCmp collider = new(aABB, unit.Faction, ColliderType.Hitbox);
            entity.Add(collider);
            entity.Add(hitboxCmp);
            hitboxCmp.TrigInfo.ColliderID =  EcsWorld.Inst.PackEntity(entity);
            Events.OnCreate.Hitbox?.Invoke(entity);

        }





    }
}
