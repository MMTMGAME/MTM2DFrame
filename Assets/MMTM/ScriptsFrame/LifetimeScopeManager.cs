using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

interface IAsyncInit
{
    UniTask AsyncInit();
}
public class LifetimeScopeManager : LifetimeScope//运行顺序 Register -> 带有inject -> RegisterEntryPoint -> 带有inject -> IAsyncInit
{
    public static LifetimeScopeManager Instance;
    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log("MyDebug..");
        builder.Register<MyDebug>(Lifetime.Singleton);
        Debug.Log("PrefabCenter..");
        builder.Register<PrefabCenter>(Lifetime.Singleton);
        Debug.Log("LayerSelector..");
        builder.Register<LayerSelector>(Lifetime.Singleton);

        Debug.Log("UpdateEvent..");
        builder.Register<UpdateEvent>(Lifetime.Singleton);
        Debug.Log("MyTimerAction..");
        builder.Register<MyTimerAction>(Lifetime.Singleton);
        Debug.Log("InputCenter..");
        builder.Register<InputCenter>(Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<GameManager>();
        Debug.Log("InitializationManager..");
        builder.RegisterComponentInHierarchy<DataManager>();
        Debug.Log("DataManager..");
        builder.Register<InitializationManager>(Lifetime.Singleton);
        builder.UseEntryPoints(Lifetime.Singleton, starts =>
        {
            starts.Add<UpdateEvent>();
            starts.Add<InitializationManager>();
        });
        
    }

    // void LoadGameSetting()
    // {
    //     Instance = this;
    //     var debugSetting = GameSetting.GetDebugGameSetting();
    //     var gameSetting = GameSetting.GetGameSetting();
    //     if (debugSetting.Test)
    //     {
    //         _gameSetting = debugSetting;
    //     }
    //     else
    //     {
    //         _gameSetting = gameSetting;
    //     }
    // }
}
