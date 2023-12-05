using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public interface IActionManager<T> where T:struct,Enum
{
    /// <summary>
    /// 只运行最新进入的行文
    /// </summary>
    /// <param name="name"></param>
    void RunTopAction(T name);

    void RunAction(T name);
    void RunActionString(string name);
    List<Action> GetAction(T actionName);
    void AddAction(T name, Action action);
    void RemoveAction(T name);
    void RemoveAction(T name, Action action);
}
/// <summary>
/// 用来注册一些行为限制，有些时候条件太多了，就必须把条件都抽象出来。没办法
/// </summary>
public interface IActionLimit
{
    public delegate bool JudgeAction();
    
    /// <summary>
    /// 当所有条件中有true的时候返回true,全部为false的时候返回false
    /// </summary>
    /// <returns></returns>
    bool isLimit();
    void RegistJudgeAction(JudgeAction action);
}
/// <summary>
/// 所有action都写在这儿，主要是用string的话，找不到哪儿用了很麻烦
/// </summary>
public enum ActionState
{
    onAttackOver,
    OnPutCatOver,
    SetAttackingFalse,
    SetAttackingTrue,
    attack,
    attackBegin,
    attackCancel,
    attacking,
}

/// <summary>
/// 额外函数容器
/// </summary>
public class ActionManager : MonoBehaviour,IActionManager<ActionState>, IActionLimit
{

    [Serializable]
    public struct AAction
    {
        public ActionState actionname;
        public UnityEvent WillAddAction;
    }
    
    private Dictionary<ActionState,List<Action>> Actions;

    [SerializeField]
    private List<AAction> AppendActions;
    private void Awake()
    {
        Actions = new();
        foreach (var action in AppendActions)
        {
            Actions.TryAdd(action.actionname, new List<Action>()
            {
                () => action.WillAddAction?.Invoke()
            });
        }
    }

    public void RunTopAction(ActionState name)
    {
        var actions = GetAction(name);
        var maxcount = actions.Count - 1;
        if(maxcount>=0) actions[maxcount]?.Invoke();
    }
    public void RunAction(ActionState name)
    {
        var getactions = GetAction(name);
        for (int i = 0; i < getactions.Count; i++)
        {
            getactions[i]?.Invoke();
        }
    }
    public void RunActionString(string name)
    {
        var getactions = GetAction(Enum.Parse<ActionState>(name));
        for (int i = 0; i < getactions.Count; i++)
        {
            getactions[i]?.Invoke();
        }
    }

    public List<Action> GetAction(ActionState actionName)
    {
        if (Actions.TryGetValue(actionName, out List<Action> value))
        {
            return value;
        }
        Actions.Add(actionName,new List<Action>());
        return Actions[actionName];
    }

    public void AddAction(ActionState name_, Action action)
    {
        // 检查字典中是否已经有这个键
        if (!Actions.ContainsKey(name_))
        {
            // 如果没有，创建一个新列表并添加
            Actions[name_] = new List<Action> { action };
        }
        else
        {
            // 如果已经有了，只需在现有列表中添加新的action
            Actions[name_].Add(action);
        }
    }

    public void RemoveAction(ActionState name,Action action)
    {

        if (Actions.TryGetValue(name, out var action1))
        {
            action1.Remove(action);
        }
    }
    
    public void RemoveAction(ActionState name)
    {
        Actions.Remove(name);
    }
    
    private List<IActionLimit.JudgeAction> allJudgeAction;

    public void RegistJudgeAction(IActionLimit.JudgeAction action)
    {
        if (allJudgeAction == null) allJudgeAction = new();
        allJudgeAction.Add(action);
    }
    public bool isLimit()
    {
        if (allJudgeAction == null) return false;
        foreach (var action in allJudgeAction)
        {
            if (action.Invoke()) return true;
        }
        return false;
    }
}

