using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkillEditorDemo.Model
{
    public class Buff : IIndexData<Buff, BuffNode>
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public BuffTrig[] Trigs;

        public int CreateTick;
        public int FreshTick;
        public int CurrentLevel;
        public int CurrentDegree;
        public float RuntimeData;
        public float[] CreateParam;
        public bool Disposed;
        public int TotalTick;
        public TrigInfo TrigInfo;
        public Dictionary<TrigType, BuffTrig[]> Lifecycle;
        public Dictionary<int, float> ValueCache;
        public Dictionary<string, float> ParamValues;
        public BuffNode BuffData=> IData<BuffNode>.Get(ID);
        public static Buff Get(int id) => IIndex<Buff>.Get(id);
        public int SourceID => TrigInfo.SourceID;
        public bool Alive => !Disposed;
        public Unit CarrierUnit => Unit.Get(TrigInfo.CurrentID);
        public Unit CreatorUnit => Unit.Get(TrigInfo.BuffCreatorID);
        public Unit SourceUnit => Unit.Get(SourceID);
        public Buff(string Id, int level, int degree, TrigInfo trigInfo, float[] createParam)
        {
            ID = Id;
            IIndex<Buff>.Add(this);
            TrigInfo = trigInfo;
            TrigInfo.BuffID = Index;
            CreateParam = createParam;
            CreateTick = Time.Tick;
            FreshTick = CreateTick;
            CurrentLevel = level;
            SetDegree(degree,true);
            BuffNode data = IData< BuffNode>.Get(Id);
            List<BuffTrig> trigs = new();
            List<BuffTrig>[] lifecycles = new List<BuffTrig>[4];

            for (int i = 0; i < data.Triggers.Count; i++)
            {
                BuffTrig buffTrig = new (data.Triggers[i], this);
                TrigType trigType = data.Triggers[i].TrigType;
                switch (trigType)
                {
                    case TrigType.Start:
                    case TrigType.Remove:
                    case TrigType.TimeOut:
                    case TrigType.RemoveOrTimeOut:
                        lifecycles[(int)trigType] ??= new();
                        lifecycles[(int)trigType].Add(buffTrig);
                        break;
                }
                trigs.Add(buffTrig);
            }
            for (int i = 0; i < lifecycles.Length; i++)
            {
                if (lifecycles[i]?.Any() ?? false)
                {
                    Lifecycle.Add((TrigType)i, lifecycles[i].ToArray());
                }
            }
            Trigs = trigs.ToArray();
            RecountTime(false);
        }






        



        public void LifeTrig(TrigType trigType)
        {
            if (Lifecycle.TryGetValue(trigType, out BuffTrig[] trigs))
            {
                for (int i = 0; i < trigs.Length; i++)
                {
                    trigs[i].Trig(5, CombatCache._, CreatorUnit.Index);
                }
            }
        }
        public bool IsTimeOut() => TotalTick + FreshTick >= Time.Tick;
        public void Refresh()
        {
            FreshTick = Time.Tick;
            BuffTimeOutSystem.TimeWheel.Add(TotalTick, Index);
        }

        public void SetLevel(int level)
        {
            if (level == CurrentLevel) { return; }
            int oldLevel = CurrentLevel;
            CurrentLevel = BuffData.MaxLevel > 0 ? (int)MathF.Min(BuffData.MaxLevel, level) : level;
        }
        public void AddLevel(int level)
        {
            SetLevel(CurrentLevel + level);
        }
        public void SetDegree(int degree, bool trigLess = false)
        {
            if(degree == CurrentDegree) { return; }
            int oldDegree = CurrentDegree;
            int maxDegree = (int)BuffData.MaxDegree.GetResult(TrigInfo);
            if (maxDegree > 0)
            {
                CurrentDegree = degree == -1 ? maxDegree: (int)MathF.Min(maxDegree, degree);
            }
            else {
                if (degree == -1) { return; }
                CurrentDegree = degree;
            }
            if (oldDegree== CurrentDegree) { return; }
            if (CurrentDegree <= 0)
            {
                CarrierUnit.BuffHandler.RemoveBuff(Index, false, trigLess);
            }
        }
        public void AddDegree(int degree)=> SetDegree(CurrentDegree + degree);

        public void RecountTime(bool refresh = true)
        {
            if (refresh) { FreshTick = Time.Tick; }
            int oldTotalTick = TotalTick;
            TotalTick =  BuffData.Time.GetTick(TrigInfo,CombatCache._);
            if (oldTotalTick == TotalTick) { return; }
            if (TotalTick > 0)
            {
                int left = FreshTick + TotalTick - Time.Tick;
                if (left > 0)
                {
                    BuffTimeOutSystem.TimeWheel.Add(left, Index);
                }
                else
                { 
                    CarrierUnit.BuffHandler.RemoveBuff(Index, false);
                }
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < Trigs.Length; i++)
            {
                Trigs[i].Dispose();
            }
            IIndex<Buff>.Remove(Index);
        }
    }
}