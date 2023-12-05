using System;
using NaughtyAttributes;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Spine_SkinOnEquiped : MonoBehaviour,IOnEquiped
{
    [InfoBox("用来给某个装备装备时，给使用装备的人添加spin皮肤的")]

    [SerializeField]
    private IWearSkin.Cloth clothName;
    //
    
    [SerializeField]
    private Sprite skin;
    public event Action onEquiped;
    public event Action onUnEquiped;
    private int equiperid;

    private Slot _slot;
    private Material material;
    public void OnEquiped(int equiper, string selfName)
    {
        DestroyMaterial();
        equiperid = equiper;
        
        _slot = PrefabCenter.Instance.GetInteface<IWearSkin>(equiperid).WearCloth(clothName);
    }

    public void OnUnEquiped()
    {
        PrefabCenter.Instance.GetInteface<IWearSkin>(equiperid).RemoveCloth(clothName);
        _slot.SetToSetupPose();
    }

    private void OnDestroy()
    {
        DestroyMaterial();
    }

    void DestroyMaterial()
    {
        if (material != null)
        {
            DestroyImmediate(material);
            material = null;
        }
    }
}
