using NaughtyAttributes;
using UnityEngine;

public class OrderInLayerRandom : MonoBehaviour
{
    public static LayerSelector _LayerSelector;

    [InfoBox("随机OrderInLayer,但是只针对单层,如果一个整体有不同的各种render请用OrderInlayerRandomRange")]
    [Label("开启自选")]
    [SerializeField] private bool isSelect = false;
    
    [ShowIf("isSelect")]
    [SerializeField]
    private Renderer _render;


    private (int startLayer, int endLayer) getlayer;
    private void Awake()
    {
        if(_LayerSelector == null) _LayerSelector = new LayerSelector();

        if (!isSelect) isSelect = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        getlayer = _LayerSelector.GetLayerRange(1);
        SetOrder(getlayer.startLayer);
    }

    private void OnDisable()
    {
        _LayerSelector.ReleaseLayerRange(getlayer.startLayer,getlayer.endLayer);
    }


    public void SetOrder(int order)
    {
        _render.sortingOrder = order;
    }
}
