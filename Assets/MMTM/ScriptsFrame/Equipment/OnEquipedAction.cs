using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class OnEquipedAction : MonoBehaviour,IOnEquiped
{
    [SerializeField]
    private UnityEvent OnEquipedEvent;
    public event Action onEquiped;
    
    [SerializeField]
    private UnityEvent onUnEquipedEvent;
    public event  Action onUnEquiped;

    public void OnEquiped(int equiper,string selfName)
    {
        onEquiped?.Invoke();
        OnEquipedEvent?.Invoke();
    }

    public void OnUnEquiped()
    {
        onUnEquiped?.Invoke();
        onUnEquipedEvent?.Invoke();
    }
}
/// <summary>
/// 被装备时会触发的接口
/// </summary>
public interface IOnEquiped
{
    void OnEquiped(int equiper,string selfName);
    void OnUnEquiped();

    event Action onEquiped;
    event Action onUnEquiped;
}
