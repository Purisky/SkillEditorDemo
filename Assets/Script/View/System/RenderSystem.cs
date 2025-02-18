using Leopotam.EcsLite;
using SkillEditorDemo.Model;
using UnityEngine;
using Debug = SkillEditorDemo.Utility.Debug;

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
            Friendly = Resources.Load<Material>("Material/Friendly");
            Hostile = Resources.Load<Material>("Material/Hostile");



        }

        void OnTransform(int entity)
        {
            TransformCmp? transform = entity.TryGet<TransformCmp>();
            if (!transform.HasValue) { return; }
            RenderCmp? render = entity.TryGet<RenderCmp>();
            if (!render.HasValue) { return; }
            render.Value.Transform.SetTransform(transform.Value);
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
            TryCreate(entity,$"Unit/{unit.Unit.ID}" );
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
            }
        }






        public void Run(IEcsSystems systems)
        {
        }
    }
}
