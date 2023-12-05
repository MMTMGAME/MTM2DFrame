using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using Cysharp.Threading.Tasks;

public enum GameState
{
    Menu,
    Run,
}

public interface IChangeGemeState<GameState>
{
    public void OnChange(GameState nowgamestate);
}

//TODO:控制游戏流程
public class GameManager : MonoBehaviour, IAsyncInit
{
    public static GameManager Instance;
    [Inject] private readonly UpdateEvent _updateEvent;
    public UpdateEvent GameControlUpdateEvent { get; private set; }
    private bool pasue = false;

    private UIManager _uiManager;
    public bool Pause
    {
        get { return pasue; }
        set
        {
            if (value)
            {
                Time.timeScale = 0;
                pasue = true;
            }
            else
            {
                Time.timeScale = 1;
                pasue = false;
            }
        }
    }

    private GameState _gameState;
    public GameState CurrentGameState {
        get
        {
            return _gameState;
        }
        set
        {
            if (value != _gameState)
            {
                _gameState = value;
                var all = PrefabCenter.Instance.GetAllComponentsByInterface<IChangeGemeState<GameState>>();
                foreach (var VARIABLE in all)
                {
                    VARIABLE.OnChange(_gameState);
                }
            }
        }
    }

    public async UniTask AsyncInit()
    {
        Instance = this;
        GameControlUpdateEvent = new UpdateEvent();
        CurrentGameState = GameState.Run;
        Pause = false;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Begin"))
        {
            CurrentGameState = GameState.Menu;
        }
        else
        {
            CurrentGameState = GameState.Run;
        }
    }

    private void Update()
    {
        GameControlUpdateEvent?.TriggerEvent();
    }


    private void OnApplicationQuit()
    {
        isquitgame = true;
    }
    private bool isquitgame = false;
    public bool isGameOver = false;
}