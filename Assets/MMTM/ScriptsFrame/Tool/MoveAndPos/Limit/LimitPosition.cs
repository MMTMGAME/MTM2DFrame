using System;
using System.Collections.Generic;
using MMTM;
using NaughtyAttributes;
using UnityEngine;

public class LimitPosition : MonoBehaviour
{
    
    [SerializeField]
    [Label("当2.5D游戏时候限制Z轴")] private bool IsLimitZ = false;

    [SerializeField]
    private bool IsLimitY;
    
    [SerializeField]
    private bool IsLimitX;

    private IPosition position;
    private List<Vector2> nowLimit;
    private void Awake()
    {
        position = GetComponent<IPosition>();
    }

    private void Start()
    {
        nowLimit = LimitPositionManager.instance.NowLimit;
    }

    private void Update()
    {
        position.Position = ProjectPointToPolygon(position.Position,nowLimit);
        position.ApplyRealPosition(position.Position);
    }

    private Vector2 beforePos;
    
    
    public static Vector2 ProjectPointToPolygon(Vector2 point, List<Vector2> polygon)
    {
        if (IsPointInPolygon(point, polygon))
        {
            return point; // 点已经在多边形内部
        }

        float minDistance = float.MaxValue;
        Vector2 closestPoint = point;

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % polygon.Count];

            Vector2 projectedPoint = ProjectPointToLineSegment(point, a, b);
            float distance = Vector2.Distance(point, projectedPoint);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = projectedPoint;
            }
        }

        return closestPoint;
    }

    private static Vector2 ProjectPointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 lineSegment = b - a;
        Vector2 projected = Project(point - a, lineSegment);
        return a + Vector2.ClampMagnitude(projected, lineSegment.magnitude);
    }

    public static bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
    {
        int intersections = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % polygon.Count];

            if (IsIntersecting(point, new Vector2(Mathf.Max(a.x, b.x), point.y), a, b))
            {
                intersections++;
            }
        }

        return (intersections % 2 != 0);
    }

    private static bool IsIntersecting(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
        if (denominator == 0)
        {
            return false;
        }

        float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
        float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

        return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
    }
    
    
    private static Vector2 Project(Vector2 a, Vector2 b)
    {
        float dotProduct = Vector2.Dot(a, b);
        float lengthSquared = Vector2.Dot(b, b);
        return (dotProduct / lengthSquared) * b;
    }
}
