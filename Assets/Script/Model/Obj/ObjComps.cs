using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Numerics;
namespace SkillEditorDemo.Model
{
    public struct ProjectileCmp
    {
        public int Life;
        public TrigInfo TrigInfo;
        public CombatCache Cache;
        public int GrowID;
        public readonly NewProjectile ProjectileNode => ObjNode.Get<NewProjectile>(GrowID);
        public TransformCmp TransformCmp;

    }
    public struct HitboxCmp
    {
        public TrigInfo TrigInfo;
        public CombatCache Cache;
        public int GrowID;
        public readonly NewHitbox HitboxNode => ObjNode.Get<NewHitbox>(GrowID);
        public TransformCmp TransformCmp;
    }



}
