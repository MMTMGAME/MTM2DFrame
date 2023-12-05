using MMTM;
using NaughtyAttributes;
using UnityEngine;


public class OrderInlayerDymic2D_Y : MonoBehaviour
{
    [SerializeField]
    [Label("是否开启")]
    bool isEnable = true;    
    
    [Label("一次性检测")]
    [InfoBox("Spine和原版都可以用")]
    [SerializeField]
    private bool isOnce = false;
    [SerializeField]
    [Label("忽略Z,如果没有IPosition的物体可以选择")]
    private bool IgnorZ = false;
    private int runtimes = 0;
    
    [SerializeField]
    [Label("开启自选")]
    private bool enableSelfLayer = false;
    
    [ShowIf("enableSelfLayer")]
    [SerializeField]
    private Renderer _skeletonAnimation;
    private IPosition _position;
    private void Awake()
    {
        if(!enableSelfLayer) _skeletonAnimation = GetComponent<Renderer>();
        if(!IgnorZ)_position = GetComponent<IPosition>();
    }
    void Update()
    {
        UpdateLayer();
    }

    public void UpdateLayer()
    {
        if(!isEnable)return;
        
        if(isOnce && runtimes >= 1)return;

        int remap;
        if(!IgnorZ) remap = MyTools.MyMath.RemapInt((-1 * _position.Position.y) * 10000, -100000, 100000, -32767, 32767);
        else remap = MyTools.MyMath.RemapInt((-1 * transform.position.y) * 10000, -100000, 100000, -32767, 32767);
                
        _skeletonAnimation.sortingOrder = remap;
        
        runtimes++;
    }

    public void SetEnable(bool va)
    {
        isEnable = va;
    }
}
