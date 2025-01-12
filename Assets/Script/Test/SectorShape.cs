using UnityEngine;
using SkillEditorDemo.Model;

public class SectorShape : MonoBehaviour
{
    public float radius = 2f;
    public float angle = 90f;
    public int entity = 3;
    public Color defaultColor = Color.yellow;
    public Color collisionColor = Color.white;

    private Sector sector;
    private TransformCmp transformCmp;
    private Color currentColor;

    void Start()
    {
        sector = new Sector { Radius = radius, Angle = angle, Entity = entity };
        transformCmp = new TransformCmp { Pos = new System.Numerics.Vector2(transform.position.x, transform.position.y), Rot = new Angle(transform.eulerAngles.z) };
        currentColor = defaultColor;
    }

    void Update()
    {
        transformCmp.Pos = new System.Numerics.Vector2(transform.position.x, transform.position.y);
        transformCmp.Rot = new Angle(transform.eulerAngles.z);
        sector.Radius = radius;
        sector.Angle = angle;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = currentColor;
        Vector3 center = transform.position;
        Vector3 startDir = Quaternion.Euler(0, 0, transform.eulerAngles.z - angle / 2) * Vector3.right;
        Vector3 endDir = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle / 2) * Vector3.right;

        Gizmos.DrawLine(center, center + startDir * radius);
        Gizmos.DrawLine(center, center + endDir * radius);

        int segments = 20;
        float deltaAngle = angle / segments;
        Vector3 prevPoint = center + startDir * radius;
        for (int i = 1; i <= segments; i++)
        {
            Vector3 nextPoint = center + Quaternion.Euler(0, 0, deltaAngle * i) * startDir * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    public Sector GetSector()
    {
        return sector;
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