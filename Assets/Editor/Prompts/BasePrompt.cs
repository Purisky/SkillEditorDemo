﻿using UnityEngine;
using System;
using System.Reflection;
using TreeNode.Utility;
using TreeNode;
using System.Collections;
using TreeNode.Runtime;

namespace SkillEditorDemo
{
    public abstract class BasePrompt
    {
        public Type Type;
        public string TypeName;
        public string Description;
        public BasePrompt(Type type)
        {
            Type = type;
            TypeName = type.Name;
            var prompt = type.GetCustomAttribute<PromptAttribute>();
            if (prompt != null)
            {
                Description = prompt.Desc;
            }
        }
    }
    public static class PromptExtensions
    {
        public static string TypeName(this Type type)
        {
            if (type.Inherited(typeof(IList)))
            {
                return $"List<{type.GetGenericArguments()[0].TypeName()}>";
            }
            return type.Name;
        }

    }
    public class FieldPrompt
    {
        public string Name;
        public string Description;
        public bool IsRequired;
        public Type FieldType;
        public Type TypeWithoutList;
        public FieldPrompt(FieldInfo field)
        {
            Name = field.Name;
            FieldType = field.FieldType;
            TypeWithoutList = field.FieldType;
            if(TypeWithoutList.Inherited(typeof(IList)))
            {
                TypeWithoutList = TypeWithoutList.GetGenericArguments()[0];
            }
            var promptAttr = field.GetCustomAttribute<PromptAttribute>();
            if (promptAttr != null)
            {
                Description = promptAttr.Desc;
            }
            var childAttr = field.GetCustomAttribute<ChildAttribute>();
            if (childAttr != null)
            {
                IsRequired = childAttr.Require;
            }
        }
        public FieldPrompt(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            FieldType = propertyInfo.PropertyType;
            TypeWithoutList = propertyInfo.PropertyType;
            if (TypeWithoutList.Inherited(typeof(IList)))
            {
                TypeWithoutList = TypeWithoutList.GetGenericArguments()[0];
            }
            var promptAttr = propertyInfo.GetCustomAttribute<PromptAttribute>();
            if (promptAttr != null)
            {
                Description = promptAttr.Desc;
            }
            var childAttr = propertyInfo.GetCustomAttribute<ChildAttribute>();
            if (childAttr != null)
            {
                IsRequired = childAttr.Require;
            }
        }
        public override string ToString()
        {
            string nodeText = "";
            if (TypeWithoutList.Inherited(typeof(JsonNode)))
            {
                nodeText = "[Node]";
            }
            var result = $"  - {Name}({FieldType.TypeName()}{nodeText})";
            if (IsRequired)
            {
                result += " [必填]";
            }     
            if (!string.IsNullOrEmpty(Description))
            {
                var indentedDescription = Description.Replace("\n", "\n      ");
                result += $"\n      {indentedDescription}";
            }
            return result;
        }
    }


}
