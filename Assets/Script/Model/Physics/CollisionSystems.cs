using Leopotam.EcsLite;
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
            foreach (int entity in Filter)
            {
                ref ColliderCmp collider = ref entity.Get<ColliderCmp>();
                returnObjects.Clear();
                Quadtree.Retrieve(returnObjects, collider.Shape.AABB);

                foreach (var other in returnObjects)
                {
                    if (collider.Shape.Entity != other.Entity && IsColliding(collider.Shape.AABB, other.AABB))
                    {
                        int entityA = Math.Min(collider.Shape.Entity, other.Entity);
                        int entityB = Math.Max(collider.Shape.Entity, other.Entity);
                        RawCollisions.Add(new Collision(entityA, entityB));
                    }
                }
            }
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
            Unit unit = Unit.Get(_unit);
            ref HitboxCmp hitbox = ref _hitbox.Get<HitboxCmp>();



        }
        void ProjectileHitUnit(int _projectile, int _unit)
        {
            Unit unit = Unit.Get(_unit);
            ref ProjectileCmp projectile = ref _projectile.Get<ProjectileCmp>();


        }
        void ProjectileHitTerrain(int _projectile, int _terrain)
        {
            ref ProjectileCmp projectile = ref _projectile.Get<ProjectileCmp>();
            projectile.TrigInfo.CurrentID = -1;
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
}
