using SkillEditorDemo.Utility;

namespace SkillEditorDemo.Model
{
    public struct ReleaseCmp
    { 
        
    }

    public struct UnitCreateCmp
    {
        public string ID;
        public string Name;
        public int Faction;
        public bool IsPlayer;

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
        public readonly OneShotBox HitboxNode => ObjNode.Get<OneShotBox>(GrowID);
        public HitboxCmp HitboxCmp => new()
        {
            TrigInfo = TrigInfo,
            Cache = Cache,
            GrowID = GrowID
        };

    }


}
