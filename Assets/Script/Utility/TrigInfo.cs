using Leopotam.EcsLite;

namespace SkillEditorDemo.Utility
{
    public struct TrigInfo
    {
        #region Unit
        public EcsPackedEntity CurrentID;
        public EcsPackedEntity SourceID;
        public EcsPackedEntity TriggerID;//Unit   trig in buff/ collided unit in collision 
        #endregion



        #region Buff
        public EcsPackedEntity BuffCarrierID;
        public EcsPackedEntity BuffCreatorID;

        public int BuffID;
        public int TrigID;
        #endregion

        #region Skill
        public int SkillID;
        #endregion

        #region Collision
        public EcsPackedEntity ColliderID;
        #endregion

        #region Enumerator
        public EcsPackedEntity EnumeratorID;
        public EcsPackedEntity[] CacheList;
        #endregion




        public readonly static TrigInfo Empty = new()
        {
            CurrentID = EcsPackedEntity.Empty,
            BuffCarrierID = EcsPackedEntity.Empty,
            BuffCreatorID = EcsPackedEntity.Empty,
            SourceID = EcsPackedEntity.Empty,
            TriggerID = EcsPackedEntity.Empty,
            BuffID = -1,
            TrigID = -1,
        };

    }
}
