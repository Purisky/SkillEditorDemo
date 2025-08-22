using Leopotam.EcsLite;
using SkillEditorDemo.Model;
using UnityEngine;
using Debug = TreeNode.Utility.Debug;

namespace SkillEditorDemo.View
{
    public class RenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        Material Friendly;
        Material Hostile;
        public void Init(IEcsSystems systems)
        {
            Events.OnChange.Transform += OnTransform;
            Events.OnCreate.Unit += OnUnitCreate;
            Events.OnCreate.Projectile += OnProjectileCreate;
            Events.OnCreate.Hitbox += OnHitboxCreate;
            Events.OnDestroy.Unit += OnUnitRelease;
            Events.OnDestroy.Projectile += OnProjectileRelease;
            Events.OnDestroy.Hitbox += OnHitboxRelease;

            Friendly = Resources.Load<Material>("Material/Friendly");
            Hostile = Resources.Load<Material>("Material/Hostile");

            // 创建无限网格地板
            InfiniteGridFloor.CreateGridFloor();
        }

        void OnTransform(int entity)
        {
            TransformCmp? transform = entity.TryGet<TransformCmp>();
            if (!transform.HasValue) { return; }
            RenderCmp? render = entity.TryGet<RenderCmp>();
            if (!render.HasValue) { return; }
            render.Value.Transform.SetTransform(transform.Value);
            if (entity.Exist<InputCmp>())
            {
                Camera.main.transform.position = new Vector3(transform.Value.Pos.X, 10, transform.Value.Pos.Y);
            }
        }


        bool TryCreate(int entity, string id)
        {
            GameObject gameObject = GoPools.Get(id);
            if (null == gameObject)
            {
                Debug.LogError("Can't find prefab: " + id);
                return false;
            }
            ref TransformCmp transform = ref entity.Get<TransformCmp>();
            RenderCmp renderCmp = new(gameObject);
            renderCmp.Transform.SetTransform(transform);
            entity.Add(renderCmp);
            return true;
        }

        void OnUnitCreate(int entity)
        {
            ref UnitCmp unit = ref entity.Get<UnitCmp>();
            if (TryCreate(entity, $"Unit/{unit.Unit.ID}"))
            {
                ref RenderCmp render = ref entity.Get<RenderCmp>();
                ref TransformCmp transform = ref entity.Get<TransformCmp>();
                render.Transform.SetTransform(transform);
            }

        }
        void OnProjectileCreate(int entity)
        {
            ref ProjectileCmp projectile = ref entity.Get<ProjectileCmp>();
            if (TryCreate(entity, $"Projectile/{projectile.ProjectileNode.DisplayPath}"))
            {
                ref RenderCmp render = ref entity.Get<RenderCmp>();
                float radius = projectile.ProjectileNode.Radius.GetResult(projectile.TrigInfo, projectile.Cache);
                render.Transform.localScale = radius / 0.5f * Vector3.one;
                render.GameObject.GetComponent<MeshRenderer>().material = Unit.Get(projectile.TrigInfo.SourceID).Faction == 0 ? Friendly : Hostile;
                ref TransformCmp transform = ref entity.Get<TransformCmp>();
                render.Transform.SetTransform(transform);
            }
        }
        void OnHitboxCreate(int entity)
        {
            ref HitboxCmp hitbox = ref entity.Get<HitboxCmp>();
            if (TryCreate(entity, $"Hitbox/{hitbox.HitboxNode.DisplayPath}"))
            {
                ref RenderCmp render = ref entity.Get<RenderCmp>();
                ShapeNode shape = hitbox.HitboxNode.Shape;
                if (shape is CircleShape circle)
                {
                    float radius = circle.Radius.GetResult(hitbox.TrigInfo, hitbox.Cache);
                    render.Transform.localScale = radius / 0.5f * Vector3.one;
                }
                else if (shape is RectShape rect)
                { 
                    float width = rect.Width.GetResult(hitbox.TrigInfo, hitbox.Cache);
                    float height = rect.Height.GetResult(hitbox.TrigInfo, hitbox.Cache);
                    render.Transform.localScale = new Vector3(width, 1, height);
                }
                else if (shape is SectorShape sector)
                {
                    float radius = sector.Radius.GetResult(hitbox.TrigInfo, hitbox.Cache);
                    float angle = sector.Angle.GetResult(hitbox.TrigInfo, hitbox.Cache);
                    render.GameObject.GetComponent<MeshFilter>().mesh = SectorMeshGenerator.CreateSectorMesh(angle);
                    render.Transform.localScale = radius / 0.5f * Vector3.one;
                }
                render.GameObject.GetComponent<MeshRenderer>().material = Unit.Get(hitbox.TrigInfo.SourceID).Faction == 0 ? Friendly : Hostile;
                ref TransformCmp transform = ref entity.Get<TransformCmp>();
                render.Transform.SetTransform(transform);
            }
        }


        void Release(string id,int entity,int delay_ms = 0)
        {
            ref RenderCmp renderCmp = ref entity.Get<RenderCmp>();
            GoPools.Release(id, renderCmp.GameObject, delay_ms);
        }
        void OnUnitRelease(int entity)
        {
            ref UnitCmp unit = ref entity.Get<UnitCmp>();
            Release($"Unit/{unit.Unit.ID}", entity);
        }
        void OnProjectileRelease(int entity)
        {
            ref ProjectileCmp projectile = ref entity.Get<ProjectileCmp>();
            Release($"Projectile/{projectile.ProjectileNode.DisplayPath}", entity);
        }
        void OnHitboxRelease(int entity)
        {
            ref HitboxCmp hitbox = ref entity.Get<HitboxCmp>();
            Release($"Hitbox/{hitbox.HitboxNode.DisplayPath}", entity,1000);
        }


        public void Run(IEcsSystems systems)
        {
        }
    }
}
