using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject gameplayUI;
    public GameObject buttons;
    public GameObject optionButtons;
    public GameObject backButton;
    public GameObject slider;
    public AudioMixer audioMixer;

    public PlayerInputController playerInputController;

    public AudioSource audioSource;

    private void Start()
    {
        optionButtons.SetActive(false);
        backButton.SetActive(false);
        slider.SetActive(false);

    }

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
        gameplayUI.SetActive(true);
        buttons.SetActive(true); //ensures buttons are placed back on menu
        optionButtons.SetActive(false);
        backButton.SetActive(false);
        slider.SetActive(false);


        //Added, see pause function

        audioSource.Play();// Changed to have direct reference to audio source to stop delay

        playerInputController.EnablePlayerControls();

    }

    void Pause() 
    {
        pauseMenuUI.SetActive(true);
        //Time.timeScale = 0f; //Disabled, this stops game loops and doesnt show the cursor
        isGamePaused = true;
        gameplayUI.SetActive(false);
        optionButtons.SetActive(false);

        //Added to pause player input and show the cursor

        audioSource.Pause(); //Changed to have direct reference to audio source to stop delay

        playerInputController.DisablePlayerControls();
    }

    public void Options()
    {
        Debug.Log("Displaying Canvas...");
        buttons.SetActive(false);
        backButton.SetActive(true);
        optionButtons.SetActive(true);
    }

    public void Audio() 
    {
        optionButtons.SetActive(false);
        Debug.Log("Displaying Audio...");
        slider.SetActive(true);
    }

    public void Display()
    {
        optionButtons.SetActive(false);
        Debug.Log("Displaying Display...");
    }

    public void Credits()
    {
        optionButtons.SetActive(false);
        Debug.Log("Displaying Credits...");
    }

    public void Controls()
    {
        optionButtons.SetActive(false);
        Debug.Log("Displaying Canvas...");
    }

    public void Back() 
    {
        if (buttons.activeSelf == false && optionButtons.activeSelf == true)
        {
            backButton.SetActive(false);
            optionButtons.SetActive(false);
            buttons.SetActive(true);
        }
        else if (optionButtons.activeSelf == false && buttons.activeSelf == false)
        {
            slider.SetActive(false); //affirms that slider is hidden
            backButton.SetActive(true);
            optionButtons.SetActive(true);
            buttons.SetActive(false);

        }
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

    public void SetVolume(float volume) 
    {
        audioMixer.SetFloat("volume", volume);
    }
}

