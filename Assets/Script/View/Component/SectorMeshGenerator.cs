using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo.View
{
    public class SectorMeshGenerator
    {
        private static Vector3[] cachedVertices;
        private static int[] cachedTriangles;
        private const int maxSegments = 24;
        private const float radius = 0.5f;
        static Mesh CreateCircleMesh()
        {
            Mesh mesh = new Mesh();

            // 圆片的顶点
            int segments = 16;

            cachedVertices = new Vector3[segments + 1];
            cachedTriangles = new int[segments * 3];

            // 圆片顶点
            cachedVertices[0] = Vector3.zero;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                cachedVertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            }

            // 圆片三角形
            for (int i = 0; i < segments; i++)
            {
                cachedTriangles[i * 3] = 0;
                cachedTriangles[i * 3 + 1] = (i + 1) % segments + 1;
                cachedTriangles[i * 3 + 2] = i + 1;
            }

            mesh.vertices = cachedVertices;
            mesh.triangles = cachedTriangles;
            mesh.RecalculateNormals();

            return mesh;
        }
        public static Mesh CreateSectorMesh(float angle)
        {
            angle = Mathf.Abs(angle);
            if (angle >= 360f)
            {
                return CreateCircleMesh();
            }

            Mesh mesh = new Mesh();

            // 扇形的顶点
            int segments = Mathf.Min(maxSegments, Mathf.CeilToInt(angle / 360f * maxSegments));
            int vertexCount = segments + 2; // 中心点 + 每个分段的顶点

            cachedVertices = new Vector3[vertexCount];
            cachedTriangles = new int[segments * 3];

            // 中心点
            cachedVertices[0] = Vector3.zero;

            // 扇形顶点
            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = Mathf.Deg2Rad * (-angle / 2 + (angle / segments) * i);
                cachedVertices[i + 1] = new Vector3(Mathf.Sin(currentAngle) * radius, 0, Mathf.Cos(currentAngle) * radius);
            }

            // 扇形三角形
            for (int i = 0; i < segments; i++)
            {
                cachedTriangles[i * 3] = 0;
                cachedTriangles[i * 3 + 1] = i + 1;
                cachedTriangles[i * 3 + 2] = i + 2;
            }

            mesh.vertices = cachedVertices;
            mesh.triangles = cachedTriangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
