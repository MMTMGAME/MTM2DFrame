using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Boom : MonoBehaviour,IOnDespawn
{
    [SerializeField] private float boomRadius = 4;
    [SerializeField] private float boomDamage = 20;
    [SerializeField] private Transform boomCenter;
    [SerializeField] private LayerMask aimLayer;

    [SerializeField]
    private bool EnableBoom;
    public void NowBoom()
    {
        // 获取爆炸范围内的所有碰撞器
        var colliders = Physics2D.OverlapCircleAll(boomCenter.position, boomRadius, aimLayer);
        // 遍历所有碰撞器，为受影响的物体施加爆炸力
        foreach (var hitCollider in colliders)
        {
            if (PrefabCenter.Instance.TryGetInteface(hitCollider.gameObject.GetInstanceID(), out IHealth health))
            {
                if (health != null && health.CurrentHealth > 0)
                {
                    if (health.Dodelta(boomDamage) && PrefabCenter.Instance.GetIntefaceComponent<IGetAttacked>(hitCollider.gameObject.GetInstanceID())[0].GetAttack(new IGetAttacked.AttackerData(gameObject.GetInstanceID(),boomDamage, transform.position, Vector2.down)))
                    {

                    }
                }
            }

        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(boomCenter.position,boomRadius);
    }

    public void OnDespawn()
    {
        if (EnableBoom) NowBoom();
    }

    public Action onDespawn { get; set; }
    public Action onDespawnClear { get; set; }
}
// if (PrefabCenter.Instance.TryGetGameObj(out Entity entity, hitCollider.gameObject.GetInstanceID()))
// {
//     Vector2 explosionDirection = Vector2.zero;
//     explosionDirection = (Vector2)hitCollider.transform.position - (Vector2)boomCenter.position;
//     float distance = explosionDirection.magnitude;
//     float force = boomForce * (1 - (distance / boomRadius));
//     (entity.prefab as IGetAttacked)?.GetAttack(
//         boomDamage,
//         new IGetAttacked.AttackerData(gameObject.GetInstanceID(),
//             boomDamage,
//             hitCollider.transform.position,
//             explosionDirection,
//             force
//         ));
// }