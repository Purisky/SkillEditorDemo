using UnityEngine;

namespace SkillEditorDemo
{
    public static class Time
    {
        public const int GameTickPerSec = 40;
        public const float GameTickDuration = 1f / GameTickPerSec;
        public static int Tick => currentGameTick;
        static int currentGameTick = 0;
        public static void StepNext() => currentGameTick++;
    }
}
