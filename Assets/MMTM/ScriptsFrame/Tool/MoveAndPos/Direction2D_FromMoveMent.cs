using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IDirection
{
    Vector2 GetFaceDirection();
    Vector2 GetInputDirection();
}

/// <summary>
/// 根据movement的方向来确定面向
/// </summary>
public class Direction2D_FromMoveMent : MonoBehaviour,IDirection
{
    
    [Label("方向取反")]
    [SerializeField] 
    private bool isNegation = false;

    private IMoveMent _moveMent;
    private void Awake()
    {
        _moveMent = GetComponent<IMoveMent>();
    }

    public Vector2 GetFaceDirection()
    {
        return _moveMent.Direction.normalized * (isNegation?-1:1);
    }

    public Vector2 GetInputDirection()
    {
        return _moveMent.Direction.normalized * (isNegation?-1:1);
    }
}
