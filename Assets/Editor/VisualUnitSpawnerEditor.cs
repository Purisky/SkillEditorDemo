using UnityEngine;
using UnityEditor;
using SkillEditorDemo;
using SkillEditorDemo.View;
using System.Collections.Generic;
using System.Linq;

namespace SkillEditorDemo.Editor
{
    /// <summary>
    /// VisualUnitSpawner的Inspector面板自定义编辑器
    /// </summary>
    [CustomEditor(typeof(VisualUnitSpawner))]
    public class VisualUnitSpawnerEditor : UnityEditor.Editor
    {
        private VisualUnitSpawner spawner;
        private bool showRotationPreview = false; // 是否显示旋转预览
        
        // Unit ID下拉框相关
        private List<string> availableUnitIDs = new List<string>();
        private string[] unitIDOptions;
        private int selectedUnitIDIndex = 0;

        private void OnEnable()
        {
            spawner = (VisualUnitSpawner)target;
            RefreshUnitIDList();
        }
        
        /// <summary>
        /// 刷新Unit ID列表
        /// </summary>
        private void RefreshUnitIDList()
        {
            availableUnitIDs.Clear();
            
            // 从Resources/Data加载所有UnitData
            var unitDatas = Resources.LoadAll<SkillEditorDemo.View.UnitData>("Data");
            
            foreach (var unitData in unitDatas)
            {
                availableUnitIDs.Add(unitData.name);
            }
            
            // 如果没有找到任何UnitData，添加默认选项
            if (availableUnitIDs.Count == 0)
            {
                availableUnitIDs.Add("Player");
                availableUnitIDs.Add("Minion0");
                availableUnitIDs.Add("Minion1");
                availableUnitIDs.Add("Minion2");
            }
            
            unitIDOptions = availableUnitIDs.ToArray();
            
            // 查找当前unitID在列表中的索引
            selectedUnitIDIndex = Mathf.Max(0, availableUnitIDs.IndexOf(spawner.unitID));
        }

        public override void OnInspectorGUI()
        {
            // 开始属性检查
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.LabelField("Unit配置", EditorStyles.boldLabel);
            
            // Unit ID下拉框
            EditorGUI.BeginChangeCheck();
            selectedUnitIDIndex = EditorGUILayout.Popup("Unit ID", selectedUnitIDIndex, unitIDOptions);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spawner, "修改Unit ID");
                spawner.unitID = unitIDOptions[selectedUnitIDIndex];
                EditorUtility.SetDirty(spawner);
            }
            
            // 刷新按钮
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.ExpandWidth(true)); // 占位符
            if (GUILayout.Button("🔄", GUILayout.Width(30), GUILayout.Height(20)))
            {
                RefreshUnitIDList();
            }
            EditorGUILayout.EndHorizontal();
            
            // 其他配置
            spawner.displayName = EditorGUILayout.TextField("Display Name", spawner.displayName);
            spawner.faction = EditorGUILayout.IntField("Faction", spawner.faction);
            spawner.isPlayer = EditorGUILayout.Toggle("Is Player", spawner.isPlayer);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("转向控制 (仅编辑器)", EditorStyles.boldLabel);
            spawner.showRotationHandle = EditorGUILayout.Toggle("Show Rotation Handle", spawner.showRotationHandle);

            // 旋转控制部分
            if (spawner.showRotationHandle)
            {
                EditorGUILayout.Space(5);
                
                // 旋转角度控制
                EditorGUI.BeginChangeCheck();
                float currentRotation = spawner.GetRotation();
                float newRotation = EditorGUILayout.Slider("旋转角度", currentRotation, 0f, 360f);
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spawner, "修改旋转角度");
                    spawner.SetRotation(newRotation);
                    EditorUtility.SetDirty(spawner);
                }

                // 快捷方向按钮
                EditorGUILayout.LabelField("快捷方向", EditorStyles.miniBoldLabel);
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("↑ 上 (0°)", GUILayout.Height(30)))
                {
                    Undo.RecordObject(spawner, "设置方向：上");
                    spawner.SetRotation(0f);
                    EditorUtility.SetDirty(spawner);
                }
                
                if (GUILayout.Button("→ 右 (90°)", GUILayout.Height(30)))
                {
                    Undo.RecordObject(spawner, "设置方向：右");
                    spawner.SetRotation(90f);
                    EditorUtility.SetDirty(spawner);
                }
                
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("↓ 下 (180°)", GUILayout.Height(30)))
                {
                    Undo.RecordObject(spawner, "设置方向：下");
                    spawner.SetRotation(180f);
                    EditorUtility.SetDirty(spawner);
                }
                
                if (GUILayout.Button("← 左 (270°)", GUILayout.Height(30)))
                {
                    Undo.RecordObject(spawner, "设置方向：左");
                    spawner.SetRotation(270f);
                    EditorUtility.SetDirty(spawner);
                }
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                // 预览信息
                showRotationPreview = EditorGUILayout.Foldout(showRotationPreview, "预览信息");
                if (showRotationPreview)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"当前角度: {currentRotation:F1}°");
                    
                    // 获取并显示单位半径
                    float unitRadius = GetUnitRadius(spawner.unitID);
                    EditorGUILayout.LabelField($"单位半径: {unitRadius:F2}");
                    
                    // 方向向量（Z轴视角）
                    Vector2 direction = new Vector2(
                        Mathf.Sin(currentRotation * Mathf.Deg2Rad),
                        Mathf.Cos(currentRotation * Mathf.Deg2Rad)
                    );
                    EditorGUILayout.LabelField($"方向向量: ({direction.x:F2}, {direction.y:F2})");
                    EditorGUI.indentLevel--;
                }
            }
            
            // 运行时提示
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("运行时：此GameObject已传递数据给CreateSystem并删除", MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("编辑器模式：运行时将自动删除GameObject并传递数据给CreateSystem", MessageType.Info);
            }
            
            // 应用属性修改
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(spawner);
            }
        }

        /// <summary>
        /// Scene视图自定义绘制
        /// </summary>
        private void OnSceneGUI()
        {
            if (spawner == null) return;

            Vector3 position = spawner.transform.position;
            
            // 显示单位名称标签
            var style = new GUIStyle();
            style.normal.textColor = spawner.isPlayer ? Color.cyan : Color.yellow;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 14;
            
            string label = spawner.unitID;
            if (!string.IsNullOrEmpty(spawner.displayName))
            {
                label += $"\n({spawner.displayName})";
            }
            
            Handles.Label(position, label, style);
            
            // 绘制转向handle（仅编辑器模式）
            if (spawner.showRotationHandle && !Application.isPlaying)
            {
                DrawRotationHandle(spawner);
            }
        }
        
        /// <summary>
        /// 绘制转向控制手柄 - Z轴视角2D游戏专用（绕Z轴旋转）
        /// </summary>
        /// <param name="spawner">VisualUnitSpawner实例</param>
        private void DrawRotationHandle(VisualUnitSpawner spawner)
        {
            Vector3 spawnerPos = spawner.transform.position;
            float currentRotation = spawner.GetRotation();
            
            // 获取单位数据中的实际半径
            float unitRadius = GetUnitRadius(spawner.unitID);
            float discRadius = Mathf.Max(unitRadius, 0.5f); // 最小显示半径0.5f
            
            // 保存当前Handles设置
            Color oldColor = Handles.color;
            Matrix4x4 oldMatrix = Handles.matrix;
            
            // 设置旋转手柄的颜色
            Handles.color = spawner.isPlayer ? Color.cyan : Color.yellow;
            
            // Z轴视角2D游戏的朝向计算：在XY平面，0°=向上(Y+), 90°=向右(X+), 180°=向下(Y-), 270°=向左(X-)
            float arrowLength = discRadius + 0.5f; // 箭头长度基于单位半径
            Vector3 forward = new Vector3(
                Mathf.Sin(currentRotation * Mathf.Deg2Rad),
                Mathf.Cos(currentRotation * Mathf.Deg2Rad),
                0
            ) * arrowLength;
            
            Vector3 arrowEnd = spawnerPos + forward;
            
            // 绘制朝向线
            Handles.DrawLine(spawnerPos, arrowEnd);
            
            // 绘制箭头头部
            Vector3 arrowHead1 = arrowEnd - forward.normalized * 0.3f + Vector3.Cross(forward, Vector3.forward).normalized * 0.15f;
            Vector3 arrowHead2 = arrowEnd - forward.normalized * 0.3f - Vector3.Cross(forward, Vector3.forward).normalized * 0.15f;
            Handles.DrawLine(arrowEnd, arrowHead1);
            Handles.DrawLine(arrowEnd, arrowHead2);
            
            // 2D转向控制 - 使用Disc限制在XY平面
            EditorGUI.BeginChangeCheck();
            
            // 绘制XY平面圆盘控制器（只允许Z轴旋转）
            Handles.color = Color.white * 0.3f; // 半透明的圆盘
            
            // 对于Z轴视角，我们需要使用FreeMoveHandle结合角度计算
            Vector3 currentDirection = new Vector3(
                Mathf.Sin(currentRotation * Mathf.Deg2Rad),
                Mathf.Cos(currentRotation * Mathf.Deg2Rad),
                0
            );
            Vector3 handlePos = spawnerPos + currentDirection * discRadius;
            
            // 使用FreeMoveHandle在圆盘边缘创建控制点
            Vector3 newHandlePos = Handles.FreeMoveHandle(
                handlePos,
                0.1f,
                Vector3.zero,
                Handles.CylinderHandleCap
            );
            
            if (EditorGUI.EndChangeCheck())
            {
                // 记录撤销操作
                Undo.RecordObject(spawner, "Rotate Unit");
                
                // 计算新的方向，并约束到圆盘边缘
                Vector3 directionFromCenter = (newHandlePos - spawnerPos);
                if (directionFromCenter.magnitude > 0.001f)
                {
                    // 将Z分量置零，确保在XY平面
                    directionFromCenter.z = 0;
                    directionFromCenter = directionFromCenter.normalized;
                    
                    // 计算新的角度
                    float newRotation = Mathf.Atan2(directionFromCenter.x, directionFromCenter.y) * Mathf.Rad2Deg;
                    
                    // 确保角度在0-360范围内
                    if (newRotation < 0) newRotation += 360f;
                    
                    spawner.SetRotation(newRotation);
                }
                
                // 标记场景为脏
                EditorUtility.SetDirty(spawner);
            }
            
            // 绘制更明显的圆盘边框和背景
            Handles.color = spawner.isPlayer ? Color.cyan : Color.yellow;
            Handles.DrawWireDisc(spawnerPos, Vector3.forward, discRadius);
            
            // 绘制半透明圆盘背景
            Handles.color = (spawner.isPlayer ? Color.cyan : Color.yellow) * 0.1f;
            Handles.DrawSolidDisc(spawnerPos, Vector3.forward, discRadius);
            
            // 绘制角度刻度线（每45度一条）在XY平面
            Handles.color = Color.gray * 0.6f;
            for (int angle = 0; angle < 360; angle += 45)
            {
                float radians = angle * Mathf.Deg2Rad;
                Vector3 tickStart = spawnerPos + new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0) * (discRadius * 0.9f);
                Vector3 tickEnd = spawnerPos + new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0) * discRadius;
                Handles.DrawLine(tickStart, tickEnd);
            }
            
            // 绘制角度标签
            string angleLabel = $"{currentRotation:F0}°";
            Vector3 labelPos = spawnerPos + Vector3.forward * 0.3f;
            
            var labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 12;
            
            Handles.Label(labelPos, angleLabel, labelStyle);
            
            // 绘制方向指示器（Z轴视角）- 在XY平面
            Handles.color = Color.white * 0.8f;
            var compassStyle = new GUIStyle();
            compassStyle.normal.textColor = Color.white;
            compassStyle.fontStyle = FontStyle.Bold;
            compassStyle.alignment = TextAnchor.MiddleCenter;
            compassStyle.fontSize = 10;
            
            // 上 (0°) - Y+方向
            Vector3 upPos = spawnerPos + Vector3.up * (discRadius + 0.3f) + Vector3.forward * 0.1f;
            Handles.Label(upPos, "↑", compassStyle);
            
            // 右 (90°) - X+方向  
            Vector3 rightPos = spawnerPos + Vector3.right * (discRadius + 0.3f) + Vector3.forward * 0.1f;
            Handles.Label(rightPos, "→", compassStyle);
            
            // 下 (180°) - Y-方向
            Vector3 downPos = spawnerPos + Vector3.down * (discRadius + 0.3f) + Vector3.forward * 0.1f;
            Handles.Label(downPos, "↓", compassStyle);
            
            // 左 (270°) - X-方向
            Vector3 leftPos = spawnerPos + Vector3.left * (discRadius + 0.3f) + Vector3.forward * 0.1f;
            Handles.Label(leftPos, "←", compassStyle);
            
            // 恢复Handles设置
            Handles.color = oldColor;
            Handles.matrix = oldMatrix;
        }
        
        /// <summary>
        /// 获取单位的实际半径
        /// </summary>
        /// <param name="unitID">单位ID</param>
        /// <returns>单位半径</returns>
        private float GetUnitRadius(string unitID)
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
    }
}
