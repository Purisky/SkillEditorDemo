using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SkillEditorDemo
{
    //todo gc optimize
    public class CombatCache
    {
        const int Count = (int)CombatCacheType.Count;
        public float[] Caches = new float[Count];
        public DmgType DmgType;
        public bool DirectDmg;
        public bool Dodge_able;
        public bool Crit_able;
        public bool IsCrit;
        public static CombatCache _ = new ();
        public ref float this[CombatCacheType type] => ref Caches[(int)type];
        public CombatCache Clone()
        {
            CombatCache cache = new ();
            for (int i = 0; i < Caches.Length; i++)
            {
                cache.Caches[i] = Caches[i];
            }
            cache.DirectDmg = DirectDmg;
            cache.Dodge_able = Dodge_able;
            cache.Crit_able = Crit_able;
            cache.IsCrit = IsCrit;
            return cache;
        }
    }
}
