using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class MyTimerAction:IAsyncInit
{
    public static MyTimerAction Instance;
    [SerializeField] float nowtime;
    List<TimeAction> AllRunningTimeAction = new List<TimeAction>();//正在定时运行某个函数和持续运行函数
    private EventTemplate TimeActionRemoveAction;
    
    List<TimeNoScaleAction> AllRunningNoScaleTimeAction = new List<TimeNoScaleAction>();//所有未使用的计时器
    private EventTemplate NoScaleTimeActionRemoveAction;
    
    List<frameAction> FrameTimection = new List<frameAction>();//根据帧来判断的运行的
    private EventTemplate FrameTimectionRemoveAction;

    [Inject]private UpdateEvent _updateEvent;
    public MyTimerAction()
    {
        Instance = this;
        NoScaleTimeActionRemoveAction = new EventTemplate();
        TimeActionRemoveAction = new();
        FrameTimectionRemoveAction = new();
    }
    public async UniTask AsyncInit()
    {
        _updateEvent.AddListener(Update);
        // GameCenter.instance.NoScaleUpdateEvent.AddListener(NoScaleUpdate);
    }
    public void ClearAllTimeAction()
    {
        AllRunningTimeAction.Clear();
    }
    public TimeAction AddTimingAction(float time, System.Action action = null, System.Action Killaction = null)
    {
        var m = new TimeAction(time,Killaction,action);
        AllRunningTimeAction.Add(m);//正在使用数组增加一个
        return m;
    }
    // public void KillTimeAction(TimeAction timeaction)
    // {
    //     timeaction.KillAction?.Invoke();
    //     RemoveTimingAction(timeaction);
    // }
    public void RemoveTimingAction(TimeAction timeaction)
    {
        TimeActionRemoveAction.AddListener(()=>AllRunningTimeAction.Remove(timeaction));
    }
    public void AddTimingAction(TimeAction timeaction) => AllRunningTimeAction.Add(timeaction);

    public void RunTimingAction()
    {//定时函数和持续运行函数
        TimeActionRemoveAction.TriggerEvent();
        TimeActionRemoveAction.ClearListener();
        
        for (int i = 0; i < AllRunningTimeAction.Count; i++)
        {
            if (Time.time < AllRunningTimeAction[i].BeginTime + AllRunningTimeAction[i].DurationTime)
            {
                AllRunningTimeAction[i].Update?.Invoke();
            }
            else
            {
                AllRunningTimeAction[i].KillAction?.Invoke();
                AllRunningTimeAction.RemoveAt(i);
                i--;
            }
        }
    }

    
    void RunNoScaleTimeAction(){
        NoScaleTimeActionRemoveAction.TriggerEvent();//先移除再执行
        NoScaleTimeActionRemoveAction.ClearListener();
        for (var i = 0; i < AllRunningNoScaleTimeAction.Count; i++)
        {
            if(Time.unscaledTime >= AllRunningNoScaleTimeAction[i].DurationTime){
                AllRunningNoScaleTimeAction[i].action?.Invoke();
                AllRunningNoScaleTimeAction.RemoveAt(i);
                i--;
            }
        }
    }
    public TimeNoScaleAction AddNoScaleTimeAction(float nextFrame,System.Action action)
    {

        var timeaction = new TimeNoScaleAction(Time.unscaledTime + nextFrame, action);
        AllRunningNoScaleTimeAction.Add(timeaction);
        return timeaction;
    }


    public void RemoveNoScaleTimeAction(TimeNoScaleAction timeNoScaleAction)
    {
        NoScaleTimeActionRemoveAction.AddListener(() => AllRunningNoScaleTimeAction.Remove(timeNoScaleAction));
    }

    void RunFrameTimeAction(){
        FrameTimectionRemoveAction.TriggerEvent();
        FrameTimectionRemoveAction.ClearListener();
        for (var i = 0; i < FrameTimection.Count; i++)
        {
            if(Time.frameCount >= FrameTimection[i].frame){
                FrameTimection[i].action?.Invoke();
                FrameTimection.RemoveAt(i);
                i--;
            }
        }
    }

    public void RemoveFrameTimeAction(frameAction frameAction)
    {
        FrameTimectionRemoveAction.AddListener(()=>FrameTimection.Remove(frameAction));
    }

    public void AddFrameTimeAction(int nextFrame,System.Action action){
        FrameTimection.Add(new frameAction(Time.frameCount + nextFrame,action));
    }


    private void Update()
    {
        RunTimingAction();
        RunFrameTimeAction();
    }

    void NoScaleUpdate()
    {
        RunNoScaleTimeAction();
    }
}
public struct frameAction{
    public frameAction(int frame_,System.Action action_){
        frame = frame_;
        action = action_;
    }
    public int frame;
    public System.Action action;
}
public struct TimeNoScaleAction{
    public TimeNoScaleAction(float time,System.Action action_){
        DurationTime = time;
        action = action_;
    }
    public float DurationTime;
    public System.Action action;
}
[System.Serializable]
public struct TimeAction
{
    public TimeAction(float DurationTime, System.Action KillAction = null, System.Action action = null)
    {
        this.DurationTime = DurationTime;
        this.Update = action;
        this.KillAction = KillAction;
        BeginTime = Time.time;
    }//构造函数
    public TimeAction Reset(float DurationTime, System.Action KillAction = null, System.Action action = null)
    {
        this.DurationTime = DurationTime;
        this.Update = action;
        this.KillAction = KillAction;
        BeginTime = Time.time;
        return this;
    }
    public float BeginTime;
    public float DurationTime;//持续时间
    public System.Action Update;//行为
    public System.Action KillAction;//结束运行后要运行的函数
}