using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class GetAttackAbleHealth : MonoBehaviour,IGetAttacked
{
    private Action<IGetAttacked.AttackerData> onGetAttack;
    [SerializeField]
    [ReadOnly]
    private Health _health;
    
    [SerializeField]private UnityEvent OnGetAttack;
    public IGetAttacked.AttackerData GetAttackerData_ { get; set; }

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    public bool GetAttack(IGetAttacked.AttackerData gettackData)
    {
        MyDebug.Log($"受到攻击:{gettackData.ToString()}");
        GetAttackerData_ = gettackData;
        if (_health.CurrentHealth > 0)
        {
            _health.Dodelta(gettackData.end_damage);
            OnGetAttack?.Invoke();
            onGetAttack?.Invoke(gettackData);
            return true;
        }
        return false;
    }

    public void RegistOnGetAttack(Action<IGetAttacked.AttackerData> action)
    {
        onGetAttack += action;
    }
}
