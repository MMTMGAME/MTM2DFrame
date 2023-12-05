using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class StreamingAssetsLoader
{
    // 加载StreamingAssets中的任何类型资源
    public static async UniTask<T[]> LoadAssetsFromBundle<T>(string bundlePath) where T : Object
    {
        T[] loadedAssets = null;

        var fullBundlePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundlePath);
        AssetBundle assetBundle = null;

        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(fullBundlePath))
        {
            await uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                loadedAssets = assetBundle.LoadAllAssets<T>();
            }
            else
            {
                Debug.LogError(uwr.error);
            }
        }

        // 释放AssetBundle以节省内存
        if (assetBundle != null)
        {
            assetBundle.Unload(false);
        }

        return loadedAssets;
    }
}
