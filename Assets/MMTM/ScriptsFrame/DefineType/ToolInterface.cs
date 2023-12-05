using System;
using UnityEngine;

public interface ITimer
{
    /// <summary>
    /// 设置过多少时间
    /// </summary>
    /// <param name="add"></param>
    void SetIntervalTime(float add);
    
    /// <summary>
    /// 时间是否到了
    /// </summary>
    /// <returns></returns>
    bool IsTimeArrive();
}



public interface IGravity
{
    public Rigidbody2D rigidbody2D { get; set; }
    bool HaveGravity { get; set; }
}
public interface IHealth
{
    float MaxHealth { get;}
    float CurrentHealth { get;}
    bool IsDie { get;}
    /// <summary>
    /// 出生
    /// </summary>
    void OnSpawn();

    bool Dodelta(float Damage);

    public event Action OnDie;
}

public interface IDieAble
{
    void Die();
    event Action OnDieEvent;
    bool IsDie { get; set; }
}

public interface ISwitch
{
    void Enable();
    void Disable();
}

public interface IThroghProbability
{
    public float ThroughProbability { get; set; } //穿透几率
}