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
    public Toggle gamepadControlsToggle;
    public Slider volumeSlider;
    public AudioSource audioSource;
    public Slider cameraSensitivitySlider;
    private List<Resolution> resolutions;

    public GameObject pauseFirstButton, optionsDisplayMenuButton;// , displayFirstButton, displayClosedButton, creditsFirstButton, creditsClosedButton, controlsFirstButton, controlsClosedButton;
    public Button optionsAudioButton, optionsCreditButton, optionsControlsButton;
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            optionsDisplayMenuButton.SetActive(false);

            Navigation audioNav = new Navigation();
            audioNav.selectOnDown = optionsCreditButton;
            optionsAudioButton.navigation = audioNav;

            Navigation creditsNav = new Navigation();
            creditsNav.selectOnDown = optionsControlsButton;
            creditsNav.selectOnUp = optionsAudioButton;
            optionsCreditButton.navigation = creditsNav;

            LoadVolume();
            LoadSensitivity();
        }
        else
        {
            resolutions = new List<Resolution>();
            GetResolutions(); //gets all the resolutions available to the player based on their display
            LoadSettings();
        }
    }

    private void LoadSettings()
    {
        LoadVolume();
        LoadSensitivity();
        LoadScreenMode();
        LoadResolution();

    }

    public void ChangeSelectedItem(GameObject button)
    {
        Debug.Log(button.name);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
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
        //audioSource.Play(); //Changed to have direct reference to audio source to stop delay
        PlayerInputController.instance.EnablePlayerControls(); //allows for player to move around in-game
    }

    void Pause() //represents changes in the game that occur when the game is paused
    {
        isGamePaused = true;

        pauseMenu.SetActive(true); //shows pauseMenuUI when game is paused
       
        gameplayUI.SetActive(false); //hides gameplayUI when game is paused
        
        //audioSource.Pause(); //Changed to have direct reference to audio source to stop delay

        PlayerInputController.instance.DisablePlayerControls(); //ceases player ability to move around in-game
        ChangeSelectedItem(pauseFirstButton);

    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene((int)SceneIndex.MainMenu);
    }

    public void SaveGame() //Saves Game
    {
        //Feature will be implemented in final game
        Debug.Log("Saving  Game...");
    }

    public void QuitGame() //Quits Game
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Application.Quit();
        }
        else
        {
            Application.OpenURL("https://www.murdoch.edu.au/");
        }
    }

    public void SetVolume(float volume) //Sets the volume of the in-game audio
    {
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("GameMusicVolume", volume);
    }

    public void LoadVolume()
    {
        float vol = PlayerPrefs.GetFloat("GameMusicVolume", -100);
        if(vol != -100)
        {
            audioMixer.SetFloat("volume", vol);
            volumeSlider.value = vol;
        }
    }

    public void ToggleFullscreen() //Sets the game window to fullscreen if true and windowed if false
    {
        if(fullScreenToggle.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerPrefs.SetInt("GameFullscreenMode", 1);
            //GetResolutions();
            Debug.Log("Fullscren");
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            PlayerPrefs.SetInt("GameFullscreenMode", -1);
            Debug.Log("NOT Fullscren");
        }
    }

    public Image controlsImage;
    public Sprite pcControlsSprite, gamepadControlsSprite;
    public void ToggleGamepadControlsImage()
    {
        if (gamepadControlsToggle.isOn)
        {
            controlsImage.sprite = gamepadControlsSprite;
        }
        else
        {
            controlsImage.sprite = pcControlsSprite;
        }
    }
    public void LoadScreenMode()
    {
        int mode = PlayerPrefs.GetInt("GameFullscreenMode", 2);
        if(mode != 2)
        {
            if(mode == 1)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                fullScreenToggle.isOn = true;
            }
            else if(mode == -1)
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                fullScreenToggle.isOn = false;
            }
        }
    }

    public void SetResolution(int resolutionIndex) //Sets the resolution of the game to one of the selections from the resolution index
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("GameResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("GameResolutionHeight", resolution.height);
        //ObjectPickUp.instance.UpdateScreenSize();
    }

    private void LoadResolution()
    {
        int w, h;
        w = PlayerPrefs.GetInt("GameResolutionWidth");
        h = PlayerPrefs.GetInt("GameResolutionHeight");
        if(w != 0 || h != 0)
        {
            Screen.SetResolution(w, h, Screen.fullScreenMode);
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreenMode);
        }
    }

    public void OpenMusicCredit() //credits the user of the in game music   
    {
        Application.OpenURL("https://pinevoc.bandcamp.com/album/green-ideas"); 

    }

    public string[] allowedResolutions;
    public Resolution[] tempRes;
    public void GetResolutions() //gets all resolutions user can set and stores them in a list, while also setting the default resolution to best fit the current screen 
    {
        Resolution[] tempRes = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < tempRes.Length; i++)
        {
            string option = tempRes[i].width + " x " + tempRes[i].height + " @" + tempRes[i].refreshRate + "Hz"; //represents each resolution by showing their width, height and refresh rate
            foreach (string st in allowedResolutions)
            {
                if (option.Contains(st))
                {
                    options.Add(option);
                    resolutions.Add(tempRes[i]);
                }
            }

            if (tempRes[i].width == Screen.currentResolution.width && tempRes[i].height == Screen.currentResolution.height) //sets the default resolution used in game to match the current resolution of the screen 
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options); //adds the list options to the drop down
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    public void LoadSensitivity()
    {
        float sen = PlayerPrefs.GetFloat("GameCameraSensitivity", 20);
        cameraSensitivitySlider.value = sen;
        PlayerLook.instance.sensitivity = sen;
    }

}

