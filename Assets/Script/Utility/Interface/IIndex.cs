using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillEditorDemo.Utility
{

    public interface IData
    {
        string ID { get; set; }
    }
    public interface IData<T> : IData where T : IData
    {
        public static Dictionary<string, T> DataDic { get; set; } = new();
        public static T Get(string id) => DataDic.GetValueOrDefault(id);
        public static void Add(string index, T value)
        {
            DataDic[index] = value;
        }
    }
    public interface IGrowID
    {
        [JsonIgnore]
        int GrowID { get; set; }
    }
    public interface IGrowID<T> : IGrowID where T : IGrowID
    {
        static T[] values = new T[8];
        static int index = -1;
        public static T Get(int id) => values.GetValueOrDefault(id);
        public static void Add(T value)
        {
            index++;
            if (index >= values.Length)
            {
                Array.Resize(ref values, values.Length << 1);
            }
            value.GrowID = index;
            values[index] = value;
        }
    }



    public interface IIndex
    {
        [JsonIgnore]
        int Index { get; set; }
    }
    public interface IIndex<T> : IIndex where T : class, IIndex
    {
        public static Dictionary<int, T> Dic { get; set; } = new ();
        static int Count { get;private set; } = -1;
        public static int NewIndex() => ++Count;
        public static T Get(int id) => Dic!.GetValueOrDefault(id, null)!;
        public static int Add(T t)
        {
            t.Index = NewIndex();
            Dic.Add(t.Index, t);
            return t.Index;
        }
        public static void Remove(int index)
        {
            Dic.Remove(index);
        }
    }
    public interface IIndexData<T, TData> : IIndex<T>, IData<TData> where T : class, IIndex where TData : class, IData
    {
    }
}
