using MCP4Unity;
using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TreeNode;
using TreeNode.Editor;
using TreeNode.Runtime;
using TreeNode.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkillEditorDemo
{
    public class NodeTools
    {
        [Tool("为一个Asset文件添加Node")]
        public static string AddNode(
            [Desc("文件路径"), ParamDropdown(nameof(GetValidPath))] string path,
            [Desc("添加的节点路径"),ParamDropdown(nameof(GetValidNodePath))] string nodePath,
            [Desc("Node类型")] string typeName,
            [Desc("以合并方式并入新对象,无需重复添加$type,数据禁止嵌套其他Node,所有Node数据必须通过AddNode工具添加")] string json)
        {
            return ToolUtil.AddNode(path, nodePath, typeName, json);
        }
        [Tool("修改Node数据")]
        public static string ModifyNode(
            [Desc("文件路径"), ParamDropdown(nameof(GetValidPath))] string path,
            [Desc("修改的节点路径"), ParamDropdown(nameof(GetValidNodePath))] string nodePath,
            [Desc("以覆盖合并方式修改对象,修改时禁止嵌套其他Node,所有Node数据必须通过AddNode工具添加")] string json)
        {
            return ToolUtil.ModifyNode(path, nodePath, json);
        }
        [Tool("删除一个Node")]
        public static string RemoveNode(
            [Desc("文件路径"), ParamDropdown(nameof(GetValidPath))] string path,
            [Desc("删除的节点路径"), ParamDropdown(nameof(GetValidNodePath))] string nodePath)
        {
            return ToolUtil.RemoveNode(path, nodePath, true);
        }
        [Tool("获取可用的Node信息")]
        public static string ListNodes([Desc("null时获取所有Node,否则获取继承自baseType的Node")] string baseType)
        {
            return string.Join("\n", ToolUtil.GetNodesByName(baseType).Select(n => n.HeadInfo()));
        }
        [Tool("获取Node的结构与用法")]
        public static string GetNodePrompt(string typeName)
        {
            if (ToolUtil.Prompts.TryGetValue(typeName, out var prompt) && prompt is NodePrompt nodePrompt)
            {
                return nodePrompt.ListDetail();
            }
            else
            {
                return $"Node {typeName} not found";
            }
        }
        [Tool("在所有节点构造结束后用于检查当前资源中是否有不应存在的空值或者其他数值越界行为,不应频繁调用,返回Success表示没有问题")]
        public static string ValidateAsset([Desc("文件路径"), ParamDropdown(nameof(GetValidPath))] string path)
        {
            return ToolUtil.ValidateAsset(path);
        }

        public static List<string> GetValidPath(MethodInfo methodInfo, Dictionary<string, object> Args)
        {
            var filePaths = new List<string>();

            try
            {
                // 获取Assets目录路径
                string assetsPath = Application.dataPath;

                if (!Directory.Exists(assetsPath))
                {
                    UnityEngine.Debug.LogWarning($"Assets path does not exist: {assetsPath}");
                    return filePaths;
                }

                // 递归搜索所有.ja和.pja文件
                var jaFiles = Directory.GetFiles(assetsPath, "*.ja", SearchOption.AllDirectories);
                var pjaFiles = Directory.GetFiles(assetsPath, "*.pja", SearchOption.AllDirectories);

                // 合并文件列表
                var allFiles = jaFiles.Concat(pjaFiles);

                foreach (string fullPath in allFiles)
                {
                    try
                    {
                        // 转换为相对于Assets的路径
                        string relativePath = Path.GetRelativePath(assetsPath, fullPath);

                        // 将反斜杠转换为正斜杠，保持一致性
                        relativePath = relativePath.Replace('\\', '/');

                        filePaths.Add(relativePath);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogWarning($"Error processing file path {fullPath}: {ex.Message}");
                    }
                }

                // 按文件名排序，便于用户查找
                filePaths.Sort();

                //UnityEngine.Debug.Log($"Found {filePaths.Count} .ja/.pja files");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error scanning for .ja/.pja files: {ex.Message}");
            }

            return filePaths;
        }

        public static List<(string,string)> GetValidNodePath(MethodInfo methodInfo, Dictionary<string, object> Args)
        {
            if (Args.TryGetValue("path", out object pathObj) && pathObj is string path && !string.IsNullOrEmpty(path))
            {
                return ToolUtil.GetValidPortPath(path);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Invalid or missing 'path' argument");
                return new List<(string, string)>();
            }
        }
    }
}
