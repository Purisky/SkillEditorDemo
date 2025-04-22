using System;
using System.Reflection;
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
