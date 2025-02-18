using SkillEditorDemo.Model;
using SkillEditorDemo.View;
using System;
using System.Linq;
using System.Reflection;
using TreeNode.Utility;
using UnityEditor;
using UnityEngine;
using UnitData = SkillEditorDemo.View.UnitData;

namespace SkillEditorDemo
{
    [CustomPropertyDrawer(typeof(UnitData.StatData))]
    public class StatDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            var typeRect = new Rect(position.x, position.y, position.width * 2 / 3, position.height);
            var valueRect = new Rect(position.x + position.width*2 / 3, position.y, position.width/3, position.height);
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("Type"), GUIContent.none);

            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("Value"), GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
    [CustomPropertyDrawer(typeof(Model.StatType))]
    public class StatTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取当前枚举值
            StatType currentEnum = (StatType)property.intValue;

            // 获取所有枚举值
            StatType[] enumValues = (StatType[])Enum.GetValues(typeof(StatType));

            // 过滤枚举值
            StatType[] filteredEnumValues = enumValues.Where(ShowOnMenu).ToArray();

            // 获取当前枚举值在过滤后的枚举值中的索引
            int currentIndex = Array.IndexOf(filteredEnumValues, currentEnum);

            // 获取显示名称
            string[] displayNames = filteredEnumValues.Select(e => GetLabelInfo(e)).ToArray();

            // 显示枚举弹出菜单
            int newIndex = EditorGUI.Popup(position, currentIndex, displayNames);

            // 更新枚举值
            if (newIndex >= 0 && newIndex < filteredEnumValues.Length)
            {
                property.intValue = (int)filteredEnumValues[newIndex];
            }

            EditorGUI.EndProperty();
        }

        private string GetLabelInfo(StatType enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            LabelInfoAttribute[] attributes = (LabelInfoAttribute[])fieldInfo.GetCustomAttributes(typeof(LabelInfoAttribute), false);
            return attributes.Length > 0 ? attributes[0].Text : enumValue.ToString();
        }

        public bool ShowOnMenu(StatType type)
        {
            FieldInfo fieldInfo = type.GetType().GetField(type.ToString());
            HideEnumAttribute[] hideAttributes = (HideEnumAttribute[])fieldInfo.GetCustomAttributes(typeof(HideEnumAttribute), false);
            return hideAttributes.Length == 0 && type.IsStat();
        }

    }


}
