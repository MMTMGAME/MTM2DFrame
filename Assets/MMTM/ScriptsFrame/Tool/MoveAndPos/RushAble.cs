using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IRush
{
    void StartRush();
    void CancelRush();
    bool IsRush { get; }
    bool IsRushCding { get; }
    float RushDir { get; }
}

public class RushAble : MonoBehaviour,IRush
{
    
    [SerializeField]private float rushTime = 1;
    [SerializeField]private float rushSpeed;
    [SerializeField]private bool rushDirUseInput = false;
    [SerializeField]private float rushCd = 0.5f;
    private bool rushCding = false;
    private bool isRush = false;
    private bool _rushing;
    
    private IMoveMent _moveMent;
    private IDirection _direction;

    private float rushDir;

    public float RushDir => rushDir;
    public bool IsRush => isRush;
    public bool IsRushCding => rushCding;
    
    private CancellationTokenSource rushtoken;


    private void Awake()
    {
        _moveMent = GetComponent<IMoveMent>();
        _direction = GetComponent<IDirection>();
    }

    public void StartRush()
    {
        if(isRush)return;
        if(rushCding)return;
        Rushing().Forget();
        isRush = true;
    }
    
    async UniTaskVoid Rushing()
    {
        float rushTimer = 0;
        
        if (rushtoken != null && !rushtoken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            rushtoken.Cancel();
            rushtoken.Dispose();
        }
        
        rushtoken = new CancellationTokenSource();
        
        
        rushDir = _direction.GetFaceDirection().x;
        if (rushDirUseInput) rushDir = _direction.GetInputDirection().x;
        if((int)rushDir == 0)rushDir = _direction.GetFaceDirection().x;//如果玩家没有输入就还是用脸的方向
        
        while (rushTimer < rushTime)
        {
            _moveMent.Move(rushDir * Vector3.right , rushSpeed);
            await UniTask.Delay(TimeSpan.FromTicks(1),cancellationToken:rushtoken.Token);
            rushTimer += Time.deltaTime;
        }
        JoinRushCding();
        CancelRush();
    }

    public void CancelRush()
    {
        if (rushtoken != null && !rushtoken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            rushtoken.Cancel();
            rushtoken.Dispose();
        }
        
        rushtoken = new CancellationTokenSource();
        isRush = false;
    }



    async UniTaskVoid JoinRushCding()
    {
        rushCding = true;
        var time = 0f;
        while (time < rushCd)
        {
            time += Time.deltaTime;
            await UniTask.Delay(TimeSpan.FromTicks(1));
        }

        rushCding = false;
    }
}
