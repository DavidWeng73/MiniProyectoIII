using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    private CursorLock lockC;
    public static bool isPaused;

    void Start()
    {
        lockC = GetComponent<CursorLock>();
        pauseMenu.SetActive(false);
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
                mainMenu.SetActive(true);
                settingsMenu.SetActive(false);
            }

        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        lockC.UnlockCursor();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        lockC.LockCursor();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
        lockC.LockCursor();
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
        lockC.UnlockCursor();
    }
}
