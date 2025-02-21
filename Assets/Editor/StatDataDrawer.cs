using SkillEditorDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEditor;
using UnityEngine.UIElements;

namespace SkillEditorDemo
{
    [CustomPropertyDrawer(typeof(BuffInfo))]
    public class BuffInfoDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            var idField = new PopupField<string>(null, UniqNodeManager<BuffNode, BuffAsset>.Dropdowns.Select(n => n.Value).ToList(), 0,
                (value) => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns.FirstOrDefault(n => n.Value == value)?.Text ?? "None",
                (value) => UniqNodeManager<BuffNode, BuffAsset>.Dropdowns.FirstOrDefault(n => n.Value == value)?.Text ?? "None")
            {
                bindingPath = "ID"
            };
            idField.style.flexGrow = 2;
            idField.RegisterValueChangedCallback(evt =>
            {
                var buffs = UniqNodeManager<BuffNode, BuffAsset>.Dropdowns;
                var selectedBuff = buffs.FirstOrDefault(n => n.Value == evt.newValue);
                if (selectedBuff != null)
                {
                    property.FindPropertyRelative("ID").stringValue = selectedBuff.Value;
                }
            });
            container.Add(idField);
            var levelField = new IntegerField("Level") { bindingPath = "Level"};
            levelField.style.flexGrow = 1;
            levelField.Q <Label>().style.width = 50;
            levelField.Q<Label>().style.minWidth = 50;
            container.Add(levelField);


            var degreeField = new IntegerField("Degree") { bindingPath = "Degree" };
            degreeField.style.flexGrow = 1;
            degreeField.Q<Label>().style.width = 50;
            degreeField.Q<Label>().style.minWidth = 50;
            container.Add(degreeField);

            var paramField = new FloatField("Param") { bindingPath = "Param" };
            paramField.style.flexGrow = 1;
            paramField.Q<Label>().style.width = 50;
            paramField.Q<Label>().style.minWidth = 50;
            container.Add(paramField);

            return container;
        }
    }
    [CustomPropertyDrawer(typeof(SkillInfo))]
    public class SkillInfoDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            var idField = new PopupField<string>(null, UniqNodeManager<SkillNode, SkillAsset>.Dropdowns.Select(n => n.Value).ToList(), 0,
                (value)=> UniqNodeManager<SkillNode, SkillAsset>.Dropdowns.FirstOrDefault(n => n.Value == value)?.Text ?? "None",
                (value) => UniqNodeManager<SkillNode, SkillAsset>.Dropdowns.FirstOrDefault(n => n.Value == value)?.Text ?? "None")
            {
                bindingPath = "ID"
            };
            idField.style.flexGrow = 2;
            idField.RegisterValueChangedCallback(evt =>
            {
                var skills = UniqNodeManager<SkillNode, SkillAsset>.Dropdowns;
                var selectedSkill = skills.FirstOrDefault(n => n.Text == evt.newValue);
                if (selectedSkill != null)
                {
                    property.FindPropertyRelative("ID").stringValue = selectedSkill.Value;
                }
            });
            container.Add(idField);

            var levelField = new IntegerField("Level") { bindingPath = "Level" };
            levelField.style.flexGrow = 1;
            levelField.Q<Label>().style.width = 50;
            levelField.Q<Label>().style.minWidth = 50;
            container.Add(levelField);

            return container;
        }



    }
    [CustomPropertyDrawer(typeof(StatData))]
    public class StatDataDrawer : PropertyDrawer 
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            StatType[] filteredEnumValues = ((StatType[])Enum.GetValues(typeof(StatType))).Where(ShowOnMenu).ToArray();

            List<string> values = filteredEnumValues.Select(n => n.ToString()).ToList();

            List<string> Texts = filteredEnumValues.Select(n => GetLabelInfo(n)).ToList();

            var statField = new PopupField<string>(null, values, 0,
                GetLabelInfo, GetLabelInfo)
            {
                bindingPath = "Type"
            };
            statField.style.flexGrow = 2;
            statField.RegisterValueChangedCallback(evt =>
            {
                var selectedStat = values.FirstOrDefault(n => n == evt.newValue);
                if (selectedStat != null)
                {
                    property.FindPropertyRelative("Type").stringValue = selectedStat;
                }
            });

            container.Add(statField);

            var valueField = new FloatField(null) { bindingPath = "Value" };
            valueField.style.flexGrow = 1;
            container.Add(valueField);

            return container;
        }
        public bool ShowOnMenu(StatType type)
        {
            FieldInfo fieldInfo = type.GetType().GetField(type.ToString());
            HideEnumAttribute[] hideAttributes = (HideEnumAttribute[])fieldInfo.GetCustomAttributes(typeof(HideEnumAttribute), false);
            return hideAttributes.Length == 0;// && type.IsStat();
        }
        private string GetLabelInfo(StatType enumValue)
        {

            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            LabelInfoAttribute labelInfo = fieldInfo.GetCustomAttribute<LabelInfoAttribute>();
            string text =  labelInfo == null ? enumValue.ToString() : labelInfo.Text;
            //Debug.Log(text);
            return text;
        }
        private string GetLabelInfo(string enumValue)
        {
            if (string.IsNullOrEmpty(enumValue))
            {
                return "None";
            }
            FieldInfo fieldInfo = typeof(StatType).GetField(enumValue);
            if (fieldInfo != null)
            {
                LabelInfoAttribute labelInfo = fieldInfo.GetCustomAttribute<LabelInfoAttribute>();
                return labelInfo == null ? enumValue : labelInfo.Text;
            }
            return enumValue;

        }
    }
}
