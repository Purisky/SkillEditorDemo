using UnityEngine;

namespace SkillEditorDemo.Utility
{
    public class FixRandom
    {
        public static float Get01() => Random.value;

        public static int Range(int min, int max) => Random.Range(min, max);
    }
}
