using System.Collections.Generic;
using System.Numerics;

namespace SkillEditorDemo.Model
{
    public struct TransformCmp
    {
        public Vector2 Pos;
        public Angle Rot;



        public static TransformCmp operator +(TransformCmp a, TransformCmp b)
        {
            return new TransformCmp()
            {
                Pos = a.Pos + b.Pos,
                Rot = a.Rot + b.Rot
            };
        }
    }

    public struct VelocityCmp
    {
        public Vector2 Speed;//per sec
        public Angle Rot;//per sec
    }

    public struct Collision : IEqualityComparer<Collision>
    {
        public int EntityA;
        public int EntityB;
        public ColliderType HitType;
        public Collision(ColliderCmp x, ColliderCmp y)
        {
            if (x.Type < y.Type)
            {
                EntityA = x.Shape.Entity;
                EntityB = y.Shape.Entity;
            }
            else
            {
                EntityA = y.Shape.Entity;
                EntityB = x.Shape.Entity;
            }
            HitType = x.Type | y.Type;
        }
        public Collision(int entityA, int entityB)
        {
            EntityA = entityA;
            EntityB = entityB;
            HitType = ColliderType.None;
        }
        public bool Equals(Collision x, Collision y)
        {
            return x.EntityA == y.EntityA && x.EntityB == y.EntityB;
        }
        public int GetHashCode(Collision obj)
        {
            return obj.EntityA.GetHashCode() ^ obj.EntityB.GetHashCode();
        }
    }
}
