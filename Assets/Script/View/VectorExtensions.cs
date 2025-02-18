using UnityEngine;
using System.Numerics;
using SkillEditorDemo.Model;

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

    public static class TransformExtensions
    {
        public static void SetTransform(this Transform transform, TransformCmp cmp)
        {
            transform.SetPositionAndRotation(new UnityEngine.Vector3(cmp.Pos.X, 0, cmp.Pos.Y), UnityEngine.Quaternion.Euler(0, cmp.Rot, 0));
        }
    }

}
