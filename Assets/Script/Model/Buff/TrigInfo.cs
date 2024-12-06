using System.Collections.Generic;
using UnityEngine;

namespace SkillEditorDemo
{
    public struct TrigInfo
    {
        #region Unit
        public int CurrentID;
        public int CarrierID;
        public int CreatorID;
        public int SourceID;
        public int TriggerID;
        #endregion

        public int BuffID;
        public int TrigID;

        public readonly static TrigInfo Empty = new()
        {
            CurrentID = -1,
            CarrierID = -1,
            CreatorID = -1,
            SourceID = -1,
            TriggerID = -1,
            BuffID = -1,
            TrigID = -1,
        };

    }
}
