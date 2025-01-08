using System;

namespace SkillEditorDemo.Utility
{
    public class FixRandom
    {
        static Random random = new();

        public static float Get01() => (float)random.NextDouble();

        public static int Range(int min, int max) => random.Next(min, max);

        public static float Range(float min, float max) => min + (max - min) * Get01();

    }
}
