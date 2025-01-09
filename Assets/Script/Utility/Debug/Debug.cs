using System;
using TreeNode.Utility;
namespace SkillEditorDemo.Utility
{
    public class Debug : Singleton<Debug>
    {
        Action<string> LogAction;
        Action<string> Error;
        public static void Log(string message)
        {
            Inst.LogAction?.Invoke(message);
        }
        public static void Log(object message)
        {
            Inst.LogAction?.Invoke(message.ToString());
        }
        public static void LogError(string message)
        {
            Inst.Error?.Invoke(message);
        }

        public static void Init(Action<string> log, Action<string> error)
        {
            Inst.LogAction = log;
            Inst.Error = error;
        }

    }
}
