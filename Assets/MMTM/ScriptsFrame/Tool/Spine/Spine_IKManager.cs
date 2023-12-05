using System;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;

public interface IIkManager
{
    void SetIkPosition(string name, Vector3 pos);
}

public class Spine_IKManager : MonoBehaviour,IIkManager
{
    private ISkeletonAnimation _iSkeletonAnimation;

    [Serializable]
    public struct IkPoint
    {
        public string iKName;
        public Vector2 offsetPos;
    }
    [Serializable]
    public struct IkCombine
    {
        public string name;
        public List<IkPoint> ikPoints;
    }
    
    [SerializeField]
    private List<IkCombine> allIk;
    private Dictionary<string, List<IkPoint>> ikManager;
    private void Awake()
    {
        _iSkeletonAnimation = GetComponent<ISkeletonAnimation>();
        ikManager = allIk.ToDictionary(p => p.name, p => p.ikPoints);

        _iSkeletonAnimation.UpdateLocal += AfterUpdateComplete;
    }

    private string nowName;
    private Vector3 nowWorldPos;
    private bool isOpenIk;
    public void SetIkPosition(string name,Vector3 worldPos)
    {
        nowName = name;
        nowWorldPos = worldPos;
        isOpenIk = true;
    }
    void AfterUpdateComplete (ISkeletonAnimation anim) {
        var ikPoints = ikManager[nowName];
        foreach (var ikname in ikPoints)
        {
            var target = _iSkeletonAnimation.Skeleton.FindIkConstraint(ikname.iKName).Target;
            Transform spineGameObjectTransform = transform;// 获取 Spine GameObject 的 Transform 组件
            Vector2 skeletonSpacePos = spineGameObjectTransform.InverseTransformPoint(nowWorldPos + (Vector3)ikname.offsetPos);// 将世界坐标转换为骨架空间坐标
            target.SetPositionSkeletonSpace(skeletonSpacePos);
        }
    }
}
