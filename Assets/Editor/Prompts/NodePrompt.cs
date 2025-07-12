using System;
using System.Collections.Generic;
using System.Linq;
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
        public AssetFilterAttribute AssetFilter;
        public NodePrompt(Type type) : base(type)
        {
            Abstract = type.IsAbstract;
            var nodeInfoAttr = type.GetCustomAttribute<NodeInfoAttribute>();
            if (nodeInfoAttr != null&& nodeInfoAttr.Type!=null)
            {
                PortType = nodeInfoAttr.Type.TypeName();
            }
            AssetFilter = type.GetCustomAttribute<AssetFilterAttribute>();
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
            string filterInfo = "";
            if (AssetFilter != null)
            {
                if (AssetFilter.Allowed)
                {
                    filterInfo = $"只允许出现在 {string.Join(',', AssetFilter.Types.Select(t => t.Name))} 中";
                }
                else
                {
                    filterInfo = $"禁止出现在 {string.Join(',', AssetFilter.Types.Select(t => t.Name))} 中";
                }
            }
            if (string.IsNullOrEmpty(PortType))
            {
                result += $" {filterInfo}\n";
            }
            else
            {
                result += $":{PortType} {filterInfo}\n";
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
                if (ToolUtil.Prompts.TryGetValue(type.Name, out var prompt))
                { 
                    result += $"\n{prompt}";
                }
            }
            
            return result;
        }
    }
}
