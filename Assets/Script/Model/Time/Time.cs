using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    public class Time : Singleton<Time>
    {
        public const int GameTickPerSec = 40;
        public const float GameTickDuration = 1f / GameTickPerSec;
        public static int Tick => Inst.currentGameTick;
        public static void StepNext() => Inst.currentGameTick++;
        public static ref bool Pause => ref Inst.pause;

        bool pause;
        int currentGameTick = -1;


    }
}
