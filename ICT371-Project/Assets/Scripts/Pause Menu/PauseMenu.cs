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
    public GameObject creditButton;
    public AudioMixer audioMixer;
    public GameObject credits;
    //public GameObject graphicsDropdown;

    public PlayerInputController playerInputController;

    public AudioSource audioSource;

    private void Start()
    {
        optionButtons.SetActive(false);
        backButton.SetActive(false);
        slider.SetActive(false);
        credits.SetActive(false);
        //graphicsDropdown.SetActive(false);

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
        //graphicsDropdown.SetActive(false);
        credits.SetActive(false);
        creditButton.SetActive(false);


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
        creditButton.SetActive(false);
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
        //graphicsDropdown.SetActive(true);
        Debug.Log("Displaying Display...");
    }

    public void Credits()
    {
        optionButtons.SetActive(false);
        Debug.Log("Displaying Credits...");
        credits.SetActive(true);
        creditButton.SetActive(true);
        backButton.SetActive(true);
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
            //graphicsDropdown.SetActive(false);
            credits.SetActive(false);
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

    public void SetQuality(int quality) 
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void OpenMusicCredit() 
    {
        Application.OpenURL("https://pinevoc.bandcamp.com/album/green-ideas"); //credits the user of the in game music
    }
}

