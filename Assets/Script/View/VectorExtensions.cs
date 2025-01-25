using UnityEngine;
using System.Numerics;

namespace SkillEditorDemo.View
{
    public static class VectorExtensions
    {
        public static System.Numerics.Vector2 ToNumerics(this UnityEngine.Vector2 uVec2)
        {
            return new System.Numerics.Vector2(uVec2.x, uVec2.y);
        }
        public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 nVec2)
        {
            return new UnityEngine.Vector2(nVec2.X, nVec2.Y);
        }
    }
}
