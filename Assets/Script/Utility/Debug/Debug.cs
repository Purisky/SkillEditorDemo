using System;
namespace SkillEditorDemo.Utility
{
    public class Debug
    {
        public static Action<string> LogAction;
        public static Action<string> Error;
        public static void Log(string message)
        {
            LogAction?.Invoke(message);
        }
        public static void LogError(string message)
        {
            Error?.Invoke(message);
        }

        public static void Init(Action<string> log, Action<string> error)
        {
            LogAction = log;
            Error = error;
        }

    }
}
