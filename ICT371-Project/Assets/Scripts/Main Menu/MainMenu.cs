using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {

    }

    public void LoadGame()
    {

    }

    public void GameInfo()
    {

    }

    public void Options()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadTestLevel() // Needs to be removed for submission
    {
        SceneManager.LoadScene("TestScene");
    }
}
