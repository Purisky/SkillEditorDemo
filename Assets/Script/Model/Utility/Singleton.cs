using UnityEngine;

namespace SkillEditorDemo
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly T instance = new();
        static Singleton() { }
        protected Singleton() { Init(); }
        public static T Instance => instance;
        public virtual void Init() { }
    }
}
