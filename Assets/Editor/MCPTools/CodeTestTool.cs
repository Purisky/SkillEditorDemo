using MCP4Unity;
using SkillEditorDemo.Model;
using System.Linq;
using TreeNode.Editor;
using UnityEngine;
using UnityEditor;

namespace SkillEditorDemo.MCP
{
    public class TestTools
    {
        [Tool("强制Unity重新编译程序集")]
        public static string RecompileAssemblies()
        {
            try
            {
                var log = new System.Text.StringBuilder();
                log.AppendLine("=== 开始强制重新编译程序集 ===");
                log.AppendLine($"时间: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                
                // 刷新资源数据库
                log.AppendLine("正在刷新资源数据库...");
                AssetDatabase.Refresh();
                
                // 强制重新编译脚本
                log.AppendLine("正在强制重新编译脚本...");
                AssetDatabase.ImportAsset("Assets", ImportAssetOptions.ImportRecursive);
                
                // 等待编译完成
                log.AppendLine("等待编译完成...");
                
                // 检查编译状态
                if (EditorApplication.isCompiling)
                {
                    log.AppendLine("⚠️ Unity正在编译中，请等待编译完成后再尝试调用测试方法");
                }
                else
                {
                    log.AppendLine("✅ 编译完成，程序集已重新加载");
                }
                
                log.AppendLine("=== 重新编译操作完成 ===");
                log.AppendLine();
                log.AppendLine("💡 提示：如果仍然无法调用测试方法，请尝试：");
                log.AppendLine("1. 等待几秒钟让Unity完成所有编译操作");
                log.AppendLine("2. 检查Console面板是否有编译错误");
                log.AppendLine("3. 重启Unity Editor（如果问题持续存在）");
                
                return log.ToString();
            }
            catch (System.Exception ex)
            {
                return $"重新编译过程中发生错误: {ex.Message}\n堆栈跟踪: {ex.StackTrace}";
            }
        }

        [Tool("检查程序集加载状态")]
        public static string CheckAssemblyStatus()
        {
            try
            {
                var log = new System.Text.StringBuilder();
                log.AppendLine("=== 程序集加载状态检查 ===");
                log.AppendLine($"检查时间: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                log.AppendLine();
                
                // 检查编译状态
                log.AppendLine($"Unity编译状态: {(EditorApplication.isCompiling ? "正在编译" : "编译完成")}");
                log.AppendLine($"Unity播放模式: {(EditorApplication.isPlaying ? "播放中" : "停止")}");
                log.AppendLine();
                
                // 获取程序集信息
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                log.AppendLine($"当前加载的程序集数量: {assemblies.Length}");
                log.AppendLine();
                
                // 查找测试相关的类型
                log.AppendLine("查找测试相关的类型:");
                int testClassCount = 0;
                int testMethodCount = 0;
                
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes()
                            .Where(t => t.Namespace?.Contains("SkillEditorDemo") == true || 
                                       t.Name.Contains("Test") || 
                                       t.Name.Contains("Performance"))
                            .ToArray();
                            
                        foreach (var type in types)
                        {
                            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                                .Where(m => m.GetParameters().Length == 0 && 
                                           m.ReturnType == typeof(string) &&
                                           (m.Name.StartsWith("Test") || m.Name.StartsWith("Profile")))
                                .ToArray();
                                
                            if (methods.Length > 0)
                            {
                                testClassCount++;
                                testMethodCount += methods.Length;
                                log.AppendLine($"  类: {type.FullName}");
                                foreach (var method in methods)
                                {
                                    log.AppendLine($"    方法: {method.Name}");
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // 跳过无法访问的程序集
                        log.AppendLine($"  无法访问程序集: {assembly.GetName().Name} ({ex.Message})");
                    }
                }
                
                log.AppendLine();
                log.AppendLine($"📊 统计结果:");
                log.AppendLine($"  测试类数量: {testClassCount}");
                log.AppendLine($"  测试方法数量: {testMethodCount}");
                
                if (testMethodCount == 0)
                {
                    log.AppendLine();
                    log.AppendLine("⚠️ 未找到任何测试方法，可能的原因：");
                    log.AppendLine("1. 脚本尚未编译完成");
                    log.AppendLine("2. 测试方法不符合规范（必须是public static string方法，无参数）");
                    log.AppendLine("3. 类名或命名空间不正确");
                }
                else
                {
                    log.AppendLine();
                    log.AppendLine("✅ 程序集状态正常，可以尝试调用测试方法");
                }
                
                log.AppendLine();
                log.AppendLine("=== 检查完成 ===");
                return log.ToString();
            }
            catch (System.Exception ex)
            {
                return $"检查程序集状态时发生错误: {ex.Message}\n堆栈跟踪: {ex.StackTrace}";
            }
        }



        [Tool("运行无参静态函数")]
        public static string RunCode(string methodFullName)
        {
            try
            {
                // 清理方法名，去除可能的括号和分号
                string cleanMethodName = methodFullName?.Trim()
                    .Replace("()", "")
                    .Replace(";", "")
                    .Trim();
                
                if (string.IsNullOrEmpty(cleanMethodName))
                {
                    return "方法名不能为空";
                }
                
                // 解析方法名，可能包含命名空间和类名
                string targetNamespace = null;
                string targetClassName = null;
                string targetMethodName = cleanMethodName;
                
                var parts = cleanMethodName.Split('.');
                if (parts.Length >= 3)
                {
                    // 格式: NameSpace.ClassName.MethodName 或 NameSpace.SubNameSpace.ClassName.MethodName
                    targetMethodName = parts[parts.Length - 1];
                    targetClassName = parts[parts.Length - 2];
                    targetNamespace = string.Join(".", parts.Take(parts.Length - 2));
                }
                else if (parts.Length == 2)
                {
                    // 格式: ClassName.MethodName
                    targetMethodName = parts[1];
                    targetClassName = parts[0];
                }
                
                // 获取当前域中的所有程序集
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        // 获取程序集中的所有类型
                        var types = assembly.GetTypes();
                        
                        foreach (var type in types)
                        {
                            // 通过命名空间和类名前缀过滤
                            if (!string.IsNullOrEmpty(targetNamespace) && 
                                !type.Namespace?.Equals(targetNamespace, System.StringComparison.OrdinalIgnoreCase) == true)
                            {
                                continue;
                            }
                            
                            if (!string.IsNullOrEmpty(targetClassName) && 
                                !type.Name.Equals(targetClassName, System.StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                            
                            // 查找指定名称的公共静态方法
                            var method = type.GetMethod(targetMethodName, 
                                System.Reflection.BindingFlags.Public | 
                                System.Reflection.BindingFlags.Static);
                            
                            if (method != null && method.GetParameters().Length == 0)
                            {
                                return $"成功调用方法: {type.FullName}.{targetMethodName}:\n{method.Invoke(null, null)}";
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // 跳过无法访问的程序集
                        UnityEngine.Debug.LogWarning($"无法访问程序集 {assembly.FullName}: {ex.Message}");
                    }
                }
                
                return $"未找到无参静态方法: {cleanMethodName}";
            }
            catch (System.Exception ex)
            {
                return $"执行方法时发生错误: {ex.Message}";
            }
        }
    }
}