using UnityEngine;
using System;
using System.Collections.Generic;

// 对于匿名函数的gc alloc 测试

public static class Utils
{
    public static void Forecah<TKey, TValue>(this Dictionary<TKey, TValue> dict, System.Action<TKey, TValue> EnumeratorFunc)
    {
        if (dict == null || EnumeratorFunc == null)
            throw new System.ArgumentNullException();

        var i = dict.GetEnumerator();
        while (i.MoveNext())
        {
            EnumeratorFunc(i.Current.Key, i.Current.Value);
        }
    }
}

public class lambaTest : MonoBehaviour
{
    Dictionary<int, int> table = new Dictionary<int, int>();
    public int count;

    public Action<int, int> pCall;

    void Start()
    {
        pCall = CallVariable;

        table.Add(1, 1);
        table.Add(2, 2);
        table.Add(3, 3);
        table.Add(4, 4);
        table.Add(5, 5);
        table.Add(6, 6);
        table.Add(7, 7);
        table.Add(8, 8);
        table.Add(9, 9);

        count = 0;
    }

    void Update()
    {
        Profiler.BeginSample("AnonymousWithoutParam");
        AnonymousWithoutVariable();
        Profiler.EndSample();

        Profiler.BeginSample("FunctionWithoutVariable");
        FunctionWithoutVariable();
        Profiler.EndSample();

        Profiler.BeginSample("AnonymousWithoutParam");
        AnonymousVariable();
        Profiler.EndSample();

        Profiler.BeginSample("FunctionWithoutVariable");
        FunctionVariable();
        Profiler.EndSample();

        Profiler.BeginSample("CallInitVariable");
        aaaa();
        Profiler.EndSample();
    }

    // no gc
    void AnonymousWithoutVariable()
    {
        table.Forecah((k, v) =>
        {
            int c = 0;
            c = k + v;
        });
    }

    // has 104B
    void FunctionWithoutVariable()
    {
        table.Forecah(AddWithoutVariable);
    }

    void AddWithoutVariable(int k, int v)
    {
        int c = 0;
        c = k + v;
    }

    //////////////////////////////////////////////////
    /// 使用外部变量
    /////////////////////////////////////////
    void AnonymousVariable()
    {
        table.Forecah((k, v) =>
        {
            count = k + v;
        });
    }

    void FunctionVariable()
    {
        table.Forecah(AddtVariable);
    }

    void AddtVariable(int k, int v)
    {
        count = k + v;
    }

    /// <summary>
    /// 以下是解决方法
    /// </summary>
    void aaaa()
    {
        table.Forecah(pCall);
    }

    void CallVariable(int k, int v)
    {
        count = k + v;
    }
}
