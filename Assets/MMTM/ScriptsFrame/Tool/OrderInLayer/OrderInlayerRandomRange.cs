using System.Collections.Generic;
using System.Linq;
using MMTM;
using NaughtyAttributes;
using UnityEngine;

public class OrderInlayerRandomRange : MonoBehaviour
{
    
    // public static LayerSelector _LayerSelector;
    //
    // [InfoBox("在范围之内随机OrderInLayer,如果想给单独某个render添加偏移那么给那个render添加一个OffsetOrderInLayer")] [Label("需要随机的目标")] [SerializeField]
    // private List<Renderer> _renders;
    //
    //
    // [SerializeField] private int layerRangeDistance;
    //
    // [SerializeField] private Vector2Int selfLayerRange;
    // [SerializeField] private Vector2Int randomLayer;
    //
    // private void Awake()
    // {
    //     MyDebug.Log($"_LayerSelector:{_LayerSelector==null}");
    //     if (_LayerSelector == null)
    //     {
    //         _LayerSelector = new LayerSelector();
    //
    //     }
    //
    //     var get = _renders.OrderBy(r => r.sortingOrder);
    //
    //     selfLayerRange =new Vector2Int(get.First().sortingOrder, get.Last().sortingOrder);
    //     layerRangeDistance = selfLayerRange.y - selfLayerRange.x;
    //     
    // }
    //
    // private void OnEnable()
    // {
    //     var getlayer_ = _LayerSelector.GetLayerRange(layerRangeDistance);
    //     randomLayer = new Vector2Int(getlayer_.startLayer, getlayer_.endLayer);
    //     
    //     SetOrder(randomLayer.x,randomLayer.y);
    // }
    //
    // private void OnDisable()
    // {
    //     _LayerSelector.ReleaseLayerRange(randomLayer.x,randomLayer.y);
    // }
    //
    //
    // public void SetOrder(int startLayer, int endLayer)
    // {
    //     foreach (var VARIABLE in _renders)
    //     {
    //         VARIABLE.sortingOrder = MyTools.MyMath.RemapInt(VARIABLE.sortingOrder,selfLayerRange.x,selfLayerRange.y,startLayer, endLayer);
    //         var instance = VARIABLE.gameObject.GetInstanceID();
    //         if (PrefabCenter.Instance.TryGetInteface(instance, out IOffsetOrderInLayer offsetOrderInLayers))
    //         {
    //             offsetOrderInLayers.SetDefaultOffsetLayer();
    //         }
    //     }
    // }


}
