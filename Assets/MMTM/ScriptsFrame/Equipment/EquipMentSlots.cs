using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using NaughtyAttributes;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
public class EquipMentSlots : MonoBehaviour
{

    [Serializable]
    public struct EquipedData
    {
        public Slot Slot;
        public GameObject equipment;
        [FormerlySerializedAs("Cloth")] public IWearSkin.Cloth cloth;

        public EquipedData(Slot slot, GameObject equipment, IWearSkin.Cloth cloth)
        {
            Slot = slot;
            this.equipment = equipment;
            this.cloth = cloth;
        }
    }
    [SerializeField]
    private ItemDataResource itemDataResource;

    private IGetItemData<string,EquipData> equipmentsData;

    private List<EquipedData> now_equipments;

    private SkeletonUtility _skeletonUtility;
    private IWearSkin _wearSkin;
    
    
    [HorizontalLine(2f,EColor.Green)]
    [SerializeField]
    private UnityEvent OnEquipEvent;
    public event Action onEquip;

    [SerializeField]
    private UnityEvent OnUnEquipEvent;
    public event Action onUnEquip;


    [SerializeField]
    private string addEquipname;

    [Button("添加一件装备")]
    void AddEquipment()
    {
        Equip(addEquipname);
    }
    [Button("删除一件装备")]
    void RemoveEquipment()
    {
        UnEquip(NowEquipment);
    }

    private void Awake()
    {
        _wearSkin = GetComponent<IWearSkin>();
        _skeletonUtility = GetComponent<SkeletonUtility>();

        equipmentsData = itemDataResource as IGetItemData<string, EquipData>;
    }

    public GameObject Equip(string name)//Bug 装备的时候没有把skin数据插入进装备的ItemData
    {
        //寻找现在正在装备上的武器有没有名字相同的,有的话就别装备了
        var EqualsCount = -1;//TODO
        
        if(EqualsCount>0)return null;
        MyDebug.Log($"装备了:{name}");
        var weapondata = equipmentsData.GetData(name);
        if (weapondata.name == null ) return null;

        Slot slot = _wearSkin.WearCloth(weapondata.cloth);        //开始把武器的样子设置到皮肤上
        slot.ChangeMeshAttachment(weapondata.equipmentSprite);      //替换图片

        SkeletonUtilityBone getbone = null;//获取装备装在哪个骨骼上
        if (_skeletonUtility != null)
        {
            getbone = _skeletonUtility.boneComponents.FirstOrDefault(p => p.boneName.Equals(weapondata.boneName));
        }
    
        var equipment = LeanPool.Spawn(weapondata.equipMent, getbone != null?getbone.transform:transform);//生成武器
        equipment.transform.localPosition = weapondata.offsetPos;
        equipment.transform.localEulerAngles =  weapondata.offsetRot;//设置武器的各种相对位置什么的
        
        now_equipments.Add(new EquipedData(slot,equipment,weapondata.cloth));//插入数据以方便卸载装备
        
        if (PrefabCenter.Instance.TryGetIntefaceComponent(equipment.GetInstanceID(), out IOnEquiped[] weapons))//触发装备身上的所有被装备接口
        {
            foreach (var weapon in weapons)
            {
                weapon.OnEquiped(gameObject.GetInstanceID(),name);
            }
        }
        
        onEquip?.Invoke();
        OnEquipEvent?.Invoke();

        return equipment;
    }

    public GameObject UnEquip(GameObject equipment)
    {
        var data = now_equipments.Find(p => p.equipment.GetInstanceID() == equipment.GetInstanceID());
        if (PrefabCenter.Instance.TryGetIntefaceComponent(data.equipment.GetInstanceID(), out IOnEquiped[] weapons))//触发装备的装备事件
        {
            foreach (var weapon in weapons)
            {
                weapon.OnUnEquiped();
            }
        }
        
        _wearSkin.RemoveCloth(data.cloth);//移除那件衣服
        data.Slot.SetToSetupPose();//重置该槽位
 
        onUnEquip?.Invoke();
        OnUnEquipEvent?.Invoke();
        
        LeanPool.Despawn(data.equipment);
        now_equipments.Remove(data);
        
        return data.equipment;
    }

    public void InvokeAllAttackAbles()
    {
        foreach (var VARIABLE in now_equipments)
        {
            if(PrefabCenter.Instance.TryGetIntefaceComponent<IEquipment>(VARIABLE.equipment.GetInstanceID(),out var gets)){
                foreach (var action in gets)
                {
                    action.Attack();
                }
            }
        }
    }

    public void RemoveEquipMent(GameObject name)
    {
        UnEquip(name);
    }

    private void OnDestroy()
    {
        foreach (var VARIABLE in now_equipments)
        {
            VARIABLE.Slot.Attachment = null;
            VARIABLE.Slot.SetToSetupPose();
        }
    }

    public GameObject NowEquipment => now_equipments.FirstOrDefault().equipment;
}
