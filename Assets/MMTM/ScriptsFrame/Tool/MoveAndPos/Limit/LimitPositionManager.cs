using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitPositionManager:MonoBehaviour
{

    public List<Vector2> NowLimit;
        
    public static LimitPositionManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        
        if (NowLimit.Count >= 2)
        {
            Gizmos.color = Color.red;

            // 画出多边形的边
            for (int i = 0; i < NowLimit.Count - 1; i++)
            {
                Gizmos.DrawLine(NowLimit[i], NowLimit[i + 1]);
            }

            // 画出最后一条边，形成闭合多边形
            Gizmos.DrawLine(NowLimit[NowLimit.Count - 1], NowLimit[0]);
        }
    }
}
