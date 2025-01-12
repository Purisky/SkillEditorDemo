using UnityEngine;
using SkillEditorDemo.Model;

public class CollisionTest : MonoBehaviour
{
    public CircleShape circleShape1;
    public CircleShape circleShape2;
    public RectangleShape rectangleShape;
    public SectorShape sectorShape;

    void Update()
    {
        if (circleShape1 != null && circleShape2 != null)
        {
            Circle circle1 = circleShape1.GetCircle();
            TransformCmp transform1 = circleShape1.GetTransformCmp();
            Circle circle2 = circleShape2.GetCircle();
            TransformCmp transform2 = circleShape2.GetTransformCmp();

            bool isColliding = circle1.IsColliderTo(transform1, circle2, transform2);
            circleShape1.SetCollisionState(isColliding);
            circleShape2.SetCollisionState(isColliding);
        }

        if (circleShape1 != null && rectangleShape != null)
        {
            Circle circle = circleShape1.GetCircle();
            TransformCmp circleTransform = circleShape1.GetTransformCmp();
            Rectangle rect = rectangleShape.GetRectangle();
            TransformCmp rectTransform = rectangleShape.GetTransformCmp();

            bool isColliding = circle.IsColliderTo(circleTransform, rect, rectTransform);
            circleShape1.SetCollisionState(isColliding);
            rectangleShape.SetCollisionState(isColliding);
        }

        if (circleShape1 != null && sectorShape != null)
        {
            Circle circle = circleShape1.GetCircle();
            TransformCmp circleTransform = circleShape1.GetTransformCmp();
            Sector sector = sectorShape.GetSector();
            TransformCmp sectorTransform = sectorShape.GetTransformCmp();

            bool isColliding = circle.IsColliderTo(circleTransform, sector, sectorTransform);
            circleShape1.SetCollisionState(isColliding);
            sectorShape.SetCollisionState(isColliding);
        }
    }
}