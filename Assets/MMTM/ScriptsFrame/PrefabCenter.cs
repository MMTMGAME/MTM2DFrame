
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using UnityEngine.Networking;
using VContainer;
using Object = UnityEngine.Object;

public class PrefabCenter:IAsyncInit
{
    private Dictionary<int, Dictionary<Type, List<Component>>> allPrefabs;
    private Dictionary<string, GameObject> allPrefabToPools;

    
    
    public static PrefabCenter Instance;
    private IObjectResolver _container;
    
    
    
    public PrefabCenter(IObjectResolver container)
    {
        _container = container;
        allPrefabs = new();
        allPrefabToPools = new();
        Instance = this;
    }

    public async UniTask AsyncInit()
    {
        var abprefabreadPath = System.IO.Path.Combine(Application.streamingAssetsPath, "addtopool");
        AssetBundle abprefabread = null;
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(abprefabreadPath))
        {
            await uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                abprefabread = DownloadHandlerAssetBundle.GetContent(uwr);
                var prefabs = abprefabread.LoadAllAssets<GameObject>();

                var abgroundPath = System.IO.Path.Combine(Application.streamingAssetsPath, "ground");
                AssetBundle abground = null;
                using (UnityWebRequest uwrGround = UnityWebRequestAssetBundle.GetAssetBundle(abgroundPath))
                {
                    await uwrGround.SendWebRequest();
                    if (uwrGround.result == UnityWebRequest.Result.Success)
                    {
                        abground = DownloadHandlerAssetBundle.GetContent(uwrGround);
                        var grounds = abground.LoadAllAssets<GameObject>();
                        var all = prefabs.Concat(grounds);
                        foreach (var prefab in all)
                        {
                            //TODO:需要命名
                            MyDebug.Log($"加载物品:{prefab.gameObject.name}");
                            allPrefabToPools.Add(prefab.gameObject.name, prefab.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogError(uwrGround.error);
                    }
                }
            }
            else
            {
                Debug.LogError(uwr.error);
            }
        }
    }
    public GameObject GetPrefab(string name)
    {
        return allPrefabToPools[name];
    }
    public T[] GetIntefaceComponent<T>(int instanceId)
    {
        if (allPrefabs.TryGetValue(instanceId, out var componentsDict))
        {
            if (componentsDict.TryGetValue(typeof(T), out var component))
            {
                return component.Cast<T>().ToArray();
            }
        }
        return null; 
    }
    public T GetInteface<T>(int instanceId)
    {
        if (allPrefabs.TryGetValue(instanceId, out var componentsDict))
        {
            if (componentsDict.TryGetValue(typeof(T), out var component))
            {
                return component.Cast<T>().FirstOrDefault();
            }
        }
        return default; 
    }
    public bool TryGetComponent<T>(int instanceId,out T get)
    {
        if (allPrefabs.TryGetValue(instanceId, out var componentsDict))
        {
            if (componentsDict.TryGetValue(typeof(T), out var component))
            {
                get = component.Cast<T>().FirstOrDefault();
                return true;
            }
        }

        get = default;
        return false;
    }
    public T GetComponent<T>(int instanceId) where T:Component
    {
        if (allPrefabs.TryGetValue(instanceId, out var componentsDict))
        {
            if (componentsDict.TryGetValue(typeof(T), out var component))
            {
                return component.Cast<T>().FirstOrDefault();;
            }
        }
        return null;
    }
    public bool TryGetIntefaceComponent<T>(int instanceId, out T[] component)
    {
        component = GetIntefaceComponent<T>(instanceId);
        return component != null;
    }
    public bool TryGetInteface<T>(int instanceId,out T get)
    {
        var getlis = GetIntefaceComponent<T>(instanceId);
        if (getlis != null && getlis.Length > 0)
        {
            get = getlis[0];
            return true;
        }
        else
        {
            get = default;
            return false;
        }
    }
    
    public GameObject GetGameObj(int instanceId)
    {
        return GetIntefaceComponent<Transform>(instanceId)[0].gameObject;
    }

    public bool TryGetGameObj(int instanceId, out GameObject gameObj)
    {
        gameObj = GetGameObj(instanceId);
        return gameObj != null;
    }

    public void RegisterPrefab(MonoBehaviour prefab)=>RegisterPrefab(prefab.gameObject);
    public void RegisterPrefab(GameObject prefab)
    {
        var instanceId = prefab.GetInstanceID();
        CacheInterfaces(instanceId,prefab);
    }
    /// <summary>
    /// 获取物体身上所有接口
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="monoBehaviour"></param>
    public void CacheInterfaces(int instanceId, GameObject gameObject)
    {
        if (!allPrefabs.ContainsKey(instanceId))
        {
            allPrefabs[instanceId] = new Dictionary<Type, List<Component>>();
        }

        var components = gameObject.GetComponents<Component>();
        foreach(var component in components)
        {
            var interfaces = component.GetType().GetInterfaces();
            foreach(var intf in interfaces)
            {
                if (!allPrefabs[instanceId].ContainsKey(intf))
                {
                    allPrefabs[instanceId][intf] = new List<Component>();
                }
                Debug.Log(intf);
                allPrefabs[instanceId][intf].Add(component);
            }

            var typename = component.GetType();
            if (!allPrefabs[instanceId].ContainsKey(typename))
            {
                allPrefabs[instanceId][typename] = new List<Component>();
            }
            Debug.Log(typename);
            allPrefabs[instanceId][component.GetType()].Add(component);

        }
    }

    
    public T[] GetAllComponentsByInterface<T>()
    {
        var interfaceType = typeof(T);
        List<T> getallinterface = new List<T>();

        foreach (var prefab in allPrefabs.Values)
        {
            foreach (var kvp in prefab)
            {
                if (kvp.Key.GetInterfaces().Contains(interfaceType))
                {
                    getallinterface.Concat(kvp.Value.Cast<T>());
                }
            }
        }

        return getallinterface.ToArray();
    }
}
