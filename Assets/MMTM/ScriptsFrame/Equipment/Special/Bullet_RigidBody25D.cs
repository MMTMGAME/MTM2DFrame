using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MMTM;
using NaughtyAttributes;
using UnityEngine;

public class Bullet_RigidBody25D : MonoBehaviour,IBullet
{
    [ReadOnly]
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Vector2 Strenth;
    [SerializeField]
    private float attackheight;
    
    private Rigidbody2D _rigidbody2D;
    private IPosition _position;
    private Collider2D _collider2D;
    
    [SerializeField]
    [ReadOnly]
    private IBullet.BulletData _bulletData;
    
    public event Action onShootOver;
    public event Action<int> onShootAttack;
    public event Action onShootBegin;
    
    private CancellationTokenSource moveToken;

    private bool isEnable = false;

    [DarkTonic.MasterAudio.SoundGroupAttribute]
    [SerializeField]
    private string shootOverSound;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _position = GetComponent<IPosition>();
        _collider2D = GetComponent<Collider2D>();
    }

    public void Shoot(IBullet.BulletData bulletData)
    {
        _bulletData = bulletData;
        _rigidbody2D.isKinematic = false;
        _rigidbody2D.AddForce(bulletData.diretion * Strenth);
        _position.Position = PrefabCenter.Instance.GetIntefaceComponent<IPosition>(bulletData.attacker_instanceid)[0].Position;
        Fly().Forget();
        onShootBegin?.Invoke();
    }




    async UniTaskVoid Fly()
    {
        if (moveToken != null && !moveToken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            moveToken.Cancel();
            moveToken.Dispose();
        }

        moveToken = new CancellationTokenSource();
        _position.SetEnable(false);
        while(_position.Position.z >= 0)
        {
            ContinueAttack();
            _position.Position = new Vector3(transform.position.x, _position.Position.y, transform.position.y - _position.Position.y);
            await UniTask.Delay(TimeSpan.FromTicks(1), cancellationToken: moveToken.Token);
        }
        _position.Position = new Vector3(_position.Position.x, _position.Position.y, 0);
        _position.SetEnable(true);
        _rigidbody2D.isKinematic = true;
        _rigidbody2D.velocity = Vector2.zero;
        onShootOver?.Invoke();
        CancelAttack();

        if (shootOverSound?.Length > 0)
        {
            DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransform(shootOverSound, transform);
        }
    }

    public void CancelAttack()
    {
        isEnable = false;
    }

    public void ContinueAttack()
    {
        isEnable = true;
        _collider2D.enabled = false;
        _collider2D.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isEnable)return;
        if ((_bulletData.aim_layer & (1 << other.gameObject.layer)) != 0)
        {
            if (PrefabCenter.Instance.TryGetIntefaceComponent(other.gameObject.GetInstanceID(), out IPosition[] positions))
            {
                var distance = MyTools.GetAbs(_position.Position.z - positions[0].Position.z);
                if (distance < attackheight)
                {
                    onShootAttack?.Invoke(other.gameObject.GetInstanceID());
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var height = Vector3.up * attackheight / 2;
        Gizmos.DrawLine(transform.position - height,transform.position + height);
    }
}
