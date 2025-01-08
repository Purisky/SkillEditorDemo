using System.Collections.Generic;
using UnityEngine;

namespace SkillEditorDemo.Utility
{
    public struct ObjInfo
    {
        #region Unit
        public int CurrentID;
        public int SourceID;
        #endregion



        #region Buff
        public int BuffCarrierID;
        public int BuffCreatorID;
        public int TriggerID;//Unit
        public int BuffID;
        public int TrigID;
        #endregion






        public readonly static ObjInfo Empty = new()
        {
            CurrentID = -1,
            BuffCarrierID = -1,
            BuffCreatorID = -1,
            SourceID = -1,
            TriggerID = -1,
            BuffID = -1,
            TrigID = -1,
        };

    }
}
