using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InitializationManager:IAsyncStartable
{
    [Inject] private MyDebug _myDebug;
    [Inject] private PrefabCenter _prefabCenter;
    [Inject] private LayerSelector _layerSelector;

    [Inject] private UpdateEvent _updateEvent;
    [Inject] private MyTimerAction _myTimerAction;
    [Inject] private GameManager _gameManager;

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        Debug.Log("_myTimerAction...");
        await _myTimerAction.AsyncInit();

        await _gameManager.AsyncInit();

        await _updateEvent.AsyncInit();
        
        MyDebug.Log("AsyncInitOver");
    }
}