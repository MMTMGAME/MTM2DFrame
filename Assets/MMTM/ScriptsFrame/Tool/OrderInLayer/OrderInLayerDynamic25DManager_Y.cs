using UnityEngine;
using System.Collections.Generic;
using MMTM;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class OrderInLayerDynamic25DManager_Y : MonoBehaviour
{
   
    private List<Renderer> _spriteRenderers;
    [InfoBox("根据Y轴来动态变化自身的OrderInLayer")]
    [Label("开启")]
    [SerializeField] private bool enableLayerSelect = true;
    [SerializeField]
    [Label("开启自选")]
    private bool enableSelfLayer = false;
    [ShowIf("enableSelfLayer")]
    [Label("添加需要调整的层级")]
    [SerializeField] private List<Renderer> selfSelect;

    [Label("一次性检测")]
    [SerializeField]
    private bool isOnce = false;
    [SerializeField]
    [Label("忽略2.5D中的Z轴也就是高度,有时候静态物体就不需要高度检测")]
    private bool IgnorZ = false;
    private int runtimes = 0;

    private IPosition _position;

    public void SetSortingLayer(string sortingLayerName)
    {
        foreach (var VARIABLE in selfSelect)
        {
            VARIABLE.sortingLayerName = sortingLayerName;
        }
    }

    public void SetSortingOrder(int sortingOrder)
    {
        foreach (var VARIABLE in selfSelect)
        {
            VARIABLE.sortingOrder = sortingOrder;
        }
    }

    
    private void Awake()
    {
        _spriteRenderers = new List<Renderer>();
        _position = GetComponent<IPosition>();
        if (!enableSelfLayer)
        {
            var childs = MyTools.GetAllChildrenDepth(transform, 1);
            foreach (var VARIABLE in childs)
            {
                if (VARIABLE.TryGetComponent(out Renderer get))
                {
                    _spriteRenderers.Add(get);
                }
            }
        }
    }
    void Update()
    {
        UpdateLayer();
    }

    public void UpdateLayer()
    {
        if(isOnce && runtimes >= 1)return;
        if(!enableLayerSelect)return;

        if (!enableSelfLayer)
        {
            foreach (var VARIABLE in _spriteRenderers)
            {
                int remap;
                if(!IgnorZ) remap = MyTools.MyMath.RemapInt((-1 * _position.Position.y) * 10000, -100000, 100000, -32767, 32767);
                else remap = MyTools.MyMath.RemapInt((-1 * VARIABLE.transform.position.y) * 10000, -100000, 100000, -32767, 32767);
                
                VARIABLE.sortingOrder = remap;
            }
        }
        else
        {
            foreach (var VARIABLE in selfSelect)
            {
                int remap;
                if(!IgnorZ) remap = MyTools.MyMath.RemapInt((-1 * _position.Position.y) * 10000, -100000, 100000, -32767, 32767);
                else remap = MyTools.MyMath.RemapInt((-1 * VARIABLE.transform.position.y) * 10000, -100000, 100000, -32767, 32767);
                
                VARIABLE.sortingOrder = remap;
            }
        }

        runtimes++;
    }

    public void SetLayerSelectEnable(bool va)
    {
        enableLayerSelect = va;
    }

    private void OnEnable()
    {
        enableLayerSelect = true;
    }

}

public class LayerSelector
{
    private List<(int start, int end)> freeLayerRanges;
    public LayerSelector()
    {
        freeLayerRanges = new List<(int start, int end)>();
        freeLayerRanges.Add((0, 10000 - 1));
    }

    public (int startLayer, int endLayer) GetLayerRange(int rangeSize)
    {
        for (int i = 0; i < freeLayerRanges.Count; i++)
        {
            var range = freeLayerRanges[i];
            int rangeSizeAvailable = range.end - range.start + 1;

            if (rangeSizeAvailable >= rangeSize)
            {
                int startLayer = range.start;
                int endLayer = startLayer + rangeSize - 1;

                // Update the free layer range.
                if (rangeSizeAvailable == rangeSize)
                {
                    // If the requested size exactly matches the available range, remove it.
                    freeLayerRanges.RemoveAt(i);
                }
                else
                {
                    // Otherwise, update the start of the available range.
                    freeLayerRanges[i] = (startLayer + rangeSize, range.end);
                }

                return (startLayer, endLayer);
            }
        }

        Debug.LogError("No enough layers for the range.");
        return (0, 0);
    }

    public void ReleaseLayerRange(int startLayer, int endLayer)
    {
        // Add the released range back to the free layer range list.
        freeLayerRanges.Add((startLayer, endLayer));

        // Optional: Merge adjacent free ranges for simplicity. This can be a separate method to be called periodically.
        MergeFreeRanges();
    }

    private void MergeFreeRanges()
    {
        // Sort the ranges by start layer.
        freeLayerRanges.Sort((a, b) => a.start.CompareTo(b.start));

        for (int i = 0; i < freeLayerRanges.Count - 1; i++)
        {
            var range1 = freeLayerRanges[i];
            var range2 = freeLayerRanges[i + 1];

            // If range1's end is adjacent to range2's start, merge them.
            if (range1.end == range2.start - 1)
            {
                freeLayerRanges[i] = (range1.start, range2.end);
                freeLayerRanges.RemoveAt(i + 1);
                i--; // Repeat the loop for the same index because the list has changed.
            }
        }
    }
}