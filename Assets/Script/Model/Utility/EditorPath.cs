#if UNITY_EDITOR
using NUnit.Framework.Interfaces;
using SkillEditorDemo.Utility;
using System.IO;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public static class EditorPath
    {
        public static string Assets;

        public static string[] GetFileNames(string path, string searchPattern, SearchOption searchOption)
        {
            DirectoryInfo directory = new ($"{Assets}/{path}");
            FileInfo[] fileInfos = directory.GetFiles(searchPattern, searchOption);
            string[] strings = new string[fileInfos.Length];
            for (int i = 0; i < fileInfos.Length; i++)
            {
                strings[i] = Path.GetFileNameWithoutExtension(fileInfos[i].FullName);
            }
            return strings;
        }

    }
}
#endif