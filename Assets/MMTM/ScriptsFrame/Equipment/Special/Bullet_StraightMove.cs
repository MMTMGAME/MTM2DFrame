using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using MMTM;
using UnityEngine;

[RequireComponent(typeof(RegistPrefab))]
public class Bullet_StraightMove : MonoBehaviour,IBullet
{
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float damage = 1;
    [SerializeField]
    private float surviveTime = 4;
    
    [SerializeField]
    private IBullet.BulletData _bulletData;
    private CancellationTokenSource moveToken;
    public void Shoot(IBullet.BulletData bulletData)
    {
        MyDebug.Log("射出子弹");
        _bulletData = bulletData;
        Move().Forget();
        onShootBegin?.Invoke();
    }

    public event Action onShootBegin;
    public event Action onShootOver;
    public event Action<int> onShootAttack;

    async UniTaskVoid Move()//前进检测是否击中
    {
        float move_t = 0;
        var surtime = _bulletData.replaceSurvivetime == 0 ? surviveTime : _bulletData.replaceSurvivetime;
        
        if (moveToken != null && !moveToken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            moveToken.Cancel();
            moveToken.Dispose();
        }

        moveToken = new CancellationTokenSource();

        while (move_t < surtime)
        {
            try
            {
                var hit = MyTools.CreateOffsetRaycast2D(transform.position, Vector2.zero, _bulletData.diretion, speed * Time.deltaTime, _bulletData.aim_layer);
                if (hit)
                {
                     AttackAim(hit.collider.gameObject.GetInstanceID(),hit.point);
                     onShootAttack?.Invoke(hit.collider.gameObject.GetInstanceID());
                     OnHit(hit.point).Forget();
                     break;
                }
                transform.position += (Vector3)_bulletData.diretion * Time.deltaTime * speed;
                move_t += Time.deltaTime;
                //延迟
                await UniTask.Delay(TimeSpan.FromTicks(1), cancellationToken: moveToken.Token);
            }
            catch (OperationCanceledException)
            {
                // 处理取消逻辑
                return;
            }
        }

        Despawn();
    }

    void AttackAim(int instance,Vector2 point)
    {
        if (PrefabCenter.Instance.TryGetIntefaceComponent(instance, out IGetAttacked[] getAttackAbles))
        {
            IGetAttacked.AttackerData getattackData = new IGetAttacked.AttackerData(_bulletData.attacker_instanceid,_bulletData.replaceDamage ==0?damage:_bulletData.replaceDamage , point, _bulletData.diretion);
            foreach (var VARIABLE in getAttackAbles)
            {
                VARIABLE.GetAttack(getattackData);
            }
        }
    }

    async UniTaskVoid OnHit(Vector2 hitpoint)//击中后子弹需要到达目标点然后消失
    {
        while (Vector2.Distance(transform.position, hitpoint) >0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, hitpoint, 0.15f);
            await UniTask.Delay(TimeSpan.FromTicks(1));
        }
    }

    void Despawn()
    {
        onShootOver?.Invoke();
        LeanPool.Despawn(gameObject,1);
    }
}

public interface IBullet
{
    [System.Serializable]
    public struct BulletData
    {
        public BulletData(int attacker_instanceid, Vector2 shootPos_, Vector2 diretion, LayerMask aim_layer, float replaceDamage_ = 0,
            float replaceSurvivetime_ = 0)
        {
            this.attacker_instanceid = attacker_instanceid;
            this.diretion = diretion;
            this.replaceDamage = replaceDamage_;
            this.replaceSurvivetime = replaceSurvivetime_;
            this.aim_layer = aim_layer;
            shootPos = shootPos_;
        }

        public int attacker_instanceid;
        public Vector2 shootPos;
        public Vector2 diretion;
        public float replaceDamage;
        public float replaceSurvivetime;
        public int aim_layer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="diretion">方向</param>
    /// <param name="FlySpeed">飞行速度</param>
    /// <param name="Damage"></param>
    /// <param name="ThroughProbability">穿透率</param>
    /// <param name="Survivetime">子弹飞行时间</param>
    void Shoot(BulletData bulletData);
    /// <summary>
    /// 飞行结束
    /// </summary>
    event Action onShootBegin;
    
    /// <summary>
    /// 飞行结束
    /// </summary>
    event Action onShootOver;
    
    /// <summary>
    /// 当射击到物品时
    /// </summary>
    event Action<int> onShootAttack;
}