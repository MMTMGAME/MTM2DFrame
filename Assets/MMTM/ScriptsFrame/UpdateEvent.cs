using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

public class UpdateEvent : EventTemplate,IStartable,ITickable,IDisposable,IAsyncInit
{
    public void Start()
    {
    
    }

    public void Tick()
    {
        if(Time.timeScale >= 1) TriggerEvent();
    }

    public void Dispose()
    {
        ClearListener();
    }

    public async UniTask AsyncInit()
    {
        GameManager.Instance.GameControlUpdateEvent.AddListener(Tick);
    }
}
public class UpdateNoTimeScaleEvent : EventTemplate,IStartable,ITickable,IDisposable,IAsyncInit
{
    public void Start()
    {
    
    }

    public void Tick()
    {
        TriggerEvent();
    }

    public void Dispose()
    {
        ClearListener();
    }
    public async UniTask AsyncInit()
    {
        GameManager.Instance.GameControlUpdateEvent.AddListener(Tick);
    }
    
}
