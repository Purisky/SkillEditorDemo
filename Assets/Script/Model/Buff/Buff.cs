using System.Collections.Generic;
using UnityEngine;

namespace SkillEditorDemo
{
    public class Buff : IIndexData<Buff, BuffNode>
    {
        public int Index { get; set; }
        public string ID { get; set; }

        public Unit CarrierUnit;
        public Unit CreatorUnit;
        public Unit SourceUnit;
        public BuffTrig[] Trigs;

        public int CreateFrame;
        public int FreshFrame;
        public int CurrentLevel;
        public int CurrentDegree;
        public float RuntimeData;
        public float CreateData;
        public float CreateData2;
        public bool Disposed;
        public int TotalFrame;
        public TrigInfo TrigInfo;
        public Dictionary<TrigType, BuffTrig[]> Lifecycle;
        public Dictionary<int, float> ValueCache;
        public Dictionary<string, float> ParamValues;
        public BuffNode BuffData=> IData<BuffNode>.Get(ID);
        public static Buff Get(int id) => IIndex<Buff>.Get(id);
        public int CarrierID => CarrierUnit?.Index ?? -1;
        public int CreatorID => CreatorUnit?.Index ?? -1;
        public int SourceID => SourceUnit?.Index ?? -1;
        public bool Alive => !Disposed;
    }

}