using System;
using System.Collections.Generic;
using System.Reflection;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    public class EnumPrompt : BasePrompt
    {
        public List<EnumField> Enums = new();
        public EnumPrompt(Type type) : base(type)
        {
            foreach (var item in Enum.GetNames(type))
            {
                FieldInfo fieldInfo = type.GetField(item);
                if (fieldInfo.GetCustomAttribute<HideEnumAttribute>() != null) { continue; }
                Enums.Add(new(fieldInfo));
            }  
        }

        public override string ToString()
        {
            var result = TypeName;
            if (!string.IsNullOrEmpty(Description))
            {
                result += $"描述: {Description}";
            }
            if (Enums.Count > 0)
            {
                result += "\n枚举:";
                foreach (var field in Enums)
                {
                    result +=$"\n   {field}";
                }
            }
            return result;
        }


    }

    public class EnumField
    {
        public string Name;
        public string Description;
        public EnumField(FieldInfo info)
        {
            Name = info.Name;
            PromptAttribute promptAttribute = info.GetCustomAttribute<PromptAttribute>();
            if (promptAttribute != null)
            {
                Description = promptAttribute.Desc;
            }
            else
            { 
                LabelInfoAttribute labelInfoAttribute = info.GetCustomAttribute<LabelInfoAttribute>();
                if (labelInfoAttribute != null)
                {
                    Description = labelInfoAttribute.Text;
                }
            }
        }

        public override string ToString()
        {
            if (Description != null)
            {
                return $"{Name}:{Description}";
            }
            return Name;
        }
    }

}
