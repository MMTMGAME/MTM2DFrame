using Lean.Pool;
using UnityEngine;

public interface IShadow
{
    public void SetEnable(bool va);
}

public class Shadow25D : MonoBehaviour,IShadow
{
    [SerializeField]
    private Vector2 ShadowOffset;
    [SerializeField] private GameObject shadowPrefab;
    private GameObject shadow;
    private IPosition _position;
    private bool isEnable = true;
    private void Awake()
    {
        _position = GetComponent<IPosition>();
    }
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
        shadow.transform.position = _position.Position + (Vector3)ShadowOffset;
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
