using System;

namespace SkillEditorDemo.Model
{
    public static class MainThread
    {
        public static System.Threading.SynchronizationContext Content;

        public static void Post(Action action)
        {
            Content?.Post(_ => action(), null);
        }
    }
}
