using SkillEditorDemo.Model;
using UnityEngine;

namespace SkillEditorDemo.View
{
    public static class SyncContext
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeInit()
        {
            MainThread.Content = System.Threading.SynchronizationContext.Current;
        }

    }
}
