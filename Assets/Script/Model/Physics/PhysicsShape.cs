using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using TreeNode.Utility;

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

            // Quick distance check - if too far away, no collision possible
            float distanceToSectorCenter = Vector2.Distance(circleCenter, sectorCenter);
            if (distanceToSectorCenter > sectorRadius + Radius)
            {
                return false;
            }

            // Convert angles to match our coordinate system (0 degree = up, clockwise positive)
            Vector2 toCircle = circleCenter - sectorCenter;
            float angleToCircle = MathF.Atan2(toCircle.X, toCircle.Y) * 180f / MathF.PI;
            
            // Normalize angle to [0, 360)
            angleToCircle = (angleToCircle + 360f) % 360f;
            
            // Calculate sector start and end angles
            float sectorStartAngle = (otherTransform.Rot.Degree - sectorAngle * 0.5f + 360f) % 360f;
            float sectorEndAngle = (otherTransform.Rot.Degree + sectorAngle * 0.5f + 360f) % 360f;

            // Check if angle is within sector range
            bool withinAngleRange = IsAngleInRange(angleToCircle, sectorStartAngle, sectorEndAngle);

            if (withinAngleRange)
            {
                // Circle is within sector angle range
                if (distanceToSectorCenter <= sectorRadius - Radius)
                {
                    // Circle is completely inside sector
                    return true;
                }
                else if (distanceToSectorCenter <= sectorRadius + Radius)
                {
                    // Circle intersects with sector (either inside or crossing the arc)
                    return true;
                }
            }
            else
            {
                // Circle is outside angle range, check collision with sector edges and arc
                
                // Check collision with the two straight edges
                Vector2 startEdgePoint = sectorCenter + sectorRadius * (otherTransform.Rot - sectorAngle * 0.5f).GetVector();
                Vector2 endEdgePoint = sectorCenter + sectorRadius * (otherTransform.Rot + sectorAngle * 0.5f).GetVector();

                if (IsCircleIntersectingLine(circleCenter, Radius, sectorCenter, startEdgePoint) ||
                    IsCircleIntersectingLine(circleCenter, Radius, sectorCenter, endEdgePoint))
                {
                    return true;
                }

                // Check collision with the arc (most important for fixing the arc collision issue!)
                return IsCircleIntersectingArc(circleCenter, Radius, sectorCenter, sectorRadius, sectorStartAngle, sectorEndAngle);
            }

            return false;
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

        /// <summary>
        /// Check if an angle is within the range [startAngle, endAngle], handling 360-degree wraparound
        /// </summary>
        private bool IsAngleInRange(float angle, float startAngle, float endAngle)
        {
            if (startAngle <= endAngle)
            {
                // Normal case: no wraparound
                return angle >= startAngle && angle <= endAngle;
            }
            else
            {
                // Wraparound case: range crosses 0 degrees
                return angle >= startAngle || angle <= endAngle;
            }
        }

        /// <summary>
        /// Check if a circle intersects with a circular arc (the curved edge of a sector)
        /// This is the key method to fix arc collision detection!
        /// </summary>
        private bool IsCircleIntersectingArc(Vector2 circleCenter, float circleRadius, Vector2 arcCenter, float arcRadius, float startAngle, float endAngle)
        {
            // Find the closest point on the arc to the circle center
            Vector2 toCircle = circleCenter - arcCenter;
            float distanceToArcCenter = toCircle.Length();

            if (distanceToArcCenter < 0.0001f)
            {
                // Circle center is at arc center
                return true;
            }

            // Calculate the angle of the closest point on the circle from arc center
            float angleToCircle = MathF.Atan2(toCircle.X, toCircle.Y) * 180f / MathF.PI;
            angleToCircle = (angleToCircle + 360f) % 360f;

            Vector2 closestPointOnArc;
            
            if (IsAngleInRange(angleToCircle, startAngle, endAngle))
            {
                // The closest point is on the arc itself
                closestPointOnArc = arcCenter + Vector2.Normalize(toCircle) * arcRadius;
            }
            else
            {
                // The closest point is one of the arc endpoints
                Vector2 startPoint = arcCenter + arcRadius * new Angle(startAngle).GetVector();
                Vector2 endPoint = arcCenter + arcRadius * new Angle(endAngle).GetVector();
                
                float distToStart = Vector2.DistanceSquared(circleCenter, startPoint);
                float distToEnd = Vector2.DistanceSquared(circleCenter, endPoint);
                
                closestPointOnArc = distToStart < distToEnd ? startPoint : endPoint;
            }

            // Check if the circle intersects with the closest point on the arc
            return Vector2.DistanceSquared(circleCenter, closestPointOnArc) <= circleRadius * circleRadius;
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
            points.Add(Center + startAngle.GetVector()*Radius);
            points.Add(Center + endAngle.GetVector()*Radius);
            //Debug.DrawLine(Center, points[1], Color.Blue, 5);
            //Debug.DrawLine(Center, points[2], Color.Blue, 5);
            //Debug.Log($"{startAngle}~{endAngle}");

            float start = startAngle.Degree;
            float end = endAngle.Degree;
            if (start > end)
            {
                start -= 360;
            }
            // 检查是否包含0度、90度、180度、270度
            if (Angle >= 360 || (start <= 0 && end >= 0) || (start <= 360 && end >= 360))
            {
                points.Add(Center + new Vector2(0, Radius));
            }
            if (Angle >= 360 || (start <= 90 && end >= 90))
            {
                points.Add(Center + new Vector2(Radius, 0));
            }
            if (Angle >= 360 || (start <= 180 && end >= 180))
            {
                points.Add(Center + new Vector2(0, -Radius));
            }
            if (Angle >= 360 || (start <= 270 && end >= 270))
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

        public bool IsColliderTo(in TransformCmp transform, in IAABB other, in TransformCmp otherTransform)
        {
            if (other is Circle circle)
            {
                return circle.IsColliderTo(otherTransform, this, transform);
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

        public bool IsColliderTo(in TransformCmp transform, in Rectangle other, in TransformCmp otherTransform)
        {
            // For sector-rectangle collision, we can approximate by checking:
            // 1. Rectangle corners against sector
            // 2. Sector arc and edges against rectangle
            
            // TODO: Implement proper sector-rectangle collision if needed
            // For now, return false as this is complex and may not be needed
            return false;
        }

        public bool IsColliderTo(in TransformCmp transform, in Sector other, in TransformCmp otherTransform)
        {
            // Sector-sector collision is complex
            // TODO: Implement if needed
            return false;
        }
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
            Draw(Color.Green, 5);
            return $"{Center}[{Size}]";
        }
        public void Draw(Color color, float time)
        { 
            Debug.DrawLine(Center + new Vector2(-Size.X / 2, -Size.Y / 2), Center + new Vector2(Size.X / 2, -Size.Y / 2), color, time);
            Debug.DrawLine(Center + new Vector2(Size.X / 2, -Size.Y / 2), Center + new Vector2(Size.X / 2, Size.Y / 2), color, time);
            Debug.DrawLine(Center + new Vector2(Size.X / 2, Size.Y / 2), Center + new Vector2(-Size.X / 2, Size.Y / 2), color, time);
            Debug.DrawLine(Center + new Vector2(-Size.X / 2, Size.Y / 2), Center + new Vector2(-Size.X / 2, -Size.Y / 2), color, time);
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
