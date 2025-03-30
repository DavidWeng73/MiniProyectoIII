using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private TextMeshProUGUI Btn_StartGame;
    private TextMeshProUGUI Btn_Score;
    private TextMeshProUGUI Btn_Settings;
    private TextMeshProUGUI Btn_Exit;

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
