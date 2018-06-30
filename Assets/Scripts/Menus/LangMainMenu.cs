/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LangMainMenu : MonoBehaviour
{
    private string _currentLang = "English";
    private Lang _langManager;
    private TextAsset _textAsset;
    public Text Aa;
    public Dropdown AntialiasingDropdown;
    public Text Back;
    public Text Credits;
    public Text Exit;

    public Text Fullscreen;

    public GameSettings GameSettings;
    public Text Language;
    public Dropdown MirrorDropdown;
    public Text Mirrors;
    public Text Resolution;
    public Text Save;
    public Text Settings;

    public Text Start;
    public Text Texture;

    public Dropdown TextureQualityDropdown;
    public Text Tutorial;


    /// <summary>
    ///     Setting up language file, choosing language and setting texts
    /// </summary>
    /// <remarks>
    ///     For more languages, add them in switch-statement
    /// </remarks>
    private void OnEnable()
    {
        _textAsset = Resources.Load("XML/lang") as TextAsset;

        if (File.Exists(Application.persistentDataPath + "/gamesettings.json"))
        {
            GameSettings =
                JsonUtility.FromJson<GameSettings>(
                    File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

            switch (GameSettings.Language)
            {
                case 0:
                    _currentLang = "English";
                    break;

                case 1:
                    _currentLang = "German";
                    break;
            }
        }

        _langManager = new Lang(_textAsset, _currentLang, false);

        SetTexts(_currentLang);
    }

    /// <summary>
    ///     Sets all the texts in the mainMenu to the specified language when called
    /// </summary>
    /// <param name="currentLang"></param>
    public void SetTexts(string currentLang)
    {
        _langManager.setLanguage(_textAsset, currentLang);

        Start.text = _langManager.getString("start");
        Settings.text = _langManager.getString("settings");
        Credits.text = _langManager.getString("credits");
        Exit.text = _langManager.getString("exit");
        Fullscreen.text = _langManager.getString("fullscreen");
        Tutorial.text = _langManager.getString("tutorial");
        Language.text = _langManager.getString("language");
        Resolution.text = _langManager.getString("resolution");
        Texture.text = _langManager.getString("texture");
        Aa.text = _langManager.getString("aa");
        Mirrors.text = _langManager.getString("mirrors");
        Back.text = _langManager.getString("back");
        Save.text = _langManager.getString("save");

        //Add the values to all dropdowns for language support
        TextureQualityDropdown.options.Clear();
        TextureQualityDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("high")));
        TextureQualityDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("medium")));
        TextureQualityDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("low")));
        AntialiasingDropdown.options.Clear();
        AntialiasingDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("off")));
        AntialiasingDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("medium")));
        AntialiasingDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("high")));
        MirrorDropdown.options.Clear();
        MirrorDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("off")));
        MirrorDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("medium")));
        MirrorDropdown.options.Add(new Dropdown.OptionData(_langManager.getString("high")));
    }
}