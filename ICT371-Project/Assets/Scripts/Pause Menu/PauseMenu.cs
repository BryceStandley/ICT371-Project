using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;

    public PlayerInputController playerInputController;

    public AudioSource audioSource;

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

        audioSource.Play();// Changed to have direct reference to audio source to stop delay

        playerInputController.EnablePlayerControls();

    }

    void Pause() 
    {
        pauseMenuUI.SetActive(true);
        //Time.timeScale = 0f; //Disabled, this stops game loops and doesnt show the cursor
        isGamePaused = true;


        //Added to pause player input and show the cursor

        audioSource.Pause(); //Changed to have direct reference to audio source to stop delay

        playerInputController.DisablePlayerControls();
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

