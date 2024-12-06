using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace SkillEditorDemo
{
    /// <summary>
    /// 共轭字典
    /// </summary>
    /// <typeparam name="TKeyF"></typeparam>
    /// <typeparam name="TKeyI"></typeparam>
    public class ConjugateDic<TKeyF, TKeyI>
    {
        readonly Dictionary<TKeyF, TKeyI> forward;
        readonly Dictionary<TKeyI, TKeyF> inverse;
        public int Count => forward.Count;
        public ConjugateDic()
        {
            if (typeof(TKeyF) == typeof(TKeyI))
            {
                throw new Exception($"use {typeof(ConjugateDic<TKeyF>)} instead");
            }
            forward = new Dictionary<TKeyF, TKeyI>();
            inverse = new Dictionary<TKeyI, TKeyF>();
        }
        public void Add(TKeyF kf, TKeyI ki)
        {
            if (!forward.ContainsKey(kf) && !inverse.ContainsKey(ki))
            {
                forward.Add(kf, ki);
                inverse.Add(ki, kf);
            }
        }

        public void Remove(TKeyF key)
        {
            if (forward.TryGetValue(key, out var value))
            {
                forward.Remove(key);
                inverse.Remove(value);
            }
        }
        public void Remove(TKeyI key)
        {
            if (inverse.TryGetValue(key, out var value))
            {
                forward.Remove(value);
                inverse.Remove(key);
            }
        }
        public void Clear()
        {
            forward.Clear();
            inverse.Clear();
        }

        public TKeyI GetValueOrDefault(TKeyF key) => forward.GetValueOrDefault(key);
        public bool TryGet(TKeyI value, out TKeyF key) => inverse.TryGetValue(value, out key);
        public bool TryGet(TKeyF key, out TKeyI value) => forward.TryGetValue(key, out value);
        public TKeyI this[TKeyF key] => forward[key];
        public TKeyF this[TKeyI key] => inverse[key];

        public Dictionary<TKeyF, TKeyI>.KeyCollection KeyFs => forward.Keys;
        public Dictionary<TKeyI, TKeyF>.KeyCollection KeyIs => inverse.Keys;
        public bool Exist(TKeyI value) => inverse.ContainsKey(value);
        public bool Exist(TKeyF key) => forward.ContainsKey(key);

        public void AddOrSet(TKeyF key, TKeyI value)
        {
            if (Exist(key))
            {
                Remove(key);
            }
            Add(key, value);
        }

        public override string ToString()
        {
            string code = "";
            foreach (var item in forward)
            {
                code += $"\n{item.Key}:{item.Value}";
            }
            return code;
        }


    }
    /// <summary>
    /// 共轭字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class ConjugateDic<TKey>
    {
        Dictionary<TKey, TKey> dic;
        public ConjugateDic()
        {
            dic = new Dictionary<TKey, TKey>();
        }

        public void Add(TKey key, TKey value)
        {
            if (dic.ContainsKey(key) || dic.ContainsKey(value)) { return; }
            dic.Add(key, value);
            if (!key.Equals(value))
            {
                dic.Add(value, key);
            }
        }
        public TKey this[TKey key] => dic[key];

        public bool ContainsKey(TKey key) => dic.ContainsKey(key);
        public bool TryGetValue(TKey key, out TKey value) => dic.TryGetValue(key, out value);
        public TKey GetValueOrDefault(TKey key) => dic.GetValueOrDefault(key);
        public Dictionary<TKey, TKey>.KeyCollection Keys => dic.Keys;
        public void Clear() => dic.Clear(); 
    }


    /// <summary>
    /// 只增不减连续共轭字典
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConjugateGrowDic<T>
    {
        [JsonProperty]
        public T[] Arr;
        public Dictionary<T, int> Dic;
        [JsonProperty]
        int count;
        public int Add(T value)
        {
            if (!Dic.ContainsKey(value))
            {
                if (count == Arr.Length)
                {
                    Array.Resize(ref Arr, Arr.Length << 1);
                }
                count++;
                Arr[count - 1] = value;
                Dic.Add(value, count - 1);
                return count - 1;
            }
            return -1;
        }
        public T this[int index] => Arr.GetValueOrDefault(index);
        public int this[T value] => Dic.GetValueOrDefault(value, -1);

        public ConjugateGrowDic(int cap = 2)
        {
            Arr = new T[cap];
            Dic = new ();
        }
        public bool TryGetKey(T value, out int key) => Dic.TryGetValue(value, out key);
        public bool TryGetValue(int key, out T value)
        {
            if (key < Arr.Length && key >= 0)
            {
                value = Arr[key];
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

    }
}
