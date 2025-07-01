using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    private CursorLock lockC;
    public static bool isPaused;
    [SerializeField] private string gameSceneName = "LobbyScene";

    void Start()
    {
        Cursor.visible = true;
        lockC = GetComponent<CursorLock>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();

            }
            else
            {
                PauseGame();
            }

        }
    }

    public void TogglePauseMenu()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        lockC.UnlockCursor();
        settingsMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        lockC.LockCursor();
        settingsMenu.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LobbyLevel");
        lockC.LockCursor();
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
        lockC.UnlockCursor();
    }

    public void BackToLobby()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
