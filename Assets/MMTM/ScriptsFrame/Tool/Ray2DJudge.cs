using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Ray2DJudge : MonoBehaviour
{
    [Button("添加射线")]
    [HideIf("isHaveRays")]
    void AddRay()
    {
        if (rays == null)
        {
            rays = new List<ARay>();
        }
    
        ARay newRay = new ARay
        {
            rayName = $"ray_{rays.Count}",
            rayOrigin = transform,
            rayLength = 1,
            layerMask = 1 << 3,
            rayDirection = Vector2.left,
            rayOriginOffset = Vector3.zero,
            hitColor = Color.red,
            noHitColor = Color.green,
            enableRayDebugTest = true
        };
    
        rays.Add(newRay);
    }
    [Serializable]
    public struct ARay
    {
        public  string rayName;
        [Header("起点")]public Transform rayOrigin;

        [Header("长度")]public float rayLength; //= 5f;

        [Header("层级")]public LayerMask layerMask;

        [Header("方向")]public Vector2 rayDirection;

        [Header("偏移")]public Vector3 rayOriginOffset;

        [Header("击中颜色")]public Color hitColor; //= Color.red;

        [Header("未击中颜色")]public Color noHitColor; //= Color.green;
        public bool enableRayDebugTest;
    }
    [ShowIf("isHaveRays")]
    public List<ARay> rays;
    private bool isHaveRays => rays != null && rays.Count>0;
    
    
    private void OnDrawGizmos()
    {

            if (rays == null)
            {
                Debug.LogWarning("now rays!" );
                return;
            }
            foreach (var VARIABLE in rays)
            {
                if(!VARIABLE.enableRayDebugTest)continue;
                if (VARIABLE.rayOrigin == null)
                {
                    Debug.LogWarning("Ray origin is null for ray: " + VARIABLE.rayName);
                    return;
                }
                
                Vector3 scale = transform.localScale;
                Vector3 adjustedOffset = Vector3.Scale(VARIABLE.rayOriginOffset, scale);
                float adjustedLength = VARIABLE.rayLength * scale.magnitude;
                
                var hit = Physics2D.Raycast(VARIABLE.rayOrigin.position + adjustedOffset, VARIABLE.rayDirection.normalized, adjustedLength, VARIABLE.layerMask);
                Gizmos.color = hit ? VARIABLE.hitColor : VARIABLE.noHitColor;

                Gizmos.DrawLine(VARIABLE.rayOrigin.position + adjustedOffset, VARIABLE.rayOrigin.position + adjustedOffset + (Vector3)(VARIABLE.rayDirection.normalized * adjustedLength));
            }

    }
    

    bool ishit = false;
    
    //判断这些射线是否击中
    public bool IsHit{
        get
        {
            ishit = false;
            foreach (var VARIABLE in rays)
            {
                if (CastRay(VARIABLE))
                {
                    ishit = true;
                    return ishit;
                }
            }

            return ishit;
        }
    }

    public RaycastHit2D GetHit(string rayName)
    {
        var getray = rays.FirstOrDefault(p => p.rayName.Equals(rayName));
        if (!string.IsNullOrEmpty(getray.rayName))
        {
            return CastRay(getray);
        }
        return new RaycastHit2D();
    }

    private RaycastHit2D CastRay(ARay ray)
    {
        Vector3 scale = transform.localScale;
        Vector3 adjustedOffset = Vector3.Scale(ray.rayOriginOffset, scale);
        float adjustedLength = ray.rayLength * scale.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(ray.rayOrigin.position + adjustedOffset, ray.rayDirection.normalized, adjustedLength, ray.layerMask);
        Debug.DrawRay(ray.rayOrigin.position + adjustedOffset, ray.rayDirection.normalized * adjustedLength, hit ? ray.hitColor : ray.noHitColor);
        return hit;
    }


}
