using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取
/// </summary>
/// <typeparam name="Value"></typeparam>
public interface IGetItemData<Value>
{
    Value GetData();
}

/// <summary>
/// 获取
/// </summary>
/// <typeparam name="Value"></typeparam>
public interface IGetItemData<Key,Value>
{
    Value GetData(Key k);
}

public interface ISetItemData<Value>
{
    bool SetData(Value v);
}

public interface ISetItemData<Key, Value>
{
    bool SetData(Key k, Value v);
}

[Serializable]
public struct EquipData
{
    public string name;
    /// <summary>
    /// 把武器装在哪个骨头上
    /// </summary>
    public string boneName;
    public Vector3 offsetPos;
    public Vector3 offsetRot;
    public int layerOffset;
    /// <summary>
    /// 显示哪个部件,这个不显示的话就不知道武器这张图片替换到哪个部位了
    /// </summary>
    public IWearSkin.Cloth cloth;
    public Sprite equipmentSprite;
    public GameObject equipMent;
}
[Serializable]
public struct OnGroundData
{
    
}

//数据分为地上的,手上的,物品栏上的
[Serializable]
public struct ItemData
{
    public UIData uiData;
    public EquipData equipData;
    private OnGroundData onGroundData;
}

public struct UIData
{
    public string name;
    public string info;
    public Sprite uiIcon;
    public GameObject Prefab;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/所有物品数据", order = 0)]
public class ItemDataResource : ScriptableObject
{
    public List<ItemData> allItemData;
    private Dictionary<string, ItemData> AllItemDataDic;
}

