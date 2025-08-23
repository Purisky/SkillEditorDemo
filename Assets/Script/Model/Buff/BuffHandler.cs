using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System.Collections.Generic;
using System.Linq;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public class BuffHandler
    {
        public readonly Unit Unit;
        public Dictionary<string, Dictionary<EcsPackedEntity, int>> UniqBuffs;
        public Dictionary<TrigType, List<BuffTrig>> TrigDic;
        public HashSet<Buff> Buffs;



        public BuffHandler(Unit unit)
        {
            Unit = unit;
            UniqBuffs = new();
            TrigDic = new();
            Buffs = new();
        }

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
                    if (!arr[i].Trig(trigCount, cache, from != null ? from.Entity : EcsPackedEntity.Empty)) { return false; }
                }
            }
            return true;
        }

        public bool Compete(BuffNode buffData, EcsPackedEntity sourceID)
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
                        return !IsBuffExist(buffData.ID, buffData.CompeteInSource ? sourceID : EcsPackedEntity.Empty);
                }
            }
            return true;
        }
        public Buff AddBuff(string buffDataID, EcsPackedEntity sourceId, EcsPackedEntity creatorId, int level, int degree, params float[] createParams)
        {
            BuffNode buffData = IData<BuffNode>.Get(buffDataID);
            if (buffData == null)
            {
                Debug.LogError($"BuffData{buffDataID}不存在");
            }
            Buff buff = null;
            if (Compete(buffData, sourceId))
            {
                if (TryGetBuffsByID(buffDataID, buffData.CompeteInSource ? sourceId : EcsPackedEntity.Empty, out buff))
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
                    TrigInfo trigInfo = new(sourceId) { 
                        BuffCarrierID = Unit.Entity,
                        BuffCreatorID = creatorId,
                    };
                    buff = new Buff(buffDataID, level, degree, trigInfo, createParams);
                    Unit from = Unit.Get(sourceId);
                    Debug.Log($"{from?.ToString() ?? "未知目标"} 对 {Unit} 添加Buff(lv.{level} {buffDataID}|{degree}层|{createParams[0]})");
                    AddBuff(buff);
                }
            }
            return buff;
        }

        public Buff InitAdd(string buffDataID, int level, int degree, params float[] createParams)
        {
            return AddBuff(buffDataID, Unit.Entity, Unit.Entity, level, degree, createParams);
        }

        public bool IsBuffExist(string buffID, EcsPackedEntity sourceId)
        {
            foreach (Buff buff in Buffs)
            {
                if (buff.ID == buffID && (sourceId == EcsPackedEntity.Empty || sourceId == buff.SourceID))
                {
                    return true;
                }
            }
            return false;
        }
        public List<int> GetBuffsByID(string buffID, EcsPackedEntity sourceId)
        {
            BuffNode node = IData<BuffNode>.Get(buffID);
            bool uniq = node.CompeteType != CompeteType.Indie;
            List<int> buffs = new ();
            foreach (Buff buff in Buffs)
            {
                if (buff.ID == buffID && (sourceId == EcsPackedEntity.Empty || sourceId == buff.SourceID))
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

        public bool TryGetBuffsByID(string buffID, EcsPackedEntity sourceId, out Buff buff)
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
            buff.TrigInfo.BuffCarrierID = Unit.Entity;
            EcsPackedEntity sourceId = buff.SourceID;
            if (buff.BuffData.CompeteType != CompeteType.Indie)
            {
                if (!UniqBuffs.TryGetValue(buff.BuffData.ID, out Dictionary<EcsPackedEntity, int> byId))
                {
                    byId = new();
                    UniqBuffs.Add(buff.BuffData.ID, byId);
                }
                byId[sourceId] = buff.Index;
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
                BuffPeriodicSystem.TimeWheel.Add(trig.TrigTickDelay, trig.Index);
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
                Dictionary<EcsPackedEntity, int> buffById = UniqBuffs[buff.BuffData.ID];
                buffById.Remove(buff.SourceID);
            }
            Buffs.Remove(buff);
            foreach (BuffTrig trig in buff.Trigs)
            {
                if (trig.TrigNode.TrigType > TrigType.ByTime_4)//过滤初始化
                {
                    TrigDic[trig.TrigNode.CombinedTrigType].Remove(trig);
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
