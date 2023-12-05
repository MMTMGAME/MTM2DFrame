using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IRun
{
    void StartRun();
    void CancelRun();
    bool IsRun { get; }
    bool IsRunning { get; }
}
public class RunAble : MonoBehaviour,IRun
{
    [SerializeField]private float runSpeed;
    public float RunSpeed => runSpeed;
    
    private bool isRun = false;
    public bool IsRun => isRun;

    private bool isRunning = false;
    public bool IsRunning => isRunning;
    
    private IActionLimit _actionLimit;
    private IMoveMent _moveMent;
    private IDirection _direction;
    
    private CancellationTokenSource runtoken;
    private void Awake()
    {
        _actionLimit = GetComponent<IActionLimit>();
        _moveMent = GetComponent<IMoveMent>();
        _direction = GetComponent<IDirection>();

    }

    public void StartRun()
    {
        if(isRun)return;
        Running().Forget();
        isRun = true;
    }

    public void CancelRun()
    {
        StopRun();
        isRun = false;
        isRunning = false;
    }

    async UniTaskVoid Running()
    {
        StopRun();
        
        while (true)
        {
            _moveMent.Move(Vector3.right * _direction.GetInputDirection().x,runSpeed * Time.deltaTime);
            await UniTask.Delay(TimeSpan.FromTicks(1),cancellationToken:runtoken.Token);
        }
    }

    void StopRun()
    {
        if (runtoken != null && !runtoken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            runtoken.Cancel();
            runtoken.Dispose();
        }
        
        runtoken = new CancellationTokenSource();
    }
}
