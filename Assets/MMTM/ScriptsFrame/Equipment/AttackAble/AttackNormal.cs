using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 给装备用的，抛射物不要用。
/// </summary>
public interface IEquipment
{
    /// <summary>
    /// 只触发一次
    /// </summary>
    void Attack();
    /// <summary>
    /// 取消攻击
    /// </summary>
    void CancelAttack();
    /// <summary>
    /// 每帧都攻击
    /// </summary>
    void ContinueAttack();
    
    event Action<int> onAttack;
    event Action onAttackOver;
}

public class AttackNormal : MonoBehaviour,IEquipment
{
    [SerializeField]private LayerMask attackLayer;
    [SerializeField]private float damage;
    private Collider2D _collider2D;
    private bool isEnable = false;
    public event Action<int> onAttack;
    public event Action onAttackOver;

    [SerializeField]
    private UnityEvent OnAttackAction;
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        CancelAttack();
    }

    public void SetEnable(bool va)
    {
        isEnable = va;

        if (isEnable)
        {
            _collider2D.enabled = true;
        }
        else
        {
            _collider2D.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    
    {
        int instanceid = other.gameObject.GetInstanceID();
        if ((attackLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (PrefabCenter.Instance.TryGetIntefaceComponent(instanceid,out IGetAttacked[] getAttackeds))
            {
                foreach (var getAttacked in getAttackeds)
                {
                    getAttacked.GetAttack(new IGetAttacked.AttackerData(gameObject.GetInstanceID(), damage, transform.position,(other.transform.position - transform.position).normalized));
                }
                MyDebug.Log($"攻击到了:{gameObject.GetInstanceID()}");
                onAttack?.Invoke(instanceid);
                OnAttackAction?.Invoke();
            }
        }
    }

    public void ContinueAttack()
    {
        _collider2D.enabled = false;
        _collider2D.enabled = true;
    }



    public void Attack()
    {
        _collider2D.enabled = true;
    }

    public void CancelAttack()
    {
        _collider2D.enabled = false;
        onAttackOver?.Invoke();
    }
}
