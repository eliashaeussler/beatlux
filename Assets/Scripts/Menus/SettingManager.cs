/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Dropdown AntialiasingDropdown;
    public Button ApplyButton;

    //Referencing all the options from the menu
    public Toggle FullscreenToggle;

    public GameSettings GameSettings;
    public GameObject LangManager;
    public Dropdown LanguageDropdown;
    public Dropdown MirrorDropdown;
    public GameObject Mirrors;
    public Dropdown ResolutionDropdown;

    //Storage for all available resolutions
    public Resolution[] Resolutions;
    public Dropdown TextureQualityDropdown;
    public Toggle TutorialToggle;


    private void Start()
    {
        Resolutions = Screen.resolutions; //Saving all resolutions that the current monitor is able to display

        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") != true) return;

        Screen.fullScreen = GameSettings.Fullscreen;
        Screen.SetResolution(Resolutions[GameSettings.ResolutionIndex].width,
            Resolutions[GameSettings.ResolutionIndex].height, Screen.fullScreen);
        QualitySettings.masterTextureLimit = GameSettings.TextureQuality;
        QualitySettings.antiAliasing = GameSettings.Antialiasing;
        SetMirrors();
    }

    private void OnEnable()
    {
        GameSettings = new GameSettings();

        //Listeners for all options, delegating the appropriate methode on change of value
        FullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        TutorialToggle.onValueChanged.AddListener(delegate { OnTutorialToggle(); });
        ResolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        LanguageDropdown.onValueChanged.AddListener(delegate { OnLanguageChange(); });
        TextureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        AntialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        ApplyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });
        MirrorDropdown.onValueChanged.AddListener(delegate { OnMirrorsChange(); });
        Resolutions = Screen.resolutions;

        foreach (var resolution in Resolutions)
            ResolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));

        LoadSettings();
    }


    /**
     * Methods that execute all the functions of the settings options, as well as writing them into the gameSettings file
     **/
    private void OnFullscreenToggle()
    {
        GameSettings.Fullscreen = Screen.fullScreen = FullscreenToggle.isOn;
    }

    private void OnTutorialToggle()
    {
        GameSettings.Tutorial = TutorialToggle.isOn;
        Settings.Player.TutorialTog = GameSettings.Tutorial;
    }

    private void OnResolutionChange()
    {
        Screen.SetResolution(Resolutions[ResolutionDropdown.value].width, Resolutions[ResolutionDropdown.value].height,
            Screen.fullScreen);
        GameSettings.ResolutionIndex = ResolutionDropdown.value;
    }

    private void OnTextureQualityChange()
    {
        GameSettings.TextureQuality = QualitySettings.masterTextureLimit = TextureQualityDropdown.value;
    }

    private void OnAntialiasingChange()
    {
        GameSettings.Antialiasing = QualitySettings.antiAliasing = (int) Mathf.Pow(2f, AntialiasingDropdown.value);
    }

    private void OnLanguageChange()
    {
        GameSettings.Language = LanguageDropdown.value;


        var currentLang = "English";

        switch (LanguageDropdown.value)
        {
            case 0:
                currentLang = "English";
                break;

            case 1:
                currentLang = "German";
                break;
        }

        LangManager.GetComponent<LangMainMenu>().SetTexts(currentLang);
    }

    private void OnMirrorsChange()
    {
        GameSettings.Mirrors = MirrorDropdown.value;

        SetMirrors();
    }

    private void OnApplyButtonClick()
    {
        SaveSettings();
    }

    private void SetMirrors()
    {
        switch (GameSettings.Mirrors)
        {
            case 0:
                Mirrors.GetComponent<MirrorReflection>().enabled = false;
                break;

            case 1:
                Mirrors.GetComponent<MirrorReflection>().enabled = true;
                Mirrors.GetComponent<MirrorReflection>().TextureSize = 256;
                break;

            case 2:
                Mirrors.GetComponent<MirrorReflection>().enabled = true;
                Mirrors.GetComponent<MirrorReflection>().TextureSize = 512;
                break;
        }
    }

    /**
     * Save all settings into a json-file
     **/
    private void SaveSettings()
    {
        var jsonData = JsonUtility.ToJson(GameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json",
            jsonData); //In Windows, saving to "Appdata/LocalLow/HS_Harz_Musikvisualisierung" (foldername => company name in player settings)
    }

    private void LoadSettings()
    {
        ResolutionDropdown.RefreshShownValue();
        //Check if there is a config file
        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") != true) return;

        GameSettings =
            JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

        LanguageDropdown.value = GameSettings.Language;
        AntialiasingDropdown.value = GameSettings.Antialiasing;
        TextureQualityDropdown.value = GameSettings.TextureQuality;
        ResolutionDropdown.value = GameSettings.ResolutionIndex;
        FullscreenToggle.isOn = GameSettings.Fullscreen;
        TutorialToggle.isOn = GameSettings.Tutorial;
        MirrorDropdown.value = GameSettings.Mirrors;
    }
}