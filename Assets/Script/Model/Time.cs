using UnityEngine;

namespace SkillEditorDemo
{
    public class Time
    {
        public const int FramePerSec = 40;
        public const float FrameDuration = 1f / FramePerSec;
        public static int CurrentFrame => Frame;
        static int Frame = 0;
        public static void StepNext() => Frame++;
    }
}
