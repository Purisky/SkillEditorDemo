using System;
using System.Collections.Generic;
using System.Numerics;

namespace SkillEditorDemo.Model
{
    public struct ColliderCmp
    {
        public IAABB Shape;
        public ColliderType Type;
        public ColliderType ColliderToType;
        public int Faction;//-1=>Terrain


        public static ColliderType DftColliderTo(ColliderType type)
        {
            return type switch
            {
                ColliderType.Unit => ColliderType.Hitbox | ColliderType.Projectile,
                ColliderType.Hitbox => ColliderType.Unit,
                ColliderType.Terrain => ColliderType.Projectile,
                ColliderType.Projectile => ColliderType.Unit | ColliderType.Terrain,
                _ => ColliderType.None
            };
        }

        public bool IsColliderTo(ref ColliderCmp other)
        {
            if (!(ColliderToType.HasFlag(other.Type) && other.ColliderToType.HasFlag(Type)))
            {
                return false;
            }
            if (Shape is not Circle && other.Shape is not Circle) { return false; }
            if (Faction == -1 || other.Faction == -1 || Faction != other.Faction)// hostile to each other
            {
                return true;
            }
            return false;
        }


        public ColliderCmp(IAABB shape, int faction, ColliderType type, ColliderType colliderType)
        {
            Shape = shape;
            Type = type;
            Faction = faction;
            ColliderToType = colliderType;
        }
        public ColliderCmp(IAABB shape, int faction, ColliderType type)
        {
            Shape = shape;
            Type = type;
            Faction = faction;
            ColliderToType = DftColliderTo(type);
        }
    }
    [Flags]
    public enum ColliderType
    {
        None,
        Hitbox = 1,
        Projectile = 2,
        Unit = 4,
        Terrain = 8,
    }




    public struct Circle:IAABB
    {
        public float Radius;
        public AABB GetAABB(ref TransformCmp transform)
        {
            return new AABB
            {
                Center = transform.Pos,
                Size = new Vector2(Radius * 2, Radius * 2)
            };
        }
        public AABB AABB { get; set; }
        public AABB oldAABB { get; set; }
        public bool Dirty { get; set; }
        public int Entity { get; set; }


        public bool IsColliderTo(in TransformCmp transform, in IAABB other, in TransformCmp otherTransform)
        { 
            if (other is Circle circle)
            {
                return IsColliderTo(transform, circle, otherTransform);
            }
            if (other is Rectangle rectangle)
            {
                return IsColliderTo(transform, rectangle, otherTransform);
            }
            if (other is Sector sector)
            {
                return IsColliderTo(transform, sector, otherTransform);
            }
            return false;
        }




        public bool IsColliderTo(in TransformCmp transform, in Circle other, in TransformCmp otherTransform)
        {
            float distanceSquared = Vector2.DistanceSquared(transform.Pos, otherTransform.Pos);
            float radiusSum = Radius + other.Radius;
            return distanceSquared < (radiusSum * radiusSum);
        }
        public bool IsColliderTo(in TransformCmp transform, in Rectangle other, in TransformCmp otherTransform)
        {
            Vector2 circleCenter = transform.Pos;
            Vector2 rectCenter = otherTransform.Pos;
            Vector2 rectHalfSize = other.Size / 2;

            // Rotate circle's center point back
            Vector2 rotatedCircleCenter = Vector2.Transform(circleCenter - rectCenter, Matrix3x2.CreateRotation(-otherTransform.Rot.Radian)) + rectCenter;

            // Closest point on the rectangle to the circle's center
            Vector2 closestPoint = new Vector2(
                MathF.Max(rectCenter.X - rectHalfSize.X, MathF.Min(rotatedCircleCenter.X, rectCenter.X + rectHalfSize.X)),
                MathF.Max(rectCenter.Y - rectHalfSize.Y, MathF.Min(rotatedCircleCenter.Y, rectCenter.Y + rectHalfSize.Y))
            );

            // Distance from the circle's center to the closest point
            float distanceSquared = Vector2.DistanceSquared(rotatedCircleCenter, closestPoint);

            return distanceSquared < (Radius * Radius);


        }
        public bool IsColliderTo(in TransformCmp transform, in Sector other, in TransformCmp otherTransform)
        {
            Vector2 circleCenter = transform.Pos;
            Vector2 sectorCenter = otherTransform.Pos;
            float sectorRadius = other.Radius;
            float sectorAngle = other.Angle;

            // Check if the circle's center is within the sector's radius
            if (Vector2.DistanceSquared(circleCenter, sectorCenter) > (sectorRadius + Radius) * (sectorRadius + Radius))
            {
                return false;
            }

            // Calculate the angle between the sector's center and the circle's center
            Vector2 direction = Vector2.Normalize(circleCenter - sectorCenter);
            float angleToCircle = MathF.Atan2(direction.Y, direction.X) * 180 / MathF.PI;

            // Normalize angles
            float sectorStartAngle = (otherTransform.Rot.Degree - sectorAngle / 2 + 360) % 360;
            float sectorEndAngle = (otherTransform.Rot.Degree + sectorAngle / 2 + 360) % 360;
            angleToCircle = (angleToCircle + 360) % 360;

            // Check if the angle is within the sector's angle range
            bool withinAngleRange;
            if (sectorStartAngle < sectorEndAngle)
            {
                withinAngleRange = angleToCircle >= sectorStartAngle && angleToCircle <= sectorEndAngle;
            }
            else
            {
                withinAngleRange = angleToCircle >= sectorStartAngle || angleToCircle <= sectorEndAngle;
            }

            if (withinAngleRange)
            {
                return true;
            }

            // Check collision with the sector's straight edges
            Vector2 startEdge = sectorCenter + new Vector2(sectorRadius * MathF.Cos(sectorStartAngle * MathF.PI / 180), sectorRadius * MathF.Sin(sectorStartAngle * MathF.PI / 180));
            Vector2 endEdge = sectorCenter + new Vector2(sectorRadius * MathF.Cos(sectorEndAngle * MathF.PI / 180), sectorRadius * MathF.Sin(sectorEndAngle * MathF.PI / 180));

            return IsCircleIntersectingLine(circleCenter, Radius, sectorCenter, startEdge) || IsCircleIntersectingLine(circleCenter, Radius, sectorCenter, endEdge);
        }

        private bool IsCircleIntersectingLine(Vector2 circleCenter, float radius, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 lineDir = lineEnd - lineStart;
            Vector2 toCircle = circleCenter - lineStart;
            float t = Vector2.Dot(toCircle, lineDir) / lineDir.LengthSquared();
            t = MathF.Max(0, MathF.Min(1, t));
            Vector2 closestPoint = lineStart + t * lineDir;
            return Vector2.DistanceSquared(circleCenter, closestPoint) <= radius * radius;
        }

    }

    public struct Rectangle : IAABB
    {
        public Vector2 Size;
        public AABB GetAABB(ref TransformCmp transform)
        {
            Angle Angle = transform.Rot;
            Vector2 Center = transform.Pos;

            Vector2 halfSize = Size / 2;
            Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(Angle.Radian);

            Vector2[] corners = new Vector2[4];
            corners[0] = Center + Vector2.Transform(-halfSize, rotationMatrix);
            corners[1] = Center + Vector2.Transform(new Vector2(halfSize.X, -halfSize.Y), rotationMatrix);
            corners[2] = Center + Vector2.Transform(halfSize, rotationMatrix);
            corners[3] = Center + Vector2.Transform(new Vector2(-halfSize.X, halfSize.Y), rotationMatrix);

            Vector2 min = corners[0];
            Vector2 max = corners[0];

            for (int i = 1; i < corners.Length; i++)
            {
                min = Vector2.Min(min, corners[i]);
                max = Vector2.Max(max, corners[i]);
            }

            return new AABB
            {
                Center = (min + max) / 2,
                Size = max - min
            };
        }
        public AABB AABB { get; set; }
        public AABB oldAABB { get; set; }
        public bool Dirty { get; set; }
        public int Entity { get; set; }
    }

    public struct Sector : IAABB
    {
        public float Radius;
        public float Angle; // in degrees
        public AABB GetAABB(ref TransformCmp transform)
        {
            List<Vector2> points = new();
            Angle CenterAngle = transform.Rot;
            Vector2 Center = transform.Pos;

            Angle startAngle = CenterAngle.Degree - Angle / 2;
            Angle endAngle = CenterAngle.Degree + Angle / 2;
            // 扇形的三个关键点
            points.Add(Center);
            points.Add(Center + new Vector2(Radius * System.MathF.Cos(startAngle.Radian), Radius * System.MathF.Sin(startAngle.Radian)));
            points.Add(Center + new Vector2(Radius * System.MathF.Cos(endAngle.Radian), Radius * System.MathF.Sin(endAngle.Radian)));

            // 检查是否包含0度、90度、180度、270度
            if (Angle >= 360 || (startAngle <= 0 && endAngle >= 0) || (startAngle <= 360 && endAngle >= 360))
            {
                points.Add(Center + new Vector2(0, Radius));
            }
            if (Angle >= 360 || (startAngle <= 90 && endAngle >= 90))
            {
                points.Add(Center + new Vector2(Radius, 0));
            }
            if (Angle >= 360 || (startAngle <= 180 && endAngle >= 180))
            {
                points.Add(Center + new Vector2(0, -Radius));
            }
            if (Angle >= 360 || (startAngle <= 270 && endAngle >= 270))
            {
                points.Add(Center + new Vector2(-Radius, 0));
            }

            // 计算最小和最大坐标
            Vector2 min = points[0];
            Vector2 max = points[0];

            foreach (var point in points)
            {
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }

            return new AABB
            {
                Center = (min + max) / 2,
                Size = max - min
            };
        }
        public AABB AABB { get; set; }
        public AABB oldAABB { get; set; }
        public bool Dirty { get; set; }
        public int Entity { get; set; }
    }

    public interface IAABB
    {
        int Entity { get; set; }
        AABB GetAABB(ref TransformCmp transform);
        AABB AABB { get; set; }
        AABB oldAABB { get; set; }
        bool Dirty { get; set; }
    }

    public static class AABBExtensions
    { 
        public static void SetDirty(this IAABB aabb, ref TransformCmp transform)
        {
            aabb.oldAABB = aabb.AABB;
            aabb.AABB = aabb.GetAABB(ref transform);
            aabb.Dirty = true;
        }
    }

    public struct AABB
    {
        public Vector2 Center;
        public Vector2 Size;
        public override string ToString()
        {
            return $"{Center}[{Size}]";
        }
    }

    public struct Angle
    {
        const float Degree2Radian = MathF.PI / 180;
        const float Radian2Degree = 180 / MathF.PI;
        public float Degree;// in degrees (0,1)=>0, (1,0)=>90, (0,-1)=>180, (-1,0)=>270
        public readonly float Radian => Degree * Degree2Radian;
        public Angle(float degree)
        {
            Degree = (degree % 360 +360) % 360;
        }
        public readonly Vector2 GetVector()
        {
            float radian = MathF.PI/ 2 -Radian;
            float x = MathF.Cos(radian);
            x = (int)(x * 1000000) / 1000000f;
            float y = MathF.Sin(radian);
            y = (int)(y * 1000000) / 1000000f;
            return new Vector2(x, y);
        }

        public static Angle operator +(Angle a, Angle b)
        {
            return new(a.Degree + b.Degree);
        }
        public static Angle operator -(Angle a, Angle b)
        {
            return new(a.Degree - b.Degree);
        }
        public static Angle operator *(Angle a, float b)
        {
            return new (a.Degree * b);
        }
        public static Angle operator /(Angle a, float b)
        {
            return new  (a.Degree / b) ;
        }
        public static bool operator ==(Angle a, Angle b)
        {
            return a.Degree == b.Degree;
        }
        public static bool operator !=(Angle a, Angle b)
        {
            return a.Degree != b.Degree;
        }
        public override readonly bool Equals(object obj)
        {
            return obj is Angle degree &&
                   Degree == degree.Degree;
        }
        public override int GetHashCode()
        {
            return -1937169414 + Degree.GetHashCode();
        }
        public static Angle Up => new (0) ;
        public static Angle Right => new(90) ;
        public static Angle Down => new(180) ;
        public static Angle Left => new(270) ;

        public static implicit operator Angle(float angle)
        {
            return new(angle);
        }
        public static implicit operator float(Angle degree)
        {
            return degree.Degree;
        }
        public static implicit operator Angle(Vector2 vector)
        {
            return new(90-Radian2Degree * MathF.Atan2(vector.Y, vector.X));
        }
        public static implicit operator Vector2(Angle degree)
        {
            return degree.GetVector();
        }
        public override readonly string ToString()
        {
            return $"{Degree}=>{GetVector()}";
        }
    }


}
