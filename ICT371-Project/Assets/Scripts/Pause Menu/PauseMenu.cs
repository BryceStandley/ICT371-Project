using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;

    public PlayerController playerController;
    public PlayerLook playerLook;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else 
            {
                Pause();
            }
        }
    }

    public void Resume() 
    {
        pauseMenuUI.SetActive(false);
        //Time.timeScale = 1.0f; // Disable,see Pause function
        isGamePaused = false;


        //Added, see pause function
        GetComponent<AudioSource>().Play();
        playerController.enabled = true;
        playerLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Pause() 
    {
        pauseMenuUI.SetActive(true);
        //Time.timeScale = 0f; //Disabled, this stops game loops and doesnt show the cursor
        isGamePaused = true;


        //Added to pause player input and show the cursor
        GetComponent<AudioSource>().Pause();
        playerController.enabled = false;
        playerLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SaveGame() 
    {
        Debug.Log("Saving  Game...");
    }

    public void QuitGame() 
    {
        Debug.Log("Qutting Game...");
        Application.Quit();
    }
}

