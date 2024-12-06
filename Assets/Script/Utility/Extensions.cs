using UnityEngine;

namespace SkillEditorDemo
{
    public static partial class Extensions
    {
        public static T GetValueOrDefault<T>(this T[] array, int index)
        {
            return GetValueOrDefault(array, index, default)!;
        }
        public static T GetValueOrDefault<T>(this T[] array, int index, T defaultValue)
        {
            return array.ContainsIndex(index) ? array[index] : defaultValue;
        }
        public static bool ContainsIndex<T>(this T[] array, int index) => 0 <= index && index < array.Length;
    }
}
