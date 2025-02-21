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

        public TrigInfo(EcsPackedEntity source)
        {
            SourceID = source;
            CurrentID = EcsPackedEntity.Empty;
            TriggerID = EcsPackedEntity.Empty;
            BuffCarrierID = EcsPackedEntity.Empty;
            BuffCreatorID = EcsPackedEntity.Empty;
            ColliderID = EcsPackedEntity.Empty;
            EnumeratorID = EcsPackedEntity.Empty;
            CacheList = null;
            BuffID = -1;
            TrigID = -1;
            SkillID = -1;
        }

        public readonly static TrigInfo Empty = new(EcsPackedEntity.Empty);

    }
}
