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
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<PromptAttribute>() == null) { continue; }
                var fieldPrompt = new FieldPrompt(field);
                Fields.Add(fieldPrompt);
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
            var result = $"{TypeName}\n";
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
