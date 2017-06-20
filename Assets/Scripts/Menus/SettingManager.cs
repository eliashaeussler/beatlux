using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class SettingManager : MonoBehaviour {

    //Referencing all the options from the menu
    public Toggle fullscreenToggle;
    public Toggle tutorialToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureQualityDropdown;
    public Dropdown antialiasingDropdown;
    public Button applyButton;
    public Dropdown mirrorDropdown;

    public GameSettings gameSettings;
    public GameObject mirrors;
    //Storage for all available resolutions
    public Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;   //Saving all resolutions that the current monitor is able to display

        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
        {
            Screen.fullScreen = gameSettings.fullscreen;
            Screen.SetResolution(resolutions[gameSettings.resolutionIndex].width, resolutions[gameSettings.resolutionIndex].height, Screen.fullScreen);
            QualitySettings.masterTextureLimit = gameSettings.textureQuality;
            QualitySettings.antiAliasing = gameSettings.antialiasing;
            setMirrors();

        }
    }

    void OnEnable()
    {
        gameSettings = new GameSettings();

        //Listeners for all options, delegating the appropriate methode on change of value
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        tutorialToggle.onValueChanged.AddListener(delegate { OnTutorialToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });
        mirrorDropdown.onValueChanged.AddListener(delegate { OnMirrorsChange(); });
        resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
        LoadSettings();
    }


    public void OnFullscreenToggle()
    {
       gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnTutorialToggle()
    {
        gameSettings.tutorial = tutorialToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
    }

    public void OnTextureQualityChange()
    {
        gameSettings.textureQuality = QualitySettings.masterTextureLimit = textureQualityDropdown.value;
        
    }

    public void OnAntialiasingChange()
    {
        gameSettings.antialiasing = QualitySettings.antiAliasing = (int)Mathf.Pow(2f, antialiasingDropdown.value);
    }

    public void OnMirrorsChange()
    {
        gameSettings.mirrors = mirrorDropdown.value;

        setMirrors();
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData); //In Windows, saving to "Appdata/LocalLow/HS_Harz_Musikvisualisierung" (foldername => company name in player settings)
    }

    public void LoadSettings()
    {
        resolutionDropdown.RefreshShownValue();
        //Check if there is a config file
        if(File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

            antialiasingDropdown.value = gameSettings.antialiasing;
            textureQualityDropdown.value = gameSettings.textureQuality;
            resolutionDropdown.value = gameSettings.resolutionIndex;
            fullscreenToggle.isOn = gameSettings.fullscreen;
            mirrorDropdown.value = gameSettings.mirrors;
        }
    }

    public void setMirrors()
    {
        switch (gameSettings.mirrors)
        {
            case 0:
                mirrors.GetComponent<MirrorReflection>().enabled = false;
                break;
            case 1:
                mirrors.GetComponent<MirrorReflection>().enabled = true;
                mirrors.GetComponent<MirrorReflection>().m_TextureSize = 256;
                break;
            case 2:
                mirrors.GetComponent<MirrorReflection>().enabled = true;
                mirrors.GetComponent<MirrorReflection>().m_TextureSize = 512;
                break;
            default:
                break;
        }
    }

    
}
