using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public static bool isGamePaused = false;
    public bool inDialogue = true;
    public GameObject pauseMenu,mainPauseMenuUI, gameplayUI;
    public GameObject[] subMenus;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropDown;
    public Toggle fullScreenToggle;
    public AudioSource audioSource;
    private Resolution[] resolutions;
    private EventSystem es;
    private GameObject origSelectedItem;

    void Awake()
    {
        instance = this;
        es = FindObjectOfType<EventSystem>();

    }

    private void Start()
    {
        GetResolutions(); //gets all the resolutions available to the player based on their display
        ToggleFullscreen(); //starts the game off in fullscreen
        origSelectedItem = es.currentSelectedGameObject;

    }
    public void OnPausePressed(InputAction.CallbackContext callback)
    {
        if(callback.performed)
        {
            if(isGamePaused && !inDialogue)//if game is paused
            {
                Resume();
            }
            else if(!isGamePaused && !inDialogue)//if game isn't paused
            {
                Pause();
            }
        }
    }

    public void SetCurrentSelectedItem(GameObject button)//Used when changing menus for controller input
    {
        es.SetSelectedGameObject(button);
    }

    public void Resume() //represents changes in the game that occur when the game is resumed
    {
        isGamePaused = false;
        gameplayUI.SetActive(true); //returns gameplay UI when game is resumed
        foreach(GameObject go in subMenus)
        {
            go.SetActive(false);
        }
        mainPauseMenuUI.SetActive(true);
        pauseMenu.SetActive(false);
        audioSource.Play(); //Changed to have direct reference to audio source to stop delay
        PlayerInputController.instance.EnablePlayerControls(); //allows for player to move around in-game
    }

    void Pause() //represents changes in the game that occur when the game is paused
    {
        isGamePaused = true;

        pauseMenu.SetActive(true); //shows pauseMenuUI when game is paused
       
        gameplayUI.SetActive(false); //hides gameplayUI when game is paused
        
        audioSource.Pause(); //Changed to have direct reference to audio source to stop delay

        PlayerInputController.instance.DisablePlayerControls(); //ceases player ability to move around in-game
    }

    public void SaveGame() //Saves Game
    {
        //Feature will be implemented in final game
        Debug.Log("Saving  Game...");
    }

    public void QuitGame() //Quits Game
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.OpenURL("about:blank");
        }
        else
        {
            Application.Quit();
        }
    }

    public void SetVolume(float volume) //Sets the volume of the in-game audio
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int quality) //Sets the graphical quality
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void ToggleFullscreen() //Sets the game window to fullscreen if true and windowed if false
    {
        if(fullScreenToggle.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            //Debug.Log("Is Fullscreen");
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            //Debug.Log("Is Windowed");
        }
    }

    public void SetResolution(int resolutionIndex) //Sets the resolution of the game to one of the selections from the resolution index
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        ObjectPickUp.instance.UpdateScreenSize();
    }

    public void OpenMusicCredit() //credits the user of the in game music   
    {
        Application.OpenURL("https://pinevoc.bandcamp.com/album/green-ideas"); 

    }

    public string[] allowedResolutions;
    public void GetResolutions() //gets all resolutions user can set and stores them in a list, while also setting the default resolution to best fit the current screen 
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRate + "Hz"; //represents each resolution by showing their width, height and refresh rate
            foreach(string st in allowedResolutions)
            {
                if(option.Contains(st))
                {
                    options.Add(option);
                }
            }

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) //sets the default resolution used in game to match the current resolution of the screen 
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options); //adds the list options to the drop down
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }
}

