using System;
using System.Collections.Generic;
using System.Reflection;
using TreeNode;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;

namespace SkillEditorDemo
{
    public class NodePrompt : TypePrompt
    {
        public bool Abstract;
        public string PortType;
        public NodePrompt(Type type) : base(type)
        {
            Abstract = type.IsAbstract;
            var nodeInfoAttr = type.GetCustomAttribute<NodeInfoAttribute>();
            if (nodeInfoAttr != null&& nodeInfoAttr.Type!=null)
            {
                PortType = nodeInfoAttr.Type.TypeName();
            }
        }
        public override string ToString()
        {
            var result = HeadInfo();

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

        public string HeadInfo()
        {
            var result = TypeName;
            if (string.IsNullOrEmpty(PortType))
            {
                result += "\n";
            }
            else
            {
                result += $":{PortType}\n";
            }
            if (!string.IsNullOrEmpty(Description))
            {
                result += $"描述: {Description}";
            }
            return result;
        }

        public string ListDetail()
        {

            string result = ToString();
            HashSet<Type> handledTypes = new ();
            for (int i = 0; i < Fields.Count; i++)
            {
                Type typeWithoutList = Fields[i].TypeWithoutList;
                if (!typeWithoutList.Inherited(typeof(JsonNode)))
                {
                    handledTypes.Add(typeWithoutList);
                }
            }
            foreach (var type in handledTypes)
            {
                if (type.IsPrimitive || type == typeof(string)) { continue; }
                if (NodeTools.Prompts.TryGetValue(type.Name, out var prompt))
                { 
                    result += $"\n{prompt}";
                }
            }
            return result;
        }
    }
}
