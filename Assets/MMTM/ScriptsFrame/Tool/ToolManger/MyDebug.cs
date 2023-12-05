using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

public class MyDebug
{
    public static void Log(string information)
    {
        Debug.Log(information);
    }
}

// 在VContainer中注册这个类


