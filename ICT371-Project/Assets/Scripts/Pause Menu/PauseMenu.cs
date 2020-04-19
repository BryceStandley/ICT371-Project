using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject gameplayUI;
    public GameObject buttons;
    public GameObject backButton;
    public GameObject optionButtons;
    public GameObject slider;
    public GameObject credits;
    public GameObject creditButton;
    public GameObject graphicsDropdown;
    public GameObject fullScreenToggle;

    public AudioMixer audioMixer;
    
    public TMP_Dropdown resolutionDropDown;
    
    public PlayerInputController playerInputController;

    public AudioSource audioSource;

    Resolution[] resolutions;

    private void Start()
    {
        GetResolutions(); //gets all the resolutions available to the player based on their display
        SetFullScreen(true); //starts the game off in fullscreen
        InitialiseMenu(); //sets up the menu for use
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused) //if game isn't paused
            {
                Resume();
            }
            else //if game is paused
            {
                Pause();
            }
        }
    }

    public void Resume() //represents changes in the game that occur when the game is resumed
    {
        isGamePaused = false;
        
        gameplayUI.SetActive(true); //returns gameplay UI when game is resumed
        buttons.SetActive(true); //ensures buttons are placed back on menu

        //everything flagged false is hidden when game is resumed
        pauseMenuUI.SetActive(false);
        optionButtons.SetActive(false);
        backButton.SetActive(false);
        slider.SetActive(false);
        graphicsDropdown.SetActive(false);
        resolutionDropDown.gameObject.SetActive(false);
        fullScreenToggle.SetActive(false);
        credits.SetActive(false);
        creditButton.SetActive(false);

        audioSource.Play(); //Changed to have direct reference to audio source to stop delay

        playerInputController.EnablePlayerControls(); //allows for player to move around in-game

    }

    void Pause() //represents changes in the game that occur when the game is paused
    {
        isGamePaused = true;

        pauseMenuUI.SetActive(true); //shows pauseMenuUI when game is paused
       
        gameplayUI.SetActive(false); //hides gameplayUI when game is paused
        
        audioSource.Pause(); //Changed to have direct reference to audio source to stop delay

        playerInputController.DisablePlayerControls(); //ceases player ability to move around in-game
    }

    public void Options() //displays option menu
    {
        buttons.SetActive(false);
        backButton.SetActive(true);
        optionButtons.SetActive(true);
    }

    public void Audio() //displays audio menu
    {
        optionButtons.SetActive(false);
        slider.SetActive(true);
    }

    public void Display() //displays display menu
    {
        optionButtons.SetActive(false);
        graphicsDropdown.SetActive(true);
        resolutionDropDown.gameObject.SetActive(true);
        fullScreenToggle.SetActive(true);
    }

    public void Credits() //displays credit menu
    {
        optionButtons.SetActive(false);
        credits.SetActive(true);
        creditButton.SetActive(true);
        backButton.SetActive(true);
    }

    public void Controls() //displays controls menu
    {
        optionButtons.SetActive(false);
        //content on controls page will be present in final game
    }

    public void Back() //displays previous menu dependant on what menu user was in
    {
        if (buttons.activeSelf == false && optionButtons.activeSelf == true) //deals with transition from options menu back to the main section of the pause menu
        {
            //everything below is hidden
            backButton.SetActive(false);
            optionButtons.SetActive(false);
            buttons.SetActive(true);
        }
        else if (optionButtons.activeSelf == false && buttons.activeSelf == false) //deals with transition from submenus of option menu back to the option menu
        {
            //everything below is hidden
            slider.SetActive(false);
            graphicsDropdown.SetActive(false);
            resolutionDropDown.gameObject.SetActive(false);
            fullScreenToggle.SetActive(false);
            credits.SetActive(false);
            backButton.SetActive(true);
            optionButtons.SetActive(true);
            buttons.SetActive(false);
        }
    }

    public void SaveGame() //Saves Game
    {
        //Feature will be implemented in final game
        Debug.Log("Saving  Game...");
    }

    public void QuitGame() //Quits Game
    {
        Application.Quit();
    }

    public void SetVolume(float volume) //Sets the volume of the in-game audio
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int quality) //Sets the graphical quality
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetFullScreen(bool isFullScreen) //Sets the game window to fullscreen if true and windowed if false
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex) //Sets the resolution of the game to one of the selections from the resolution index
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OpenMusicCredit() //credits the user of the in game music   
    {
        Application.OpenURL("https://pinevoc.bandcamp.com/album/green-ideas"); 
    }

    public void GetResolutions() //gets all resolutions user can set and stores them in a list, while also setting the default resolution to best fit the current screen 
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRate + "Hz"; //represents each resolution by showing their width, height and refresh rate
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) //sets the default resolution used in game to match the current resolution of the screen 
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options); //adds the list options to the drop down
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    public void InitialiseMenu() //hides content within menus that aren't immediately visible
    {
        optionButtons.SetActive(false);
        backButton.SetActive(false);
        creditButton.SetActive(false);
        graphicsDropdown.SetActive(false);
        resolutionDropDown.gameObject.SetActive(false);
        fullScreenToggle.SetActive(false);
        slider.SetActive(false);
    }
}

