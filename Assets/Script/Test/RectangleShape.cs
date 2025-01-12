using UnityEngine;
using SkillEditorDemo.Model;

public class RectangleShape : MonoBehaviour
{
    public Vector2 size = new Vector2(2f, 1f);
    public int entity = 2;
    public Color defaultColor = Color.green;
    public Color collisionColor = Color.cyan;

    private Rectangle rectangle;
    private TransformCmp transformCmp;
    private Color currentColor;

    void Start()
    {
        rectangle = new Rectangle { Size = new System.Numerics.Vector2(size.x, size.y), Entity = entity };
        transformCmp = new TransformCmp { Pos = new System.Numerics.Vector2(transform.position.x, transform.position.y), Rot = new Angle(transform.eulerAngles.z) };
        currentColor = defaultColor;
    }

    void Update()
    {
        transformCmp.Pos = new System.Numerics.Vector2(transform.position.x, transform.position.y);
        transformCmp.Rot = new Angle(transform.eulerAngles.z);
        rectangle.Size = new System.Numerics.Vector2(size.x, size.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = currentColor;
        Vector2 halfSize = size / 2;
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, transform.eulerAngles.z));
        Vector3[] corners = new Vector3[4];
        corners[0] = rotationMatrix.MultiplyPoint3x4(new Vector3(-halfSize.x, -halfSize.y, 0)) + transform.position;
        corners[1] = rotationMatrix.MultiplyPoint3x4(new Vector3(halfSize.x, -halfSize.y, 0)) + transform.position;
        corners[2] = rotationMatrix.MultiplyPoint3x4(new Vector3(halfSize.x, halfSize.y, 0)) + transform.position;
        corners[3] = rotationMatrix.MultiplyPoint3x4(new Vector3(-halfSize.x, halfSize.y, 0)) + transform.position;

        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);
    }

    public Rectangle GetRectangle()
    {
        return rectangle;
    }

    public TransformCmp GetTransformCmp()
    {
        return transformCmp;
    }

    public void SetCollisionState(bool isColliding)
    {
        currentColor = isColliding ? collisionColor : defaultColor;
    }
}