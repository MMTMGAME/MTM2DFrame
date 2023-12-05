using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(RegistPrefab))]
public class FollowOrderInLayer : MonoBehaviour
{
    private Renderer aimRender;
    public int offsetLayer;
    [ReadOnly]
    public int defaultOffsetLayer;

    private CancellationTokenSource updateOffsetToken;

    private void Awake()
    {
        aimRender = GetComponent<Renderer>();
    }

    public void SetDefaultOffsetLayer()
    {
        defaultOffsetLayer = aimRender.sortingOrder;
        UpdateOffsetLayer().Forget();
    }

    async UniTaskVoid UpdateOffsetLayer()
    {
        if (updateOffsetToken != null && !updateOffsetToken.Token.IsCancellationRequested)
        {
            // 可以选择取消现有的协程
            updateOffsetToken.Cancel();
            updateOffsetToken.Dispose();
        }
        updateOffsetToken = new CancellationTokenSource();

        while (true)
        {
            SyncLayer();
            await UniTask.Delay(TimeSpan.FromTicks(1), cancellationToken: updateOffsetToken.Token);
        }
    }

    void SyncLayer()
    {
        if (aimRender != null)
        {
            aimRender.sortingOrder = defaultOffsetLayer + offsetLayer;
        }
    }
}
