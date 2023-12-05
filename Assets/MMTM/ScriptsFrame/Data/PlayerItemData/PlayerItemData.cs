using System;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;

[Serializable]
public struct RuntimeItemData
{
    [HideInInspector]
    public int slotIndex;
    public string itemName;
    /// <summary>
    /// 使用次数
    /// </summary>
    public int usetimes;
    public List<IWearSkin.Cloth> skins;
    /// <summary>
    /// 猫咪是否绝育
    /// </summary>
    public bool catIsNeutered;

    public RuntimeItemData(int slotIndex, string itemName, List<IWearSkin.Cloth> skins = null, int usetimes_ = 1, bool catIsNeutered_ = false)
    {
        this.slotIndex = slotIndex;
        this.itemName = itemName;
        this.skins = skins;
        usetimes = usetimes_;
        catIsNeutered = catIsNeutered_;
    }
    public RuntimeItemData(string itemName, List<IWearSkin.Cloth> skins = null,int usetimes_ = 1)
    {
        this.slotIndex = -1;
        this.itemName = itemName;
        this.skins = skins;
        usetimes = usetimes_;
        catIsNeutered = false;
    }


}
public static class RuntimeItemDataTool
{
    /// <summary>
    /// 把数据放入目标
    /// </summary>
    /// <param name="runtimeItemData"></param>
    /// <param name="InstanceId"></param>
    public static void CopyItemData(this RuntimeItemData runtimeItemData,int InstanceId)
    {
        var wear = PrefabCenter.Instance.GetInteface<IWearSkin>(InstanceId);
        var iteD = PrefabCenter.Instance.GetInteface<ISetItemData<RuntimeItemData>>(InstanceId);
        
        wear?.WearClothes(runtimeItemData.skins);
        iteD?.SetData(runtimeItemData);
        
    }
}

[CreateAssetMenu(fileName = "PlayerItemData", menuName = "ScriptableObject/玩家身上数据", order = 0)]
public class PlayerGetItemGetItemGetItemData : ScriptableObject,IGetItemData<int,string>,IGetItemData<int,RuntimeItemData>,IGetItemData<List<RuntimeItemData>>,IGetItemData<string[]>,IGetItemData<RuntimeItemData[]>
{

    [SerializeField]
    private List<RuntimeItemData> listitems;

    public string GetData(int k)
    {
        while (listitems.Count < k + 1)
        {
            listitems.Add(new RuntimeItemData("",null));
        }
        return listitems[k].itemName;
    }

    public bool SetData(int k, RuntimeItemData v)
    {
        if (k < 0) return false;
        while (listitems.Count < k + 1)
        {
            listitems.Add(new RuntimeItemData("",null));
        }

        listitems[k] = v;
        onChange?.Invoke();
        return true;

    }
    RuntimeItemData IGetItemData<int, RuntimeItemData>.GetData(int k)
    {
        if (k < 0) return new RuntimeItemData();
        while (listitems.Count < k + 1)
        {
            listitems.Add(new RuntimeItemData("",null));
        }

        return listitems[k];
    }
    
    
    public bool SetData(int k, string v)
    {
        if (k < 0) return false;
        while (listitems.Count < k + 1)
        {
            listitems.Add(new RuntimeItemData("",null));
        }

        if (listitems[k].itemName.Length==0 || (listitems[k].itemName.Length>0 && v.Length==0))
        {
            listitems[k] = new RuntimeItemData(v,null);
            onChange?.Invoke();
            return true;
        }
        return true;
    }



    public Action onChange { get; set; }
    public int GetEmpty()
    {
        for (int index = 0; index < listitems.Count; index++)
        {
            if (listitems[index].itemName.Length ==0) return index;
        }
        return -1;
    }

    public List<RuntimeItemData> GetData()
    {
        return listitems;
    }

    public void SetData(RuntimeItemData[] v)
    {
        throw new NotImplementedException();
    }

    public void SetData(string[] v)
    {
        MyDebug.Log("清除");
        for (int i = 0; i < listitems.Count; i++)
        {
            listitems[i] = new RuntimeItemData(slotIndex:0,"", null, 0);
        }
    }

    public void SetData(List<RuntimeItemData> v)
    {
        listitems = v;
    }
    

    string[] IGetItemData<string[]>.GetData()
    {
        return listitems.Select(p => p.itemName).ToArray();
    }

    RuntimeItemData[] IGetItemData<RuntimeItemData[]>.GetData()
    {
        return listitems.ToArray();
    }
}