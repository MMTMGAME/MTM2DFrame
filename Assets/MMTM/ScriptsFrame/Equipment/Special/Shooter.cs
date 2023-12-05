using System;
using Lean.Pool;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 射击者，装备上这个以后，可以射出子弹
/// </summary>
public class ShootAttack : MonoBehaviour,IEquipment,IOnEquiped
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform holdPos;
    [SerializeField]
    private Transform shootPos;
    [SerializeField]
    private float appendDamage;
    [SerializeField]
    private float appendSurTime;
    [SerializeField]
    private LayerMask aimLayer;
    private int shooter;
    public void Attack()
    {
        var willshootbullet = LeanPool.Spawn(bullet,shootPos.position,quaternion.identity);
        var shootdata = new IBullet.BulletData(shooter, shootPos.position, (shootPos.position - holdPos.position).normalized,aimLayer,appendDamage,appendSurTime);
        PrefabCenter.Instance.GetIntefaceComponent<IBullet>(willshootbullet.GetInstanceID())[0].Shoot(shootdata);
    }

    public void CancelAttack()
    {
        onAttackOver?.Invoke();
    }

    public void ContinueAttack()
    {
        
    }

    public event Action<int> onAttack;
    public event Action onAttackOver;

    public void OnEquiped(int equiper,string selfName)
    {
        shooter = equiper;
    }

    public void OnUnEquiped()
    {

    }

    public event Action onEquiped;
    public event Action onUnEquiped;
}
