using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class JsonManager
{
    public struct JsonData<T>
    {
        public T datalist;
    }
    public static void WriteJson<T>(T datalist_, string filename)
    {
        var datalist = new JsonData<T>(){datalist = datalist_};
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename + ".json");

        // 检查目录是否存在，如果不存在，则创建
        string directory = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        string json = JsonUtility.ToJson(datalist);

        // 检查文件是否存在，如果不存在，则创建
        if (!System.IO.File.Exists(path))
        {
            System.IO.FileStream fs = System.IO.File.Create(path);
            fs.Close(); // 创建完毕后关闭文件流
        }

        System.IO.File.WriteAllText(path, json);
    }
    public static bool JudgeIsExist(string filename){
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename + ".json");

        // 检查目录是否存在，如果不存在，则创建
        string directory = System.IO.Path.GetDirectoryName(path);
        if (System.IO.Directory.Exists(directory))
        {
            return true;
        }
        return false;
    }

    public static bool WriteJsonList<T>(List<T> datalist, string filename) where T : IEquatable<T>
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename + ".json");
        bool succuss = false;
        // 检查目录是否存在，如果不存在，则创建
        string directory = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        // 如果文件已存在，则读取现有数据
        List<T> existingData = new List<T>();
        if (System.IO.File.Exists(path))
        {
            string existingJson = System.IO.File.ReadAllText(path);
            var existingJsonData = JsonUtility.FromJson<JsonData<List<T>>>(existingJson);
            if (existingJsonData.datalist != null)  // 修正的部分
            {
                existingData = existingJsonData.datalist;
            }
        }
        
        // 检查重复并添加新数据
        foreach (var item in datalist)
        {
            if (!existingData.Contains(item))
            {
                succuss = true;
                existingData.Add(item);
            }
            else
            {
                var index = existingData.IndexOf(item);
                existingData[index] = item;
                succuss = false;
            }
        }
        
        
                    
        // 将更新后的数据写入文件
        var json = JsonUtility.ToJson(new JsonData<List<T>>() { datalist = existingData });
        System.IO.File.WriteAllText(path, json);

        return succuss;
    }


    
    
    public static async UniTaskVoid ReadJsonAsync<T>(string filename, Action<T> resultaction)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename + ".json");

        if (Application.platform == RuntimePlatform.Android)
        {
            using (var www = UnityEngine.Networking.UnityWebRequest.Get(path))
            {
                await www.SendWebRequest();

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || www.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    string json = www.downloadHandler.text;
                    resultaction?.Invoke(JsonUtility.FromJson<T>(json));
                }
            }
        }
        else
        {
            string json = await UniTask.Run(() => System.IO.File.ReadAllText(path));
            resultaction?.Invoke(JsonUtility.FromJson<T>(json));
        }
    }
    
    public static T ReadJson<T>(string filename)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename + ".json");

        if (Application.platform == RuntimePlatform.Android)
        {
            using (var www = UnityEngine.Networking.UnityWebRequest.Get(path))
            {
                www.SendWebRequest();

                while (!www.isDone)
                {
                    // 可以添加一个小的延迟以避免密集的循环，如果适用
                    // System.Threading.Thread.Sleep(1);
                }

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || www.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    string json = www.downloadHandler.text;
                    return JsonUtility.FromJson<T>(json);
                }
            }
        }
        else
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        return default;
    }
    
    public static string[] ReadAllJsonFilesInFolderUnityEditor(string folderPath)
    {
        
        string fullFolderPath = System.IO.Path.Combine(Application.streamingAssetsPath, folderPath).Replace("\\", "/");
        string[] files = System.IO.Directory.GetFiles(fullFolderPath, "*.json");
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = System.IO.Path.GetFileName(files[i]);
            files[i] = fileName;
        }
        return files;
    }

}
