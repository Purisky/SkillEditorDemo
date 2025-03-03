using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;

namespace SkillEditorDemo.Model
{
    public class CollisionDetectionSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Filter;
        public static Quadtree Quadtree;
        static HashSet<Collision> RawCollisions;


        public void Init(IEcsSystems systems)
        {
            Quadtree = new Quadtree(0, new() { Center = new(0, 0), Size = new(100, 100) });
            Filter = systems.GetWorld().Filter<ColliderCmp>().End();
            RawCollisions = new();
        }

        public void Run(IEcsSystems systems)
        {
            if (Filter.GetEntitiesCount() <= 1) { return; }
            UpdateQuadtree();
            RawDetection();
            Detection();
        }

        void UpdateQuadtree()
        {
            foreach (int entity in Filter)
            {
                ref ColliderCmp collider = ref entity.Get<ColliderCmp>();
                Quadtree.Update(ref collider.Shape);
            }
        }

        void RawDetection()
        {
            List<IAABB> returnObjects = new();
            RawCollisions.Clear();
            foreach (int entity in Filter)
            {
                ref ColliderCmp collider = ref entity.Get<ColliderCmp>();
                returnObjects.Clear();
                Quadtree.Retrieve(returnObjects, collider.Shape.AABB);
                foreach (var other in returnObjects)
                {
                    if (collider.Shape.Entity == other.Entity) { continue; }
                    bool raw = IsColliding(collider.Shape.AABB, other.AABB);
                    //Debug.Log($"{collider.Shape.AABB}=>{other.AABB}");
                    if (raw)
                    {
                        int entityA = Math.Min(collider.Shape.Entity, other.Entity);
                        int entityB = Math.Max(collider.Shape.Entity, other.Entity);
                        RawCollisions.Add(new Collision(entityA, entityB));
                    }
                }
            }
            //Debug.Log(RawCollisions.Count);
        }

        void Detection()
        {
            foreach (Collision collision in RawCollisions)
            {
                ref ColliderCmp colliderA = ref collision.EntityA.Get<ColliderCmp>();
                ref ColliderCmp colliderB = ref collision.EntityB.Get<ColliderCmp>();
                if (!colliderA.IsColliderTo(ref colliderB))
                {
                    continue;
                }
                //Debug.Log($"{colliderA.Shape.AABB}=>{colliderB.Shape.AABB}");

                ref TransformCmp transformA = ref collision.EntityA.Get<TransformCmp>();
                ref TransformCmp transformB = ref collision.EntityB.Get<TransformCmp>();
                if (colliderA.Shape is Circle circleA)
                {
                    if (circleA.IsColliderTo(in transformA, in colliderB.Shape, in transformB))
                    {
                        CollisionHandlerSystem.Collisions.Add(new(colliderA, colliderB));
                    }
                }
                else if (colliderB.Shape is Circle circleB)
                {
                    if (circleB.IsColliderTo(in transformB, in colliderA.Shape, in transformA))
                    {
                        CollisionHandlerSystem.Collisions.Add(new(colliderA, colliderB));
                    }
                }
            }
        }





        bool IsColliding(AABB a, AABB b)
        {
            return (a.Center.X - a.Size.X / 2 < b.Center.X + b.Size.X / 2 &&
                    a.Center.X + a.Size.X / 2 > b.Center.X - b.Size.X / 2 &&
                    a.Center.Y - a.Size.Y / 2 < b.Center.Y + b.Size.Y / 2 &&
                    a.Center.Y + a.Size.Y / 2 > b.Center.Y - b.Size.Y / 2);
        }


    }
    public class CollisionHandlerSystem : IEcsRunSystem, IEcsInitSystem
    {
        public static HashSet<Collision> Collisions;

        public void Init(IEcsSystems systems)
        {
            Collisions = new();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (Collision collision in Collisions)
            {
                if (collision.HitType == (ColliderType.Hitbox | ColliderType.Unit))
                {
                    HitboxHitUnit(collision.EntityA, collision.EntityB);
                }
                else if (collision.HitType == (ColliderType.Projectile | ColliderType.Unit))
                {
                    ProjectileHitUnit(collision.EntityA, collision.EntityB);
                }
                else if (collision.HitType == (ColliderType.Projectile | ColliderType.Terrain))
                {
                    ProjectileHitTerrain(collision.EntityA, collision.EntityB);
                }
            }
            Collisions.Clear();
        }

        void HitboxHitUnit(int _hitbox, int _unit)
        {
            //Debug.Log("HitboxHitUnit");
            Unit unit = Unit.Get(_unit);
            ref HitboxCmp hitbox = ref _hitbox.Get<HitboxCmp>();
            hitbox.TrigInfo.TriggerID = unit.Entity;
            hitbox.HitboxNode.Hit(hitbox.TrigInfo, hitbox.Cache);
        }
        void ProjectileHitUnit(int _projectile, int _unit)
        {
            Unit unit = Unit.Get(_unit);
            ref ProjectileCmp projectile = ref _projectile.Get<ProjectileCmp>();
            projectile.TrigInfo.TriggerID = unit.Entity;
            projectile.ProjectileNode.Hit(projectile.TrigInfo, projectile.Cache);
            _projectile.Add<ReleaseCmp>();
        }
        void ProjectileHitTerrain(int _projectile, int _terrain)
        {
            ref ProjectileCmp projectile = ref _projectile.Get<ProjectileCmp>();
            projectile.TrigInfo.TriggerID = EcsPackedEntity.Empty;
            if (projectile.ProjectileNode.TrigOnHitTerrain)
            {
                for (int i = 0; i < projectile.ProjectileNode.Actions.Count; i++)
                {
                    ActionNode actionNode = projectile.ProjectileNode.Actions[i];
                    actionNode.Handle(5, projectile.TrigInfo, projectile.Cache);
                }
            }
        }


    }



    public class AfterCollisionSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Hitboxes;
        EcsFilter Projectiles;
        public void Init(IEcsSystems systems)
        {
            Hitboxes = systems.GetWorld().Filter<HitboxCmp>().Exc<ReleaseCmp>().End();
            Projectiles = systems.GetWorld().Filter<ProjectileCmp>().Exc<ReleaseCmp>().End();
        }

        public void Run(IEcsSystems systems)
        {
            ReleaseProjectiles();
            ReleaseHitboxes();
        }
        void ReleaseProjectiles()
        {
            foreach (int entity in Projectiles)
            {
                ref ProjectileCmp projectile = ref entity.Get<ProjectileCmp>();
                projectile.Life--;
                if (projectile.Life <= 0)
                {
                    entity.Add<ReleaseCmp>();
                }
            }
        }


        void ReleaseHitboxes()
        {
            foreach (int entity in Hitboxes)
            {
                entity.Add<ReleaseCmp>();
            }
        }
    }
}
