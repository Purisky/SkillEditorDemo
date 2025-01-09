using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
namespace SkillEditorDemo.Editor
{
    [InitializeOnLoad]
    public class Debug
    {
        static Debug()
        {
            Utility.Debug.Init(UnityEngine.Debug.Log, UnityEngine.Debug.LogError);
        }
    }
    internal static class LogRedirection
    {
        private static readonly Regex LogRegex = new Regex(@" \(at (.+)\:(\d+)\)");

        [OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceId, int line)
        {
            string selectedStackTrace = GetSelectedStackTrace();
            if (string.IsNullOrEmpty(selectedStackTrace))
            {
                return false;
            }

            string[] Traces = selectedStackTrace.Split("\n");
            Match match;
            foreach (var trace in Traces)
            {
                match = LogRegex.Match(trace);
                if (match.Success)
                {
                    if (!trace.Contains("Debug.cs") &&
                        !trace.Contains("Json.cs"))
                    {
                        InternalEditorUtility.OpenFileAtLineExternal(Application.dataPath.Replace("Assets", "") + match.Groups[1].Value, int.Parse(match.Groups[2].Value));
                        return true;
                    }
                }
            }
            return false;
        }

        private static string GetSelectedStackTrace()
        {
            Assembly editorWindowAssembly = typeof(EditorWindow).Assembly;
            if (editorWindowAssembly == null)
            {
                return null;
            }

            System.Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null)
            {
                return null;
            }

            FieldInfo consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (consoleWindowFieldInfo == null)
            {
                return null;
            }

            EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as EditorWindow;
            if (consoleWindow == null)
            {
                return null;
            }

            if (consoleWindow != EditorWindow.focusedWindow)
            {
                return null;
            }

            FieldInfo activeTextFieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            if (activeTextFieldInfo == null)
            {
                return null;
            }

            return (string)activeTextFieldInfo.GetValue(consoleWindow);
        }
    }


}
