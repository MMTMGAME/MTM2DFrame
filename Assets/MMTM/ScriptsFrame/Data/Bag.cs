using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// 数据槽,为玩家储存数据
/// </summary>
public interface IBagData
{
    /// <summary>
    /// 获取数据接口
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetData<T>(string name);
}

public class Bag : MonoBehaviour,IBagData
{
    [Serializable]
    public struct Data
    {
        public Data(string dataName, ScriptableObject data)
        {
            this.dataName = dataName;
            this.data = data;
        }
        public string dataName;
        public ScriptableObject data;
    }

    [SerializeField]
    private List<Data> allData;
    private Dictionary<string,Data> _alldataDiction;

    private void Start()
    {
        _alldataDiction = allData.ToDictionary(p => p.dataName, p=>p);
    }

    public T GetData<T>(string dataName)
    {
        if (_alldataDiction.TryGetValue(dataName,out var value))
        {
            if (value.data is T get)
            {
                return get;
            }
        }
        return default;
    }
}
