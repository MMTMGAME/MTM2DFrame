using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IWearSkin
{
    /// <summary>
    /// 格子颜色
    /// </summary>
    [Serializable]
    public struct Cloth
    {
        public string name;
        public List<string> clothName;
        public Color clothColor;
    }
    
    [Serializable]
    public struct Clothes
    {
        public string suitName;
        public List<Cloth> clothes;
    }
    /// <summary>
    /// 穿上一件衣服
    /// </summary>
    /// <param name="clothes"></param>
    Slot WearCloth(Cloth clothes);
    
    /// <summary>
    /// 移除一件衣服
    /// </summary>
    /// <param name="clothes"></param>
    void RemoveCloth(Cloth clothes);
    
     /// <summary>
     /// 穿上套装
     /// </summary>
     /// <param name="suitName"></param>
    void WearSuit(string suitName);
     
     /// <summary>
     /// 移除套装
     /// </summary>
     /// <param name="suitName"></param>
    void RemoveSuit(string suitName);
     
     /// <summary>
     /// 穿上很多衣服
     /// </summary>
     void WearClothes(List<Cloth> cloths);
    
     /// <summary>
     /// 移除很多衣服
     /// </summary>
     void RemoveClothes(List<Cloth> cloths);


     List<string> GetAllSuits();
     /// <summary>
     /// 现在的皮肤
     /// </summary>
    List<Cloth> NowSkin { get; }

     /// <summary>
     /// 穿随机衣服
     /// </summary>
     void WearRandomSkin();

     /// <summary>
     /// 换皮肤的时候触发,返回一个现在的新皮肤
     /// </summary>
    event Action<List<Cloth>> onChangeCloth;
}
public class Spine_SkinManager : MonoBehaviour,IWearSkin
{
    [InfoBox("用来给某个装备装备时，给使用装备的人添加spin皮肤的")]
    [SerializeField]
    private ScriptableObject ClothData;
    
    private IGetItemData<string, List<IWearSkin.Cloth>> skinWardrobe;//衣柜
    private ISkeletonAnimation iSkeletonAnimation;
    private List<IWearSkin.Cloth> nowSkin;
    private Color colorTransprent = new Color(0,0,0,0);
    private void Awake()
    {
        iSkeletonAnimation = GetComponent<ISkeletonAnimation>();
        skinWardrobe = ClothData as IGetItemData<string, List<IWearSkin.Cloth>>;
        nowSkin = new List<IWearSkin.Cloth>();
    }

    public Slot WearCloth(IWearSkin.Cloth clothes)
    {
        Slot slot = null;
        var skin = iSkeletonAnimation.Skeleton.Skin;
        if(skin==null)skin = iSkeletonAnimation.Skeleton.Data.DefaultSkin;
        //通过名字获取到默认skin里的组合
        var Skinname = clothes.clothName[Random.Range(0, clothes.clothName.Count)];
        if (Skinname.Length > 0)///剔除空皮肤
        {
            var findskin = iSkeletonAnimation.Skeleton.Data.FindSkin(Skinname);
            slot = iSkeletonAnimation.Skeleton.Slots.Items[findskin.Attachments.FirstOrDefault().SlotIndex];
            skin.AddSkin(findskin);
            iSkeletonAnimation.Skeleton.SetSkin(skin);
            iSkeletonAnimation.Skeleton.SetSlotsToSetupPose();
        
            //可以获取到皮肤slot然后设置颜色'
            var color = clothes.clothColor == colorTransprent?Color.white:clothes.clothColor;
            iSkeletonAnimation.Skeleton.Slots.Items[findskin.Attachments.FirstOrDefault().SlotIndex].SetColor(color);
            
            nowSkin.Add(clothes);
        }
        
        onChangeCloth?.Invoke(nowSkin);

        return slot;
    }

    public void RemoveCloth(IWearSkin.Cloth clothes)
    {
        Skin skin = iSkeletonAnimation.Skeleton.Skin;
        if (skin == null)return;
        
        MyDebug.Log($"clothes:{clothes.clothName == null}");
        var entries = iSkeletonAnimation.Skeleton.Data.FindSkin(clothes.clothName[0]).Attachments;
        foreach (var entry in entries)
        {
            skin.RemoveAttachment(entry.SlotIndex,entry.Name);
        }//移除该slot下的所有Attachments
        
        
        iSkeletonAnimation.Skeleton.SetSkin(skin);
        iSkeletonAnimation.Skeleton.SetSlotsToSetupPose();
        
        nowSkin.Remove(clothes);
        
        onChangeCloth?.Invoke(nowSkin);
    }

    public void WearSuit(string suitName)
    {
        MyDebug.Log($"是不是空的:{skinWardrobe == null}");
        var suit = skinWardrobe.GetData(suitName);
        WearClothes(suit);
    }

    public void RemoveSuit(string suitName)
    {
        var suit = skinWardrobe.GetData(suitName);
        RemoveClothes(suit);
    }

    public void WearClothes(List<IWearSkin.Cloth> suitget)
    {
        var suit = suitget.ToList();
        Skin skin = new Skin("Clothes");
        var nowskin = iSkeletonAnimation.Skeleton.Skin;
        
        if(nowskin!=null)nowskin.CopySkin(skin);
            
        if(suit!=null && suit.Count>0)
        foreach (var cloth in suit)
        {
            var skinname = cloth.clothName[Random.Range(0, cloth.clothName.Count)];
            if (skinname.Length > 0) ///剔除空皮肤
            {
                skin.AddSkin(iSkeletonAnimation.Skeleton.Data.FindSkin(skinname));
                nowSkin.Add(cloth);
            }
        }
        
        iSkeletonAnimation.Skeleton.SetSkin(skin);
        iSkeletonAnimation.Skeleton.SetSlotsToSetupPose();
        
        foreach (var clothcolor in suit)
        {
            var skinname = clothcolor.clothName[0];
            if (skinname.Length > 0)
            {
                var color = clothcolor.clothColor == colorTransprent?Color.white:clothcolor.clothColor;
                iSkeletonAnimation.Skeleton.Slots.Items[iSkeletonAnimation.Skeleton.Data.FindSkin(skinname).Attachments.FirstOrDefault().SlotIndex].SetColor(color);
            }
        }
        
        onChangeCloth?.Invoke(nowSkin);
    }

    public void RemoveClothes(List<IWearSkin.Cloth> suit)
    {
        Skin skin = iSkeletonAnimation.Skeleton.Skin;
        if(skin == null)return;

        
        foreach (var skinname in suit)
        {
            var entries = iSkeletonAnimation.Skeleton.Data.FindSkin(skinname.clothName[Random.Range(0,skinname.clothName.Count)]).Attachments;
            foreach (var entry in entries)
            {
                skin.RemoveAttachment(entry.SlotIndex,entry.Name);
            }
            nowSkin.Remove(skinname);
        }
        iSkeletonAnimation.Skeleton.SetSkin(skin);
        iSkeletonAnimation.Skeleton.SetSlotsToSetupPose();
        
        onChangeCloth?.Invoke(nowSkin);
    }

    public List<string> GetAllSuits()
    {
        return (ClothData as IGetItemData<List<string>>).GetData();
    }
    
    public void WearRandomSkin()
    {
        var allsuits = GetAllSuits();
        if(allsuits.Count>0) WearSuit(allsuits[Random.Range(0, allsuits.Count)]);
    }
    

    public List<IWearSkin.Cloth> NowSkin => nowSkin;


    public event Action<List<IWearSkin.Cloth>> onChangeCloth;
}
