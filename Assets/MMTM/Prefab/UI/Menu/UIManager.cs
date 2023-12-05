using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
     private DefaultInputActions uiInputActions;
     [HorizontalLine(2,EColor.Green)]
     [SerializeField]
     private GameObject GamePassUi;
     [SerializeField]
     private GameObject GameDefeatUi;
     [SerializeField]
     private GameObject PauseUi;


     public void GamePass()
     {
          GamePassUi.SetActive(true);
     }

     private void Start()
     {
          uiInputActions = new DefaultInputActions();
          uiInputActions.Enable();
          uiInputActions.UI.Cancel.performed += Pause;
     }

     public void Pause(InputAction.CallbackContext c)
     {
          GamePause();
     }

     public void GamePause()
     {
          if(GameManager.Instance.isGameOver)return;
          GameManager.Instance.Pause = !GameManager.Instance.Pause;
          if (GameManager.Instance.Pause)
          {
               PauseUi.SetActive(true);
          }
          else
          {
               PauseUi.SetActive(false);
          }
     }

     public void GameDefeat()
     {
          GameDefeatUi.SetActive(true);
     }

     public void QuitGame()
     {
          Application.Quit();
     }

     public void RestartGame()
     {
          SceneManager.LoadScene(SceneManager.GetActiveScene().name);
     }

     public void NextLevel()
     {
          var index = int.Parse(SceneManager.GetActiveScene().name.Split("_")[1]);
          MyDebug.Log($"Level_{index++}");
          SceneManager.LoadScene($"Level_{index++}");
     }
     
     public void GameMenu()
     {
          SceneManager.LoadScene("Begin");
     }
}
