using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SkillEditorDemo.Model
{
    public class CreateSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsFilter Units;
        EcsFilter Projectiles;
        EcsFilter Hitboxes;

        public static List<(TransformCmp, UnitCreateCmp)> InitInfos;
        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            Units = world.Filter<UnitCreateCmp>().Exc<UnitCmp>().End();
            Projectiles = world.Filter<ProjectileCreateCmp>().Exc<ProjectileCmp>().End();
            Hitboxes = world.Filter<HitboxCreateCmp>().Exc<HitboxCmp>().End();
            if (InitInfos != null)
            {
                for (int i = 0; i < InitInfos.Count; i++)
                {
                    (TransformCmp trans, UnitCreateCmp unit) = InitInfos[i];
                    int entity = EcsWorld.Inst.NewEntity();
                    entity.Add(trans);
                    entity.Add(unit);
                }
            }
        }
        public static void AddInitUnit(string id, string name, int faction, Vector2 pos, Angle rot, bool player = false)
        {
            TransformCmp transformCmp = new() { Pos = pos, Rot = rot };
            UnitCreateCmp unitCreateCmp = new() { ID = id, Name = name, Faction = faction, IsPlayer = player };
            if (InitInfos == null)
            {
                InitInfos = new List<(TransformCmp, UnitCreateCmp)>();
            }
            InitInfos.Add((transformCmp, unitCreateCmp));
        }
        public static int AddUnit(string id,string name, int faction, Vector2 pos, Angle rot, bool player = false)
        {
            TransformCmp transformCmp = new () { Pos = pos, Rot = rot };
            UnitCreateCmp unitCreateCmp = new () { ID = id,Name = name, Faction = faction, IsPlayer = player };
            int entity = EcsWorld.Inst.NewEntity();
            //Debug.Log(entity);
            entity.Add(transformCmp);
            entity.Add(unitCreateCmp);
            return entity;
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
            ref UnitCreateCmp unitCreate = ref entity.Get<UnitCreateCmp>();
            ref TransformCmp transform = ref entity.Get<TransformCmp>();
            
            // 处理Unit名称：如果unitCreate.Name为空，使用默认名称
            string unitName = string.IsNullOrEmpty(unitCreate.Name) ? unitCreate.ID : unitCreate.Name;
            
            Unit unit = new(entity, unitCreate.ID, unitCreate.Faction) { Name = unitName };
            float radius = unit.Data.Radius;
            ColliderCmp collider = new(new Circle() { Radius = radius, Entity = entity }, unit.Faction, ColliderType.Unit);
            collider.Shape.SetDirty(ref transform);
            entity.Add(collider);
            entity.Add(new UnitCmp() { Unit = unit });
            if (unitCreate.IsPlayer)
            {
                entity.Add(new InputCmp());
                Events.OnCreate.Player?.Invoke(entity);
            }
            else//todo add ai
            { 
            
            }
            Events.OnCreate.Unit?.Invoke(entity);
        }
        void CreateProjectile(int entity)
        {
            ref ProjectileCreateCmp projectile = ref entity.Get<ProjectileCreateCmp>();
            ref TransformCmp transform = ref entity.Get<TransformCmp>();
            ProjectileCmp projectileCmp = projectile.ProjectileCmp;
            projectileCmp.Life = projectile.ProjectileNode.Time.GetTick(projectile.TrigInfo, projectile.Cache);
            float radius = projectile.ProjectileNode.Radius.GetResult(projectile.TrigInfo, projectile.Cache);
            Unit unit = Unit.Get(projectile.TrigInfo.SourceID);
            ColliderCmp collider = new(new Circle() { Radius = radius, Entity = entity}, unit.Faction, ColliderType.Projectile);
            collider.Shape.SetDirty(ref transform);
            entity.Add(collider);
            entity.Add(projectileCmp);
            projectileCmp.TrigInfo.ColliderID = EcsWorld.Inst.PackEntity(entity);
            float speed = projectile.ProjectileNode.Speed.GetResult(projectile.TrigInfo, projectile.Cache);
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
            ref TransformCmp transform = ref entity.Get<TransformCmp>();
            HitboxCmp hitboxCmp = create.HitboxCmp;
            Unit unit = Unit.Get(create.TrigInfo.SourceID);
            IAABB aABB = create.HitboxNode.Shape.GetShape(create.TrigInfo, create.Cache);
            aABB.Entity = entity;
            ColliderCmp collider = new(aABB, unit.Faction, ColliderType.Hitbox);
            collider.Shape.SetDirty(ref transform);
            entity.Add(collider);
            entity.Add(hitboxCmp);
            hitboxCmp.TrigInfo.ColliderID =  EcsWorld.Inst.PackEntity(entity);
            Events.OnCreate.Hitbox?.Invoke(entity);
        }





    }
}
