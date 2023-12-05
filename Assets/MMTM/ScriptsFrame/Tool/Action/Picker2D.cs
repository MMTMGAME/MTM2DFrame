using System;
using System.Collections.Generic;
using System.Linq;
using MMTM;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Picker2D : MonoBehaviour,IPicker
{
    [SerializeField]
    [Tag]
    private string _aimTag;
    
    [SerializeField]
    private LayerMask _aimlayerMask;
    
    [SerializeField]
    private float pickRange = 0.5f;
    [SerializeField]
    private Vector2 pickOffset;
    [SerializeField]
    [Label("同时检测周围的数量")]
    private int pickJudgeAmount = 6;
    private Collider2D[] getCloseAims;

    private IPosition _position;

    private IActionManager<ActionState> _actionManager;
    
    [SerializeField]
    private UnityEvent onPickAbleEvent;
    private void Awake()
    {
        _position = GetComponent<IPosition>();
        _actionManager = GetComponent<ActionManager>();
        getCloseAims = new Collider2D[pickJudgeAmount];
    }

    private void Start()
    {
        _actionManager.AddAction(ActionState.attack,Pick);
    }

    public void Pick()
    {
        getCloseAims.Clear();
        
        Physics2D.OverlapCircleNonAlloc(transform.position + (Vector3)pickOffset, pickRange, getCloseAims, _aimlayerMask);
        var closestCollider = getCloseAims
            .Where(collider => collider != null
                               && collider.gameObject.CompareTag(_aimTag)
                               && PrefabCenter.Instance.TryGetInteface(collider.gameObject.GetInstanceID(), out IPosition position)
                               && PrefabCenter.Instance.TryGetInteface(collider.gameObject.GetInstanceID(), out IPickedAble _))
            .OrderBy(collider => Vector3.Distance(_position.Position + (Vector3)pickOffset,
                PrefabCenter.Instance.GetInteface<IPosition>(collider.gameObject.GetInstanceID()).Position))
            .FirstOrDefault();

        if (closestCollider != null)
        {
            // 获取 IPickAble 接口
            if (PrefabCenter.Instance.TryGetInteface(closestCollider.gameObject.GetInstanceID(), out IPickedAble closestPickAble))
            {
                MyDebug.Log($"{gameObject.name} 拾取了 {getCloseAims[0].gameObject.name} 拾取成功 {closestPickAble.Picked(gameObject.GetInstanceID())}");
                // 此处处理 closestPickAble
                
                onPickAbleEvent?.Invoke();
            }
        }
    }
}
