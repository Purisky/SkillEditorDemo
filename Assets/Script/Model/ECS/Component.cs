using SkillEditorDemo.Utility;

namespace SkillEditorDemo.Model
{
    public struct ReleaseCmp
    { 
        
    }

    public struct UnitCreateCmp
    { 
        
    }
    public struct ProjectileCreateCmp
    {
        public TrigInfo TrigInfo;
        public CombatCache Cache;
        public int GrowID;
        public readonly NewProjectile ProjectileNode => ObjNode.Get<NewProjectile>(GrowID);

        public ProjectileCmp ProjectileCmp => new()
        {
            TrigInfo = TrigInfo,
            Cache = Cache,
            GrowID = GrowID
        };


    }
    public struct HitboxCreateCmp
    {
        public TrigInfo TrigInfo;
        public CombatCache Cache;
        public int GrowID;
        public readonly NewHitbox HitboxNode => ObjNode.Get<NewHitbox>(GrowID);
        public HitboxCmp HitboxCmp => new()
        {
            TrigInfo = TrigInfo,
            Cache = Cache,
            GrowID = GrowID
        };

    }


}
