using System;
using System.Threading;
using Cysharp.Threading.Tasks;

using NaughtyAttributes;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    [CurveRange(0,0,1,1f)]
    private AnimationCurve _curve;
    /// <summary>
    /// 让玩家接触物品时的行为
    /// </summary>
    public enum InteractType
    {
        ZoomIn,//放大
    }
    private Vector3 defaultScale;


    
    private CancellationTokenSource zoomInToken;

    private void Start()
    {
        defaultScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ZoomIn().Forget();
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        ZoomOut().Forget();
    }

    async UniTaskVoid ZoomIn()
    {
        if (zoomInToken != null && !zoomInToken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            zoomInToken.Cancel();
            zoomInToken.Dispose();
        }

        zoomInToken = new CancellationTokenSource();
        var zoominResult = defaultScale * 1.2f;
        float t = 0;
        while (t <=1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, zoominResult,_curve.Evaluate(t));
            await UniTask.Delay(TimeSpan.FromTicks(1), cancellationToken: zoomInToken.Token);
            t += Time.deltaTime * speed;
        }
    }
    
    
    async UniTaskVoid ZoomOut()
    {
        if (zoomInToken != null && !zoomInToken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            zoomInToken.Cancel();
            zoomInToken.Dispose();
        }

        zoomInToken = new CancellationTokenSource();
        var zoominResult = defaultScale * 1.2f;
        float t = 0;
        while (t <=1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale,_curve.Evaluate(t));
            await UniTask.Delay(TimeSpan.FromTicks(1), cancellationToken: zoomInToken.Token);
            t += Time.deltaTime * speed;
        }
    }
}
