using MCP4Unity;
using SkillEditorDemo.Model;
using System.Linq;
using System.Threading.Tasks;
using TreeNode.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace SkillEditorDemo.MCP
{
    public class CodeTools
    {
        [Tool("重新编译程序集")]
        public static string RecompileAssemblies()
        {
            CompilationPipeline.RequestScriptCompilation();
            return GetUnityConsoleLog();
        }
        [Tool("读取Unity控制台日志")]
        public static string GetUnityConsoleLog()
        {
            try
            {
                var result = new System.Text.StringBuilder();
                
                // 方法1: 通过LogEntries API获取日志
                var logInfo = GetLogEntriesInfo();
                if (!string.IsNullOrEmpty(logInfo))
                {
                    result.AppendLine(logInfo);
                }
                
                return result.ToString();
            }
            catch (System.Exception ex)
            {
                return $"❌ 获取日志失败: {ex.Message}";
            }
        }
        
        private static string GetLogEntriesInfo()
        {
            try
            {
                var result = new System.Text.StringBuilder();
                var logEntriesType = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
                if (logEntriesType == null)
                {
                    return "❌ 无法访问Unity日志系统";
                }

                // 保存原始控制台标志设置
                int originalFlags = 0;
                bool flagsChanged = false;
                System.Reflection.MethodInfo getConsoleFlagsMethod = null;
                System.Reflection.MethodInfo setConsoleFlag = null;
                try
                {
                    // 使用 get_consoleFlags() 函数获取当前控制台标志
                    getConsoleFlagsMethod = logEntriesType.GetMethod("get_consoleFlags", 
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    
                    if (getConsoleFlagsMethod != null)
                    {
                        var currentFlags = getConsoleFlagsMethod.Invoke(null, null);
                        originalFlags = (int)currentFlags;
                    }
                    
                    setConsoleFlag = logEntriesType.GetMethod("SetConsoleFlag", 
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    
                    if (setConsoleFlag != null)
                    {
                        setConsoleFlag.Invoke(null, new object[] { 0x287, true });
                        flagsChanged = true;
                    }
                }
                catch 
                {
                }

                var getCountMethod = logEntriesType.GetMethod("GetCount", 
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                var getEntryInternalMethod = logEntriesType.GetMethod("GetEntryInternal", 
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                
                if (getCountMethod == null || getEntryInternalMethod == null)
                {
                    return "❌ 无法访问日志方法";
                }

                int logCount = (int)getCountMethod.Invoke(null, null);
                if (logCount == 0)
                {
                    return "📭 控制台暂无日志";
                }

                result.AppendLine($"📊 总计 {logCount} 条日志\n");

                var logEntryType = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntry");
                if (logEntryType == null)
                {
                    return "❌ 无法访问日志条目类型";
                }

                // 显示最近的10条日志
                int startIndex = System.Math.Max(0, logCount - 10);
                int displayCount = logCount - startIndex;
                
                // // 首先输出LogEntry的所有字段信息，帮助调试
                // if (logCount > 0)
                // {
                //     try
                //     {
                //         var logEntry = System.Activator.CreateInstance(logEntryType);
                //         var parameters = new object[] { logCount - 1, logEntry };
                //         bool success = (bool)getEntryInternalMethod.Invoke(null, parameters);
                        
                //         if (success && logEntry != null)
                //         {
                //             result.AppendLine("🔍 LogEntry字段信息调试:");
                //             var allFields = logEntryType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                //             foreach (var field in allFields)
                //             {
                //                 try
                //                 {
                //                     var value = field.GetValue(logEntry);
                //                     string valueStr = value?.ToString() ?? "null";
                //                     if (field.Name == "mode" && value != null)
                //                     {
                //                         int modeInt = (int)value;
                //                         valueStr = $"{value} (0x{modeInt:X})";
                //                     }
                //                     result.AppendLine($"  {field.Name} ({field.FieldType.Name}): {valueStr}");
                //                 }
                //                 catch (System.Exception ex)
                //                 {
                //                     result.AppendLine($"  {field.Name}: 无法读取 ({ex.Message})");
                //                 }
                //             }
                //             result.AppendLine();
                //         }
                //     }
                //     catch (System.Exception ex)
                //     {
                //         result.AppendLine($"🔍 无法获取字段调试信息: {ex.Message}\n");
                //     }
                // }
                
                for (int i = startIndex; i < logCount; i++)
                {
                    try
                    {
                        var logEntry = System.Activator.CreateInstance(logEntryType);
                        var parameters = new object[] { i, logEntry };
                        
                        bool success = (bool)getEntryInternalMethod.Invoke(null, parameters);
                        if (!success || logEntry == null) continue;

                        // 获取日志信息
                        string message = GetLogMessage(logEntry, logEntryType);
                        string logType = GetLogTypeDetailed(logEntry, logEntryType);
                        string location = GetLogLocation(logEntry, logEntryType);
                        
                        // 格式化输出
                        if (!string.IsNullOrEmpty(message))
                        {
                            result.AppendLine($"{logType}{location}: {message}");
                        }
                    }
                    catch
                    {
                        // 跳过无法读取的条目
                        continue;
                    }
                }

                result.AppendLine($"\n显示了最近 {displayCount} 条日志");
                
                // 获取日志后恢复原始控制台标志设置
                if (flagsChanged && setConsoleFlag != null && getConsoleFlagsMethod != null)
                {
                    try
                    {
                        // 恢复原始标志设置
                        bool logEnabled = (originalFlags & 0x80) != 0;
                        bool warningEnabled = (originalFlags & 0x100) != 0;
                        bool errorEnabled = (originalFlags & 0x200) != 0;
                        
                        setConsoleFlag.Invoke(null, new object[] { 0x80, logEnabled });
                        setConsoleFlag.Invoke(null, new object[] { 0x100, warningEnabled });
                        setConsoleFlag.Invoke(null, new object[] { 0x200, errorEnabled });
                        
                    }
                    catch 
                    {
                    }
                }
                
                return result.ToString();
            }
            catch (System.Exception ex)
            {
                return $"❌ LogEntries访问失败: {ex.Message}";
            }
        }
    

        private static string GetLogMessage(object logEntry, System.Type logEntryType)
        {
            // 尝试多种可能的消息字段名
            string[] messageFields = { "condition", "message", "text", "content" };
            
            foreach (var fieldName in messageFields)
            {
                var field = logEntryType.GetField(fieldName, 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    string value = field.GetValue(logEntry)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
            }
            return "[无法获取消息内容]";
        }

        private static string GetLogTypeDetailed(object logEntry, System.Type logEntryType)
        {
            try
            {
                var modeField = logEntryType.GetField("mode", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (modeField == null) 
                {
                    // 尝试其他可能的字段名
                    modeField = logEntryType.GetField("type", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) ??
                               logEntryType.GetField("logType", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                }
                
                if (modeField == null) return "❓ [未知类型-无mode字段]";

                int mode = (int)modeField.GetValue(logEntry);
                
                // 尝试多种解析方式
                int logType = -1;
                // 方式1: 直接取低4位 (原始LogType值)
                int directType = mode & 0xF;
                
                
                string typeIcon = logType switch
                {
                    0 => "❌",    // Error
                    1 => "⚡",    // Assert
                    2 => "⚠️",    // Warning
                    3 => "🟢",    // Log
                    4 => "💥",    // Exception
                    _ => "❓"     // 未知
                };
                
                string typeName = logType switch
                {
                    0 => "错误",
                    1 => "断言", 
                    2 => "警告",
                    3 => "信息",
                    4 => "异常",
                    _ => $"未知({logType})"
                };
                
                // 返回详细信息用于调试
                return $"{typeIcon} [{typeName}]";
            }
            catch (System.Exception ex)
            {
                return $"❓ [类型解析失败: {ex.Message}]";
            }
        }

        private static string GetLogType(object logEntry, System.Type logEntryType)
        {
            // 尝试多种可能的类型字段名
            string[] modeFields = { "mode", "type", "logType", "entryType" };
            
            foreach (var fieldName in modeFields)
            {
                var field = logEntryType.GetField(fieldName, 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    var value = field.GetValue(logEntry);
                    if (value != null)
                    {
                        int mode = System.Convert.ToInt32(value);
                        return mode switch
                        {
                            1 => "❌",      // Error
                            2 => "⚠️",      // Warning  
                            4 => "⚠️",      // Assert
                            8 => "🔴",      // Exception
                            16 => "ℹ️",     // Log
                            _ => "🔍"       // Other
                        };
                    }
                }
            }
            return "ℹ️";
        }

        private static string GetLogLocation(object logEntry, System.Type logEntryType)
        {
            var fileField = logEntryType.GetField("file", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var lineField = logEntryType.GetField("line", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            string file = fileField?.GetValue(logEntry)?.ToString() ?? "";
            int line = lineField != null ? (int)(lineField.GetValue(logEntry) ?? 0) : 0;

            if (!string.IsNullOrEmpty(file) && line > 0)
            {
                return $" [{System.IO.Path.GetFileName(file)}:{line}]";
            }
            else if (!string.IsNullOrEmpty(file))
            {
                return $" [{System.IO.Path.GetFileName(file)}]";
            }
            return "";
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
                    targetMethodName = parts[^1];
                    targetClassName = parts[^2];
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

