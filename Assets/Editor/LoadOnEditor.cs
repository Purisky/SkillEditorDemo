using UnityEditor;
using UnityEngine;

namespace SkillEditorDemo.Editor
{
    [InitializeOnLoad]
    public class LoadOnEditor
    {
        static LoadOnEditor()
        {
            Utility.Debug.Init(Debug.Log, Debug.LogError);
            Model.EditorPath.Assets = Application.dataPath;
        }
    }
}
