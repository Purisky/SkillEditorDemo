using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SkillEditorDemo.Utility
{
    public class ObjPool<T> where T : new()
    {
        protected readonly Stack<T> stack = new();
        protected readonly Action<T> actionOnGet;
        protected readonly Action<T> actionOnRelease;
        protected readonly HashSet<T> activeSet;
        protected readonly Func<T> createFunc;
        public int CountAll { get; private set; }
        public int CountActive { get { return CountAll - CountInactive; } }
        public int CountInactive { get { return stack.Count; } }
        protected virtual T New() => new();
        public ObjPool(Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Func<T> createFunc = null)
        {
            this.actionOnGet = actionOnGet;
            this.actionOnRelease = actionOnRelease;
            this.createFunc = createFunc ?? New;
            activeSet = new HashSet<T>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get()
        {
            T element;
            if (stack.Count == 0)
            {
                element = createFunc();
                CountAll++;
            }
            else
            {
                element = stack.Pop();
            }
            actionOnGet?.Invoke(element);
            activeSet.Add(element);
            return element;
        }

        public void Release(T element)
        {
#if DEBUG
            if (stack.Count > 0 && ReferenceEquals(stack.Peek(), element))
            {
                Debug.Log(string.Format($"[<color=red>尝试回收已在池中的对象{element}</color>]"));
            }
#endif
            actionOnRelease?.Invoke(element);
            stack.Push(element);
            activeSet?.Remove(element);
        }

        public void ReleaseAll()
        {
            while (activeSet.Count > 0)
            {
                Release(activeSet.First());
            }
        }

    }
}
