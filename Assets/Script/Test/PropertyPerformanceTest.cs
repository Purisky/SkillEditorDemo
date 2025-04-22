using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using TreeNode.Runtime;
using Unity.Properties;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PropertyPerformanceTest : MonoBehaviour
{
    private const int WarmupCount = 100;
    private const int TestCount = 1000;
    private const int MinDepth = 5;
    private const int MaxDepth = 10;

    private class TestClass
    {
        public TestClass Child;
        public int Value;
        public List<TestClass> Items;
        public int[] Array;
    }

    void Start()
    {
        // 预热
        RunTest(MinDepth, true);
        
        // 正式测试
        for (int depth = MinDepth; depth <= MaxDepth; depth++)
        {
            RunTest(depth);
        }
    }

    private void RunTest(int depth, bool warmup = false)
    {
        // 创建测试对象
        var root = CreateNestedObject(depth);
        
        // 测试PropertyAccessor
        var sw = Stopwatch.StartNew();
        string path = GetPropertyPath(depth);
        for (int i = 0; i < (warmup ? WarmupCount : TestCount); i++)
        {
            int value = PropertyAccessor.GetValue<int>(root, path);
            value++;
            PropertyAccessor.SetValue(root, path, value);
        }
        PropertyAccessor.SetValue(root, path, 0);
        sw.Stop();
        var accessorTime = sw.ElapsedTicks;

        // 测试PropertyContainer
        sw.Restart();
        PropertyPath propertyPath = new PropertyPath(GetPropertyPath(depth));
        for (int i = 0; i < (warmup ? WarmupCount : TestCount); i++)
        {
            int value = PropertyContainer.GetValue<TestClass,int>(root, propertyPath);
            value++;
            PropertyContainer.SetValue(root, propertyPath, value);
        }
        sw.Stop();
        var containerTime = sw.ElapsedTicks;

        //// 测试FastExpressionCompiler
        //sw.Restart();
        //var getValueDelegate = CreateGetValueDelegate(root.GetType(), path);
        //var setValueDelegate = CreateSetValueDelegate(root.GetType(), path);
        //for (int i = 0; i < (warmup ? WarmupCount : TestCount); i++)
        //{
        //    int value = getValueDelegate(root);
        //    value++;
        //    setValueDelegate(root, value);
        //}
        //sw.Stop();
        //var fastExpressionCompilerTime = sw.ElapsedTicks;



        if (!warmup)
        {
            float rate = (float)accessorTime / containerTime;
            Debug.Log($"Depth: {depth}, PropertyAccessor: {accessorTime} ticks, PropertyContainer: {containerTime} ticks rate:{rate}");
        }
    }
    private TestClass CreateNestedObject(int depth)
    {
        var root = new TestClass();
        var current = root;
        for (int i = 1; i < depth; i++)
        {
            current.Child = new TestClass();
            current.Items = new List<TestClass>()
            {
                new TestClass()
            };
            current = current.Items[0];
        }
        return root;
    }

    private string GetPropertyPath(int depth)
    {
        var path = "Items[0]";
        for (int i = 1; i < depth - 1; i++)
        {
            path += ".Items[0]";
        }
        return path + ".Value";
    }
}
