using System;
using UnityEngine;
public class EventTemplate
{
    public event Action OnEventTriggered;

    public void AddListener(Action listener)
    {
        OnEventTriggered += listener;
    }

    public void RemoveListener(Action listener)
    {
        OnEventTriggered -= listener;
    }

    public void ClearListener()
    {
        OnEventTriggered = null;
    }

    public void TriggerEvent()
    {
        OnEventTriggered?.Invoke();
    }
}
public class UnityEventTemplate<T>
{
    public event Action<T> OnEventTriggered;

    public void AddListener(Action<T> listener)
    {
        OnEventTriggered += listener;
    }

    public void RemoveListener(Action<T> listener)
    {
        OnEventTriggered -= listener;
    }

    public void TriggerEvent(T item)
    {
        OnEventTriggered?.Invoke(item);
    }
}