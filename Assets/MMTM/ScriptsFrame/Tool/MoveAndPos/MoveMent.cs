using UnityEngine;
public class MoveMent : MonoBehaviour, IMoveMent
{
    [SerializeField] private Vector2 direction;
    public Vector3 Direction => direction;

    [SerializeField] private float speed;
    public float Speed => speed;

    [SerializeField] private bool enable = true;

    public void SetDirection(Vector2 direction_)
    {
        direction = direction_;
    }

    public void SetDirection(Vector2 direction_, float speed_)
    {
        direction = direction_;
        speed = speed_;
    }

    public void SetEnable(bool enable_)
    {
        enable = enable_;
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void Move()
    {
        Move(direction, speed);
    }
    public void SetRandomDirection()
    {
        direction = new Vector3(Random.Range(-1, 1f), Random.Range(-1, 1f), 0);
    }
    public void Move(Vector3 direction_, float speed_)
    {
        direction = direction_;
        transform.position += direction_ * speed_ * Time.deltaTime ;
    }
}