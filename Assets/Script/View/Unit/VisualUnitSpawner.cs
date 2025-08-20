using UnityEngine;
using SkillEditorDemo.Model;
using SkillEditorDemo.Utility;
using Leopotam.EcsLite;

namespace SkillEditorDemo.View
{
    /// <summary>
    /// 可视化Unit生成器 - 仅编辑器配置工具
    /// 运行时会自动删除GameObject，数据传入CreateSystem
    /// </summary>
    public class VisualUnitSpawner : MonoBehaviour
    {
        [Header("Unit配置")]
        [SerializeField] public string unitID = "Player";
        [SerializeField] public string displayName = "";
        [SerializeField] public int faction = 0;
        [SerializeField] public bool isPlayer = false;
        
        [Header("生成设置")]
        [SerializeField] public bool autoSpawnOnStart = true;
        
        [Header("转向控制 (仅编辑器)")]
        [SerializeField, Range(0f, 360f)] private float rotationDegrees = 0f;
        [SerializeField] public bool showRotationHandle = true;
        
        private void Start()
        {
            //if (Application.isPlaying)
            //{
            //    // 运行时：传递数据给CreateSystem后删除GameObject
            //    SpawnUnitAndDestroy();
            //}
        }
        
        /// <summary>
        /// 运行时生成Unit并删除GameObject
        /// </summary>
        public void SpawnUnitAndDestroy()
        {
            // 坐标转换：Unity Transform -> ECS TransformCmp格式（Z轴视角，XY平面）
            var position = new System.Numerics.Vector2(transform.position.x, transform.position.y);
            var rotation = new Angle(rotationDegrees); // 使用手柄设置的角度
            
            // 调用CreateSystem.AddUnit方法
            CreateSystem.AddInitUnit(
                unitID,
                displayName,  // 如果为空，CreateSystem内部会使用默认名称
                faction,
                position,
                rotation,
                isPlayer
            );
            
            //UnityEngine.Debug.Log($"已生成Unit并删除配置GameObject: ID={unitID}, Name={displayName}, EntityID={entity}, 角度={rotationDegrees:F1}°");
            
            // 删除配置GameObject
            DestroyImmediate(gameObject);
        }
        
        /// <summary>
        /// 设置单位转向角度（仅编辑器使用）
        /// </summary>
        /// <param name="degrees">角度值（0-360度）</param>
        public void SetRotation(float degrees)
        {
            rotationDegrees = Mathf.Repeat(degrees, 360f);
            
            // 在编辑器中标记为脏，确保保存
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
            #endif
        }
        
        /// <summary>
        /// 获取当前转向角度
        /// </summary>
        public float GetRotation()
        {
            return rotationDegrees;
        }
        
        /// <summary>
        /// 获取单位的实际半径
        /// </summary>
        /// <returns>单位半径</returns>
        public float GetUnitRadius()
        {
            try
            {
                // 尝试从Resources/Data加载UnitData
                var unitData = Resources.Load<SkillEditorDemo.View.UnitData>($"Data/{unitID}");
                if (unitData != null)
                {
                    return unitData.Radius;
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning($"无法加载UnitData: {unitID}, 错误: {e.Message}");
            }
            
            // 如果无法加载，返回默认半径
            return 0.5f;
        }
        
        private void OnDrawGizmos()
        {
            // 获取实际的单位半径
            float unitRadius = GetUnitRadius();
            
            // 在Scene视图中显示Unit信息
            Gizmos.color = isPlayer ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, unitRadius);
            
            // 显示朝向 - Z轴视角，XY平面
            Gizmos.color = Color.yellow;
            float radians = rotationDegrees * Mathf.Deg2Rad;
            Vector3 forward = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0) * (unitRadius + 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + forward);
        }
    }
    
    /// <summary>
    /// ReadOnly特性，用于在Inspector中显示只读字段
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}
