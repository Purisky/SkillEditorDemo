using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkillEditorDemo
{
    public class BuffHandler
    {
        public readonly Unit Unit;
        public Dictionary<string, Dictionary<int, int>> UniqBuffs;
        public Dictionary<TrigType, List<BuffTrig>> TrigDic;
        public HashSet<Buff> Buffs;


        public bool Trig(int trigCount, TrigType type, CombatCache cache, Unit from)
        {
            if (trigCount > 0)
            {
                //可触发次数-1
                trigCount--;
                //如果有来源
                if (from != null)
                {
                    BuffHandler from_ = from.BuffHandler;
                    if (from_ != null)
                    {
                        //主动的情景先触发 再触发被动的
                        if (type.ised())
                        {
                            if (!from_.TrigSingle(trigCount, type.Inverse_ed(), cache, Unit)) { return false; }
                            if (!TrigSingle(trigCount, type, cache, from)) { return false; }
                        }
                        else
                        {
                            if (!TrigSingle(trigCount, type, cache, from)) { return false; }
                            if (!from_.TrigSingle(trigCount, type.Inverse_ed(), cache, Unit)) { return false; }
                        }
                    }
                    else
                    {
                        if (!TrigSingle(trigCount, type, cache, from)) { return false; }
                    }
                }
                else
                {
                    if (!TrigSingle(trigCount, type, cache, from)) { return false; }
                }
            }
            return true;
        }
        /// <summary>
        /// 单向触发
        /// </summary>
        /// <param name="trigCount"></param>
        /// <param name="type"></param>
        /// <param name="cache"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool TrigSingle(int trigCount, TrigType type, CombatCache cache, Unit from = null)
        {
            //找出对应触发器
            if (TrigDic.TryGetValue(type, out List<BuffTrig> arr))
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    if (!arr[i].Trig(trigCount, cache, from != null ? from.Index : -1)) { return false; }
                }
            }
            return true;
        }

        public bool Compete(BuffNode buffData, int sourceID)
        {
            if (buffData.CompeteType == CompeteType.Indie)
            {
                return true;
            }
            else
            {
                switch (buffData.CompeteType)
                {
                    case CompeteType.None:
                    case CompeteType.Eternal:
                        return !IsBuffExist(buffData.ID, buffData.CompeteInSource ? sourceID : -1);
                }
            }
            return true;
        }
        public Buff AddBuff(string buffDataID, int sourceId, int creatorId, int level, int degree, params float[] createParams)
        {
            BuffNode buffData = IData<BuffNode>.Get(buffDataID);
            if (buffData == null)
            {
                Debug.LogError($"BuffData{buffDataID}不存在");
            }
            Buff buff = null;
            if (Compete(buffData, sourceId))
            {
                if (TryGetBuffsByID(buffDataID, buffData.CompeteInSource ? sourceId : -1, out buff))
                {
                    switch (buffData.CompeteType)
                    {
                        case CompeteType.Refresh:
                            buff.Refresh();
                            break;
                        case CompeteType.ReplaceRefresh:
                            buff.SetDegree(degree);
                            buff.CreateParam = createParams;
                            buff.RecountTime();
                            break;
                        case CompeteType.Refresh2Max:
                            buff.SetDegree(-1,true);
                            buff.RecountTime();
                            break;
                        case CompeteType.AddRefresh:
                            buff.AddDegree(degree);
                            buff.RecountTime();
                            break;
                    }
                }
                else
                {
                    TrigInfo trigInfo = new() { 
                        CarrierID = Unit.Index,
                        SourceID = sourceId,
                        CreatorID = creatorId,
                    };
                    buff = new Buff(buffDataID, level, degree, trigInfo, createParams);
                    AddBuff(buff);
                }
            }
            return buff;
        }

        public Buff InitAdd(string buffDataID, int level, int degree, params float[] createParams)
        {
            return AddBuff(buffDataID, Unit.Index, Unit.Index, level, degree, createParams);
        }

        public bool IsBuffExist(string buffID, int sourceId = -1)
        {
            foreach (Buff buff in Buffs)
            {
                if (buff.ID == buffID && (sourceId == -1 || sourceId == buff.SourceID))
                {
                    return true;
                }
            }
            return false;
        }
        public List<int> GetBuffsByID(string buffID, int sourceId = -1)
        {
            BuffNode node = IData<BuffNode>.Get(buffID);
            bool uniq = node.CompeteType != CompeteType.Indie;
            List<int> buffs = new ();
            foreach (Buff buff in Buffs)
            {
                if (buff.ID == buffID && (sourceId == -1 || sourceId == buff.SourceID))
                {
                    buffs.Add(buff.Index);
                    if (uniq)
                    {
                        return buffs;
                    }
                }
            }
            return buffs;
        }

        public bool TryGetBuffsByID(string buffID, int sourceId, out Buff buff)
        {
            List<int> ids = GetBuffsByID(buffID, sourceId);
            buff = null;
            if (ids.Any())
            {
                buff = Buff.Get(ids[0]);
                return buff!=null;
            }
            return false;
        }

        public void AddBuff(Buff buff)
        {
            internalAddBuff(buff);
            buff.LifeTrig(TrigType.Start);
        }

        void internalAddBuff(Buff buff)
        {
            buff.TrigInfo.CarrierID = Unit.Index;
            int sourceId = buff.SourceID;
            if (buff.BuffData.CompeteType != CompeteType.Indie)
            {
                UniqBuffs[buff.BuffData.ID].Add(sourceId, buff.Index);
            }
            foreach (BuffTrig trig in buff.Trigs)
            {
                internalAddBuffTrig(trig);
            }
        }
        void internalAddBuffTrig(BuffTrig trig)
        {
            if (trig.TrigNode.TrigType > TrigType.ByTime_4)//过滤初始化
            {
                if (!TrigDic.TryGetValue(trig.TrigNode.CombinedTrigType, out List<BuffTrig> arr))
                {
                    arr = new() { trig };
                    TrigDic.Add(trig.TrigNode.CombinedTrigType, arr);
                }
                else
                {
                    arr.Add(trig);
                }
            }
            else if (trig.TrigNode.TrigType >= TrigType.ByTime_1)
            {
                BuffMgr.Instance.PeriodicTrigs.Add(trig.TrigTickDelay, trig.Index);
            }
        }




        public void RemoveBuffDegree(int buffID, int degree = 1, bool trigLess = false)
        {
            Buff buff = Buff.Get(buffID);
            if (buff != null)
            {
                if (degree < 0 || buff.CurrentDegree <= degree)
                {
                    RemoveBuff(buffID, false, trigLess);
                }
            }
        }
        public void RemoveBuff(int buffID, bool byTimeOut, bool skipLifeTrig = false)
        {
            Buff buff = Buff.Get(buffID);
            if (!skipLifeTrig)
            {
                if (byTimeOut)
                {
                    buff.LifeTrig(TrigType.TimeOut);
                }
                else
                {
                    buff.LifeTrig(TrigType.Remove);
                }
                buff.LifeTrig(TrigType.RemoveOrTimeOut);
            }
            internalRemoveBuff(buff);
        }
        void internalRemoveBuff(Buff buff)
        {
            bool uniq = buff.BuffData.CompeteType != CompeteType.Indie;
            if (uniq)
            {
                Dictionary<int, int> buffById = UniqBuffs[buff.BuffData.ID];
                buffById.Remove(buff.SourceID);
            }
            Buffs.Remove(buff);
            foreach (BuffTrig trig in buff.Trigs)
            {
                if (trig.TrigNode.TrigType > TrigType.ByTime_4)//过滤初始化
                {
                    TrigDic[trig.TrigNode.TrigType].Remove(trig);
                }
            }
            buff.Dispose();
        }

        public struct BuffIndex
        { 
            public int Index;
            public int SourceID;
        }
    }
}
