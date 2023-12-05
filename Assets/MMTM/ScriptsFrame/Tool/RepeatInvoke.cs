using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

public class RepeatInvoke : MonoBehaviour
{
    public bool isShowBegin = false;
    [SerializeField]
    private float frequency = 1;
    private CancellationTokenSource cts = new CancellationTokenSource();
    private Action repeatAction;

    public void SetReapeat(float frequency_,Action repeatAction_)
    {
        frequency = frequency_;
        repeatAction = repeatAction_;
        
    }

    public void RepeatBegin()
    {
        Spawn().Forget();
    }

    async UniTaskVoid Spawn()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(frequency),cancellationToken:cts.Token);
            repeatAction?.Invoke();
        }
    }
    private void OnEnable()
    {
        if(isShowBegin) RepeatBegin();
    }

    private void OnDisable()
    {
        cts.Cancel();
    }

    private void OnDestroy()
    {
        cts.Cancel();
    }
}
