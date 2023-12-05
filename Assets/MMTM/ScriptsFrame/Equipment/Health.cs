using System;
using Lean.Pool;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour,IHealth,IOnSpawn
{

    [Foldout("基础数据")]
    [SerializeField]float currenthealth;
    [Foldout("基础数据")]
    [SerializeField]float maxhealth = 1;
    [Foldout("基础数据")]
    [SerializeField]bool isDie = false;
    public bool IsDie => isDie;
    [Foldout("基础数据")]
    [SerializeField]bool isGod = false;
    public bool IsGod => isGod;
    public float CurrentHealth => currenthealth;
    public float MaxHealth => maxhealth;
    
    
    [SerializeField]
    [Label("是否立即销毁")]
    bool isDieImmDespawn = false;
    [Label("延迟,-1不销毁")]
    [SerializeField]
    [HideIf("isDieImmDespawn")]
    private float DespawnDelay = 0;
    [HorizontalLine(2,EColor.Green)]
    public UnityEvent OnDieEvent;
    public void OnSpawn(){
        currenthealth =  maxhealth;
        isDie = false;
        isGod = false;
    }
    public Action onSpawn { get; set; }
    public Action onSpawnClear { get; set; }

    [Button("立即死")]
    public void Die()
    {
        currenthealth = 0;
        isDie = true;
        OnDieEvent.Invoke();
        OnDie?.Invoke();
        OnDieClear?.Invoke();
        OnDieClear = null;
        if(DespawnDelay >= 0 )
        LeanPool.Despawn(gameObject,DespawnDelay);

    }
    public event Action OnDie;
    public event Action OnDieClear;
    public void SetGod(bool va)
    {
        isGod = va;
    }
    public void DodeltaUI(float Damage){
        if(!isGod)
        {
            currenthealth -= Damage;
            if(currenthealth<=0&&!isDie){
                Die();
            }
            return;
        }
    }
    public bool Dodelta(float Damage){
        if(!isGod)
        {
            currenthealth -= Damage;
            if(currenthealth<=0&&!isDie){
                Die();
            }
            return true;
        }
        return false;
    }
    
    private void OnEnable()
    {
        OnSpawn();
    }
    
}
