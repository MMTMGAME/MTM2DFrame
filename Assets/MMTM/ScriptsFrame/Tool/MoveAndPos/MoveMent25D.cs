using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public interface IMoveMent
{
    Vector3 Direction { get;}
    void SetDirection(Vector3 dir);
    public void Move();
    public void Move(Vector3 direction_, float speed_);
    public float Speed { get; }

    void SetRandomDirection();
}

/// <summary>
/// 2.5D移动
/// </summary>
public class MoveMent25D : MonoBehaviour,IMoveMent
{
        
    [SerializeField]
    private Vector3 direction;
    public Vector3 Direction => direction;
    [SerializeField]private float speed;
    public float Speed => speed;


    [HorizontalLine(color:EColor.Green)]
    public UnityEvent OnMove;

    private IPosition position;

    private void Awake()
    {
        position = GetComponent<IPosition>();
    }

    private void OnEnable()
    {
        position.ApplyRealPosition(transform.position);
    }
    public void SetRandomDirection()
    {
        direction = new Vector3(Random.Range(-1, 1f), Random.Range(-1, 1f), 0).normalized;
    }
    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void Move()
    {
        Move(direction, speed);
    }
    public void Move(Vector3 direction_,float speed_)
    {
        position.ApplyRealPosition(position.Position + direction_ * speed_ * Time.deltaTime);
    }

}
