using System;
using Lean.Common;
using Lean.Pool;
using NaughtyAttributes;
using UnityEngine;


[RequireComponent(typeof(Ray2DJudge))]
public class Shadow2D : MonoBehaviour,IShadow
{
    [InfoBox("需要发射一根射线来创建影子")]
    [Label("影子对象")]
    [SerializeField]
    private GameObject shodowPrefab;
    private Ray2DJudge _ray2DJudge;

    private GameObject shadowInstance;

    private bool enable = true;
    private void Awake()
    {
        _ray2DJudge = GetComponent<Ray2DJudge>();
    }

    private void Update()
    {
        if(!enable)return;
       var hit = _ray2DJudge.GetHit("shdowJudge");
       if (hit)
       {
           JudgeIsNull();
           shadowInstance.transform.position = hit.point;
       }
       else
       {
           ReleaseShadow();
       }
    }

    void JudgeIsNull()
    {
        if (shadowInstance == null)
        {
            shadowInstance = LeanPool.Spawn(shodowPrefab);
        }
    }

    public void SetEnable(bool va)
    {
        enable = va;
        if (!enable)
        {
            ReleaseShadow();
        }
    }
    void ReleaseShadow()
    {
        if (shadowInstance != null)
        {
            LeanPool.Despawn(shadowInstance);
            shadowInstance = null;
        }
    }

    private void OnEnable()
    {
        SetEnable(true);
    }

    private void OnDisable()
    {
        SetEnable(false);
    }
}
