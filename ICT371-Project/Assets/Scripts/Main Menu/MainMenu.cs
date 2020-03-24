using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        Debug.Log("Loading New Game...");
    }

    public void LoadGame()
    {
        Debug.Log("Loading Saved Game...");
    }

    public void GameInfo()
    {
        Debug.Log("Loading Game Information...");
    }

    public void Options()
    {
        Debug.Log("Loading Options...");
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
