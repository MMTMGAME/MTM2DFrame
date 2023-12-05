using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "SkinWardrobe", menuName = "ScriptableObject/衣柜", order = 0)]
public class SkinGetItemGetItemData :ScriptableObject,IGetItemData<string,List<IWearSkin.Cloth>>,IGetItemData<List<string>>
{
    [SerializeField]
    private List<IWearSkin.Clothes>  SuitItemList;
    
    public List<IWearSkin.Cloth> GetData(string k)
    {
        return SuitItemList.FirstOrDefault(p => p.suitName.Equals(k)).clothes;
    }

    public bool SetData(string k, List<IWearSkin.Cloth> v)
    {
        return false;
    }

    public Action onChange { get; set; }
    public string GetEmpty()
    {
        return "";
    }

    public List<string> GetData()
    {
        return SuitItemList.Select(p => p.suitName).ToList();
    }

    public void SetData(List<string> v)
    {
        
    }
}
