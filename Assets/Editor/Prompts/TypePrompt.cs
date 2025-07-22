using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TreeNode;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    public class TypePrompt : BasePrompt
    {
        public List<FieldPrompt> Fields = new();
        public TypePrompt(Type type) : base(type)
        {
            MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (var member in members)
            {
                if (member.GetCustomAttribute<PromptAttribute>() == null) { continue; }
                if (member is FieldInfo field)
                {
                    Fields.Add(new FieldPrompt(field));
                }
                else if (member is PropertyInfo property)
                {
                    if (!property.CanWrite) { continue; }
                    Fields.Add(new FieldPrompt(property));
                }
            }
        }
        public void HandleFields(Dictionary<string,BasePrompt> dic)
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                Type type = Fields[i].FieldType;
                if (type.Inherited(typeof(IList)))
                { 
                    type = type.GetGenericArguments()[0];
                }
                if (type.IsPrimitive || type == typeof(string)) { continue; }
                if (type.Inherited(typeof(JsonNode))) { continue; }
                if (dic.ContainsKey(type.Name)) { continue; }
                if (type.IsEnum)
                {
                    dic[type.Name] = new EnumPrompt(type);
                }
                else
                {
                    TypePrompt typePrompt = new(type);
                    typePrompt.HandleFields(dic);
                    dic[type.Name] = typePrompt;
                }
            }
        }


        public override string ToString()
        {
            string nodeText = "";
            if (Type.Inherited(typeof(JsonNode)))
            {
                nodeText = "[Node]";
            }
            var result = $"{TypeName}{nodeText}\n";
            if (!string.IsNullOrEmpty(Description))
            {
                result += $"描述: {Description}";
            }

            if (Fields.Count > 0)
            {
                result += "\n字段:";
                foreach (var field in Fields)
                {
                    result +=$"\n{field}";
                }
            }
            return result;
        }



    }
}
