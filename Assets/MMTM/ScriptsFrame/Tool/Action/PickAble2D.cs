using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
public interface IPickedAble
{
    void SetEnable(bool va);
    
    /// <summary>
    /// 拾取条件
    /// </summary>
    bool Picked(int picker);
    
    /// <summary>
    /// 被int谁捡了
    /// </summary>
    event Action<int> OnPicked;
}

public interface IPicker
{
    void Pick();
}

public class PickAble2D : MonoBehaviour,IPickedAble
{
    private IPosition _position;
    [InfoBox("添加进背包")]
    [SerializeField]
    [Label("是否默认开启")]
    private bool isEnbale = true;
    
    [SerializeField]
    private UnityEvent OnPickEvent;
    private void Awake()
    {
        _position = GetComponent<IPosition>();
    }
    
    public void SetEnable(bool va)
    {
        isEnbale = va;
    }
    public bool Picked(int picker)
    {
        if (!isEnbale) return false;
        MyDebug.Log($"{gameObject.name}被拾取");
        OnPicked?.Invoke(gameObject.GetInstanceID());
        OnPickEvent?.Invoke();
        return true;
    }
    public event Action<int> OnPicked;
    private void OnDisable() => SetEnable(false);
}
