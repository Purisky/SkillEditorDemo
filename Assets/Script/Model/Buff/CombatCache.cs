using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SkillEditorDemo
{
    public class CombatCache
    {
        public float[] Caches = new float[(int)CombatCacheType.Count];
        public bool DirectDmg;
        public bool NeedCheckDodge;
        public bool NeedCheckCrit;
        public bool IsCrit;


        public static TempData _ = new ();
        public float this[CombatCacheType type]
        {
            get => Caches[(int)type];
            set => Caches[(int)type] = value;
        }

        public CombatCache Clone()
        {
            CombatCache cache = new ();
            for (int i = 0; i < Caches.Length; i++)
            {
                cache.Caches[i] = Caches[i];
            }
            cache.DirectDmg = DirectDmg;
            cache.NeedCheckDodge = NeedCheckDodge;
            cache.NeedCheckCrit = NeedCheckCrit;
            cache.IsCrit = IsCrit;
            return cache;
        }




    }
}
