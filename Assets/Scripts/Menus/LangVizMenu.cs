/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;


public class LangVizMenu : MonoBehaviour {

    public Text music;
    public Text viz;
    public Text ok;
    public Text cancel;
    public Text search;
    public Text color;

    public GameSettings gameSettings;
    private Lang LangManager;
    private string currentLang = "English";
    TextAsset textAsset;
    /**
    * Setting up language file, choosing language and setting texts
    * 
    * For more languages, add them in switch-statement
    **/
    void OnEnable()
    {
        textAsset = (TextAsset)Resources.Load("XML/lang");

        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

            switch (gameSettings.language)
            {
                case 0:
                    currentLang = "English";
                    break;

                case 1:
                    currentLang = "German";
                    break;

                default:
                    break;
            }
        }
        LangManager = new Lang(textAsset, currentLang, false);
        setTexts(currentLang);


    }

    /**
     *  Sets all the texts to the specified language when called 
     **/
    public void setTexts(string _currentLang)
    {
        LangManager = new Lang(textAsset, currentLang, false);

        music.text = LangManager.getString("music");
        viz.text = LangManager.getString("viz");
        ok.text = LangManager.getString("ok");
        cancel.text = LangManager.getString("cancel");
        search.text = LangManager.getString("search");
        color.text = LangManager.getString("color");

    }
}
