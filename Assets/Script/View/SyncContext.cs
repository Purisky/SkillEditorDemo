using SkillEditorDemo.Model;
using UnityEngine;

namespace SkillEditorDemo.View
{
    public static class SyncContext
    {
        public static System.Threading.SynchronizationContext Content;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeInit()
        {
            MainThread.Content = System.Threading.SynchronizationContext.Current;
        }

    }
}
