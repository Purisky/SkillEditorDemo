using UnityEditor;
using UnityEngine;
using TreeNode.Utility;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace SkillEditorDemo
{
    public static class Test
    {
        [MenuItem("Test/Run _F5")]
        public static void RunF5()
        {
           
           //Enum.GetUnderlyingType(typeof(TestEnumInt));
           Debug.Log(Enum.GetUnderlyingType(typeof(TestEnumInt)));
            long longv = 3;
            TestEnumByte type =  (TestEnumByte)longv;
            Debug.Log(type);
        }
    }

    public enum TestEnumInt : int
    {
        A = 1,
        B = 2,
        C = 3,
    }
    public enum TestEnumByte : byte
    {
        A = 1,
        B = 2,
        C = 3,
    }
    public enum TestEnumShort : short
    {
        A = 1,
        B = 2,
        C = 3,
    }
    public enum TestEnumLong : long
    {
        A = 1,
        B = 2,
        C = 3,
    }

}