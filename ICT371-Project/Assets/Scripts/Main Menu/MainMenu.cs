using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public GameObject mainButtons;
    public GameObject backButton;
    public GameObject optionButtons;
    public GameObject slider;
    public GameObject credits;
    public GameObject creditButton;
    public GameObject displayOptions;
    public GameObject graphicsDropdown;
    public GameObject fullScreenToggle;
    public GameObject controlsPage;

    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropDown;

    Resolution[] resolutions;

    private void Start()
    {
        GetResolutions(); //gets all the resolutions available to the player based on their display
        SetFullScreen(true); //starts the game off in fullscreen
        InitialiseMenu(); //sets up the menu for use
    }

    public void Update()
    {

    }

    public void OnPausePress(InputAction.CallbackContext callback)
    {
        if(callback.performed)//if pause button is pressed while in menu it will return to the home menu
        {
            mainButtons.SetActive(true);
            optionButtons.SetActive(false);
            backButton.SetActive(false);
            slider.SetActive(false);
            credits.SetActive(false);
            displayOptions.SetActive(false);
            controlsPage.SetActive(false);
        }
    }

    public void NewGame() //starts a new game by loading in the game scene
    {
        Debug.Log("Loading New Game...");
        SceneManager.LoadScene("MenuToGame");
    }

    public void LoadGame() //loads in a game save
    {
        Debug.Log("Loading Saved Game...");
        //feature will be implemented in final release
    }

    public void Options() //displays option menu
    {
        Debug.Log("Loading Options...");
        mainButtons.SetActive(false);
        backButton.SetActive(true);
        optionButtons.SetActive(true);
    }

    public void Audio() //displays audio menu
    {
        optionButtons.SetActive(false);
        Debug.Log("Displaying Audio...");
        slider.SetActive(true);
    }

    public void Display() //displays display menu
    {
        optionButtons.SetActive(false);
        displayOptions.SetActive(true);
        graphicsDropdown.SetActive(true);
        resolutionDropDown.gameObject.SetActive(true);
        fullScreenToggle.SetActive(true);
    }

    public void Credits() //displays credit menu
    {
        optionButtons.SetActive(false);
        Debug.Log("Displaying Credits...");
        credits.SetActive(true);
        creditButton.SetActive(true);
        backButton.SetActive(true);
    }

    public void Controls() //displays controls menu
    {
        optionButtons.SetActive(false);
        controlsPage.SetActive(true);
        backButton.SetActive(true);
        //content on controls page will be present in final game
    }

    public void Back() //displays previous menu dependant on what menu user was in
    {
        if (mainButtons.activeSelf == false && optionButtons.activeSelf == true)
        {
            //everything below is hidden
            backButton.SetActive(false);
            optionButtons.SetActive(false);
            mainButtons.SetActive(true);
        }
        else if (optionButtons.activeSelf == false && mainButtons.activeSelf == false)
        {
            optionButtons.SetActive(true);
            //everything below is hidden
            slider.SetActive(false); 
            graphicsDropdown.SetActive(false);
            resolutionDropDown.gameObject.SetActive(false);
            fullScreenToggle.SetActive(false);
            credits.SetActive(false);
            backButton.SetActive(true);
            mainButtons.SetActive(false);
            controlsPage.SetActive(false);
        }
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

    public void OpenMusicCredit()
    {
        Application.OpenURL("https://pinevoc.bandcamp.com/album/green-ideas"); //credits the user of the in game music
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
        controlsPage.SetActive(false);
    }
}
