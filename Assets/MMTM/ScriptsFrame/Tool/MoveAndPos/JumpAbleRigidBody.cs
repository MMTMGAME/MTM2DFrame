using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Ray2DJudge))]
public class JumpAbleRigidBody : MonoBehaviour,IJump
{
    [SerializeField]
    [ReadOnly]
    private bool isJump;
    public bool IsJump => isJump;


    public bool CanJump { get; set; }
    public float JumpStrenth { get; set; }

    private Rigidbody2D _rigidbody2D;
    private IDirection _direction;
    private Ray2DJudge _ray2DJudge;

    [SerializeField] private Vector2 JumpDirection;
    [SerializeField] private float JumpStrence;
    
    
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
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _direction = GetComponent<IDirection>();
        _ray2DJudge = GetComponent<Ray2DJudge>();
    }

    public void Jump(Vector2 Dir, float strenth)
    {
        isJump = true;
        var dir = new Vector2(JumpDirection.x * _direction.GetInputDirection().x, JumpDirection.y);
        _rigidbody2D.AddForce(dir * JumpStrence);
        Jumping().Forget();
    }

    public void Jump()
    {
        if(isJump)return;
        Jump(JumpDirection, JumpStrence);
    }

    async UniTaskVoid Jumping()
    {
        onJump?.Invoke();
        OnJumpEvent?.Invoke();
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        while (!_ray2DJudge.GetHit("judgeGround"))//如果没检测到地面就继续检测
        {
            if (_rigidbody2D.velocity.y > 0)
            {
                OnJumpRiseEvent?.Invoke();
                onJumpRise?.Invoke();
            }
            else
            {
                OnJumpFallEvent?.Invoke();
                onJumpFall?.Invoke();
            }

            await UniTask.Delay(TimeSpan.FromTicks(1));
        }
        isJump = false;
        onGround?.Invoke();
        OnGroundEvent?.Invoke();
    }

    public void CancelJump()
    {
        if(_rigidbody2D.velocity.y >=0)
        _rigidbody2D.velocity = Vector2.zero;
    }


}
