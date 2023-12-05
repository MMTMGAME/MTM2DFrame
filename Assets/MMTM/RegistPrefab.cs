using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RegistPrefab : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnRegistEvent;
    
    [SerializeField]
    private UnityEvent OnEnableEvent;
    
    [SerializeField]
    private UnityEvent OnDisableEvent;
    private void Awake()
    {
        PrefabCenter.Instance.RegisterPrefab(gameObject);
        OnRegistEvent?.Invoke();
    }

    private void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    private void OnDisable()
    {
        OnDisableEvent?.Invoke();
    }
}
