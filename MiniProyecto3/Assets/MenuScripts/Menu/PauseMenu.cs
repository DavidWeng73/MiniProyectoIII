using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    private CursorLock lockC;
    public static bool isPaused;

    void Start()
    {
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
