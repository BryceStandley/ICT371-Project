using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu, optionsMenu, audioMenu, displayMenu, creditsMenu, controlsMenu;

    public AudioMixer volumeMixer;
    public Slider volumeSlider;
    public Toggle fullScreenToggle, gamepadControlsToggle, keyboardControlsToggle;
    public Image controlsImage;
    public Sprite gamepadControlsSprite, keyboardControlsSprite;
    public TMP_Dropdown resolutionDropdown, displayDropdown;
    public string[] allowedResolutions;
    public List<Display> displays;
    public List<Resolution> resolutions;

    public GameObject mainMenuFirstButton, optionsDisplayMenuButton;// , displayFirstButton, displayClosedButton, creditsFirstButton, creditsClosedButton, controlsFirstButton, controlsClosedButton;
    public Button optionsAudioButton, optionsCreditButton, optionsControlsButton;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
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
        LoadScreenMode();
        LoadResolution();
    }
    public void OnPausePress(InputAction.CallbackContext callback)
    {
        if(callback.performed)//if pause button is pressed while in menu it will return to the home menu
        {
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
            audioMenu.SetActive(false);
            displayMenu.SetActive(false);
            creditsMenu.SetActive(false);
            controlsMenu.SetActive(false);
        }
    }

    public void NewGame() //starts a new game by loading in the game scene
    {
        //Debug.Log("Loading New Game...");
        SceneManager.LoadScene("MenuToGame");
    }

    public void LoadGame() //loads in a game save
    {
        Debug.Log("Loading Saved Game...");
        //feature will be implemented in final release
    }

    public void QuitGame() //Quits Game
    {
        if(Application.platform != RuntimePlatform.WebGLPlayer)
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
        volumeMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("GameMusicVolume", volume);
    }
    public void LoadVolume()
    {
        float vol = PlayerPrefs.GetFloat("GameMusicVolume", -100);
        if(vol != -100)
        {
            volumeMixer.SetFloat("volume", vol);
            volumeSlider.value = vol;
        }
    }

    public void SetFullScreen() //Sets the game window to fullscreen if true and windowed if false
    {
        if(fullScreenToggle.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerPrefs.SetInt("GameFullscreenMode", 1);
            GetResolutions();
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            PlayerPrefs.SetInt("GameFullscreenMode", -1);
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

    public void OpenMusicCredit()
    {
        Application.OpenURL("https://pinevoc.bandcamp.com/album/green-ideas"); //credits the user of the in game music
    }

    public void ToggleGamepadControlsImage()
    {
        if(gamepadControlsToggle.isOn)
        {
            controlsImage.sprite = gamepadControlsSprite;
        }
        else if(!gamepadControlsToggle.isOn)
        {
            controlsImage.sprite = keyboardControlsSprite;
        }
    }

    public void GetDisplays()
    {
        displayDropdown.ClearOptions();
        List<string> options = new List<string>();
        int index = 0;
        foreach(Display display in Display.displays)
        {
            string option = "Display: " +index;
            displays.Add(display);
            options.Add(option);
            index++;
        }
        displayDropdown.AddOptions(options);
    }

    public void SetDisplay(int index)
    {
        Camera.main.targetDisplay = index;
    }

    public void SetSelectedObject(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(obj);
    }

    public void GetResolutions() //gets all resolutions user can set and stores them in a list, while also setting the default resolution to best fit the current screen 
    {
        Resolution[] tempRes = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < tempRes.Length; i++)
        {
            string option = tempRes[i].width + " x " + tempRes[i].height + " @" + tempRes[i].refreshRate + "Hz"; //represents each resolution by showing their width, height and refresh rate
            foreach(string st in allowedResolutions)
            {
                if(option.Contains(st))
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

        resolutionDropdown.AddOptions(options); //adds the list options to the drop down
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
}
