using UnityEngine;

namespace SkillEditorDemo.View
{
    /// <summary>
    /// 无限网格地板生成器
    /// </summary>
    public static class InfiniteGridFloor
    {
        private static Material _gridMaterial;
        private static GameObject _gridFloor;

        /// <summary>
        /// 创建无限网格地板
        /// </summary>
        public static GameObject CreateGridFloor()
        {
            if (_gridFloor != null)
            {
                return _gridFloor;
            }

            // 创建地板GameObject
            _gridFloor = new GameObject("InfiniteGridFloor");
            
            // 添加MeshFilter和MeshRenderer
            var meshFilter = _gridFloor.AddComponent<MeshFilter>();
            var meshRenderer = _gridFloor.AddComponent<MeshRenderer>();
            
            // 创建一个大的平面网格
            meshFilter.mesh = CreatePlaneMesh(1000f);
            
            // 创建网格材质
            meshRenderer.material = GetOrCreateGridMaterial();
            
            // 设置位置
            _gridFloor.transform.position = new Vector3(0, -1, 0);
            
            return _gridFloor;
        }

        /// <summary>
        /// 创建平面网格
        /// </summary>
        private static Mesh CreatePlaneMesh(float size)
        {
            var mesh = new Mesh();
            mesh.name = "GridFloorMesh";

            // 创建一个简单的四边形
            var vertices = new Vector3[]
            {
                new Vector3(-size, 0, -size),
                new Vector3(-size, 0, size),
                new Vector3(size, 0, size),
                new Vector3(size, 0, -size)
            };

            var uvs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, size * 0.1f),
                new Vector2(size * 0.1f, size * 0.1f),
                new Vector2(size * 0.1f, 0)
            };

            var triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3
            };

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// 获取或创建网格材质
        /// </summary>
        private static Material GetOrCreateGridMaterial()
        {
            if (_gridMaterial != null)
            {
                return _gridMaterial;
            }

            // 创建一个基本的材质
            _gridMaterial = new Material(Shader.Find("Sprites/Default"));
            _gridMaterial.name = "GridFloorMaterial";
            
            // 创建网格纹理
            var gridTexture = CreateGridTexture();
            _gridMaterial.mainTexture = gridTexture;
            
            // 设置材质属性
            _gridMaterial.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            
            // 启用透明度
            _gridMaterial.SetFloat("_Mode", 2); // Fade mode
            _gridMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _gridMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _gridMaterial.SetInt("_ZWrite", 0);
            _gridMaterial.DisableKeyword("_ALPHATEST_ON");
            _gridMaterial.EnableKeyword("_ALPHABLEND_ON");
            _gridMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            _gridMaterial.renderQueue = 3000;

            return _gridMaterial;
        }

        /// <summary>
        /// 创建网格纹理
        /// </summary>
        private static Texture2D CreateGridTexture()
        {
            int textureSize = 128;
            var texture = new Texture2D(textureSize, textureSize);
            texture.name = "GridTexture";

            var pixels = new Color[textureSize * textureSize];
            
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    // 创建网格线
                    bool isGridLine = (x % 16 == 0) || (y % 16 == 0);
                    
                    if (isGridLine)
                    {
                        pixels[y * textureSize + x] = new Color(0.3f, 0.3f, 0.3f, 1f);
                    }
                    else
                    {
                        pixels[y * textureSize + x] = new Color(0f, 0f, 0f, 0f);
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;

            return texture;
        }

        /// <summary>
        /// 销毁网格地板
        /// </summary>
        public static void DestroyGridFloor()
        {
            if (_gridFloor != null)
            {
                Object.DestroyImmediate(_gridFloor);
                _gridFloor = null;
            }
            
            if (_gridMaterial != null)
            {
                Object.DestroyImmediate(_gridMaterial);
                _gridMaterial = null;
            }
        }
    }
}
