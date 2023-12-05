using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public interface IPosition
{
    Vector3 Position { get; set; }
    void ApplyRealPosition(Vector3 position);
    void ResetPosition();
    void SetEnable(bool va);
}
public class Position2D : MonoBehaviour,IPosition
{
    
    [SerializeField]
    private Vector3 position;
    public Vector3 Position
    {
        set
        {
            position = value;
            
        }
        get
        {
            return position;
        }
    }
    private void OnEnable()
    {
        ResetPosition();
    }

    public void ApplyRealPosition(Vector3 position_)
    {
        transform.position = position_;
    }

    public void ResetPosition()
    {
        ApplyRealPosition(transform.position);
    }
    
    [SerializeField]
    private bool isenable = true;
    public void SetEnable(bool va)
    {
        isenable = va;
    }
    private void Update()
    {
        if (isenable)
        {
            ApplyRealPosition(Position);
        }
    }
}
