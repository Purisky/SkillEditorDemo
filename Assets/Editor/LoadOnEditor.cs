using SkillEditorDemo.View;
using UnityEditor;
using UnityEngine;

namespace SkillEditorDemo.Editor
{
    [InitializeOnLoad]
    public class LoadOnEditor
    {
        static LoadOnEditor()
        {
            TreeNode.Utility.Debug.Init(Debug.Log, Debug.LogError, (start, end, color, time) =>
            {
                Debug.DrawLine(new(start.X, 0, start.Y), new(end.X, 0, end.Y), new Color(color.R / 256f, color.G / 256f, color.B / 256f), time);
            });

            Model.EditorPath.Assets = Application.dataPath;
        }
    }
}
