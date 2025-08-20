using UnityEngine;
using UnityEditor;

namespace SkillEditorDemo.Editor
{
    /// <summary>
    /// ReadOnly特性的属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(SkillEditorDemo.View.ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 保存当前GUI状态
            bool wasEnabled = GUI.enabled;
            
            // 禁用GUI
            GUI.enabled = false;
            
            // 绘制属性
            EditorGUI.PropertyField(position, property, label);
            
            // 恢复GUI状态
            GUI.enabled = wasEnabled;
        }
    }
}
