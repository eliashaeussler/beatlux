/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LangMusicMenu : MonoBehaviour
{
    private string _currentLang = "English";
    private Lang _langManager;
    private TextAsset _textAsset;
    public Text Cancel;


    public GameSettings GameSettings;

    public Text Music;
    public Text Ok;
    public Text PlaylistName;
    public Text Search;
    public Text Viz;


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
    ///     Sets all the texts to the specified language when called
    /// </summary>
    /// <param name="currentLang"></param>
    private void SetTexts(string currentLang)
    {
        _langManager = new Lang(_textAsset, currentLang, false);

        Music.text = _langManager.getString("music");
        Viz.text = _langManager.getString("viz");
        Ok.text = _langManager.getString("ok");
        Cancel.text = _langManager.getString("cancel");
        Search.text = _langManager.getString("search");
        PlaylistName.text = _langManager.getString("playlist");
    }
}