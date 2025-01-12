using UnityEngine;
using SkillEditorDemo.Model;

public class CircleShape : MonoBehaviour
{
    public float radius = 1f;
    public int entity = 1;
    public Color defaultColor = Color.red;
    public Color collisionColor = Color.magenta;

    private Circle circle;
    private TransformCmp transformCmp;
    private Color currentColor;

    void Start()
    {
        circle = new Circle { Radius = radius, Entity = entity };
        transformCmp = new TransformCmp { Pos = new System.Numerics.Vector2(transform.position.x, transform.position.y), Rot = new Angle(transform.eulerAngles.z) };
        currentColor = defaultColor;
    }

    void Update()
    {
        transformCmp.Pos = new System.Numerics.Vector2(transform.position.x, transform.position.y);
        transformCmp.Rot = new Angle(transform.eulerAngles.z);
        circle.Radius = radius;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = currentColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public Circle GetCircle()
    {
        return circle;
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