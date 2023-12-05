using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Shadow3D : MonoBehaviour
{
    [SerializeField]
    private Vector2 ShadowOffset;
    [SerializeField] private GameObject shadowPrefab;
    private GameObject shadow;
    private bool isEnable = true;
    private void OnEnable()
    {
        SetEnable(true);
    }

    private void OnDisable()
    {
        SetEnable(false);
    }
    public void Update()
    {
        if (!isEnable) return;

        JudgeIsNull();
        shadow.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z) + (Vector3)ShadowOffset;
    }
    void JudgeIsNull()
    {
        if (shadow == null)
        {
            shadow = LeanPool.Spawn(shadowPrefab);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + (Vector3)ShadowOffset,0.2f);
    }
    void ReleaseShadow()
    {
        if (shadow != null)
        {
            LeanPool.Despawn(shadow);
            shadow = null;
        }
    }
    
    
    public void SetEnable(bool va)
    {
        isEnable = va;

        if (shadow != null)
        {
            ReleaseShadow();
        }
    }
}
