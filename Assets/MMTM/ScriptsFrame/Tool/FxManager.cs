using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class FxManager : MonoBehaviour,IOnDespawn
{
    [Serializable]
    public struct WillSpawnFx
    {
        public string name;
        public GameObject fxPrefab;
        /// <summary>
        /// 生成的位置
        /// </summary>
        [Label("生成位置")]
        public Transform spawnTransfom;

        public Vector3 spwanPosOffset;
        [Label("跟随移动")]
        public bool isFollow;
        [Label("跟随消失")]
        public bool isForget;
    }

    [Label("自带特效")]
    [SerializeField]
    private bool isHaveFx;
    [ShowIf("isHaveFx")]
    public List<GameObject> existFx;

    [Label("要生成并的特效")]
    public List<WillSpawnFx> WillSpawnFxes;
    private List<GameObject> fxes;
    private void Awake()
    {
        fxes = new();
    }

    private void OnEnable()
    {
        SetExistFxActive(true);
    }
    public void SetExistFxActive(bool isActive)
    {
        foreach (var VARIABLE in existFx)
        {
            VARIABLE.SetActive(isActive);
        }
    }

    public void SpawnFx(string fxName)
    {
        var willspawn = WillSpawnFxes.FirstOrDefault(p => p.name.Equals(fxName));
        if (willspawn.name.Length < 0)
        {
            return;
        }
        var getfx = LeanPool.Spawn(willspawn.fxPrefab, willspawn.spawnTransfom.position + willspawn.spwanPosOffset, Quaternion.identity);
        if(willspawn.isFollow)getfx.transform.parent = willspawn.spawnTransfom;
        if(!willspawn.isForget)fxes.Add(getfx);
    }

    public void OnDespawn()
    {
        foreach (var fx in fxes)
        {
            LeanPool.Despawn(fx);
        }
        fxes.Clear();
    }

    public Action onDespawn { get; set; }
    public Action onDespawnClear { get; set; }
}
