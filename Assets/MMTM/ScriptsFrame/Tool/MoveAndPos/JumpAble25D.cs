using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MMTM;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


public interface IJump
{
    bool CanJump { get; set; }
    float JumpStrenth { get; set; } //跳跃强度

    void Jump(Vector2 Dir, float strenth);
    void Jump();
    void CancelJump();
    bool IsJump { get; }

    event Action onJump;
    
    /// <summary>
    /// 上升
    /// </summary>
    event Action onJumpRise;
    
    /// <summary>
    /// 下降
    /// </summary>
    event Action onJumpFall;
    
    event Action onGround; 
}
public class JumpAble25D : MonoBehaviour,IJump
{
    private IPosition _position;
    private IActionLimit _actionLimit;
    private IDirection _direction;
    
   [SerializeField] private Vector3 jumpDir = Vector3.right;
   [SerializeField] private float jumpStrenth = 10;
   private Vector3 jumpdir2;
   private float jumpStrenth2;
   
   public float Gravity = 1;
   private Vector3 beginpoint;
   private Vector3 endpos;
   private float jumpt;
   private float facedir;
   
    private CancellationTokenSource jumptoken;
    [SerializeField]
    [ReadOnly]
    private bool isJump = false;
    public bool IsJump => isJump;


    [SerializeField]
    private UnityEvent OnGroundEvent;
    public event Action onGround;

    [SerializeField]
    private UnityEvent OnJumpRiseEvent;
    public event Action onJumpRise;
    
    [SerializeField]
    private UnityEvent OnJumpFallEvent;
    public event Action onJumpFall;
    
    [SerializeField]
    private UnityEvent OnJumpEvent;
    public event Action onJump;

    [SerializeField]
    [CurveRange(0f, 0f, 1f, 1f)] 
    private AnimationCurve curve;
    private void Awake()
    {
        _position = GetComponent<IPosition>();
        _direction = GetComponent<IDirection>();
        
    }

    public bool CanJump { get; set; }
    public float JumpStrenth { get; set; }
    
    public void Jump(Vector2 Dir, float strenth)
    {
        jumpdir2 = Dir;
        jumpStrenth2 = strenth;
        
        Jumping(jumpdir2,jumpStrenth2).Forget();
        OnJumpEvent?.Invoke();
    }

    public void Jump()
    {
        if(isJump)return;
        Jump(jumpDir, jumpStrenth);
    }

    public void CancelJump()
    {
        CancelJump_().Forget();
    }

    public void StopJump()
    {
        jumptoken.Cancel();
        jumptoken = new CancellationTokenSource();
        isJump = false;
    }

    public async UniTaskVoid CancelJump_()
    {
        if(jumpt>=0.5f)return;

        jumptoken.Cancel();
        jumptoken = new CancellationTokenSource();
        
        jumpt = 0.5f;
        var center = _position.Position;
        endpos = beginpoint + jumpdir2 * facedir * MyTools.GetAbs((center.x - beginpoint.x)) * 2;

        while (jumpt<1)
        {
            var nowpoint = MyTools.MyMath.GetBezierPoint_MidHight(curve.Evaluate(jumpt),beginpoint,center,endpos);
            _position.ApplyRealPosition(nowpoint);
            jumpt += Time.deltaTime * Gravity;

            if (jumpt < 0.5f)
            {
                OnJumpRiseEvent?.Invoke();
                onJumpRise?.Invoke();
            }
            else
            {
                OnJumpFallEvent?.Invoke();
                onJumpFall?.Invoke();
            }

            await UniTask.Delay(TimeSpan.FromTicks(1),cancellationToken:jumptoken.Token);
        }
        _position.ApplyRealPosition(endpos);
        isJump = false;
        OnGroundEvent?.Invoke();
    }

    async UniTaskVoid Jumping(Vector3 jumpDir_,float jumpStrenth_)
    {
        jumptoken = new CancellationTokenSource();
        jumpt = 0;
        facedir = _direction.GetInputDirection().x;
        beginpoint = _position.Position;
        endpos = beginpoint + jumpDir_  * facedir  * jumpStrenth_;
        var center = MyTools.GetBetweenPoint(beginpoint, endpos, 0.5f) + Vector3.forward * jumpStrenth_;
        while (jumpt<1)
        {
            var nowpoint = MyTools.MyMath.GetBezierPoint_MidHight(curve.Evaluate(jumpt),beginpoint,center,endpos);
            _position.ApplyRealPosition(nowpoint);
            jumpt += Time.deltaTime * Gravity;
            
            if (jumpt < 0.5f)
            {
                OnJumpRiseEvent?.Invoke();
                onJumpRise?.Invoke();
            }
            else
            {
                OnJumpFallEvent?.Invoke();
                onJumpFall?.Invoke();
            }
            
            await UniTask.Delay(TimeSpan.FromTicks(1),cancellationToken:jumptoken.Token);
        }
        _position.ApplyRealPosition(endpos);
        isJump = false;
        OnGroundEvent?.Invoke();
    }
}
