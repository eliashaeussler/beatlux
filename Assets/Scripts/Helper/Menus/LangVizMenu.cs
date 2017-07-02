using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class LangVizMenu : MonoBehaviour {

    public Text music;
    public Text viz;
    public Text ok;
    public Text cancel;

    public GameSettings gameSettings;
    private Lang LangManager;
    private string currentLang = "English";
    // Use this for initialization
    void OnEnable()
    {


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
        LangManager = new Lang(Path.Combine(Application.dataPath, "Resources/XML/lang.xml"), currentLang, false);
        setTexts(currentLang);


    }
    public void setTexts(string _currentLang)
    {
        LangManager.setLanguage(Path.Combine(Application.dataPath, "Resources/XML/lang.xml"), _currentLang);

        music.text = LangManager.getString("music");
        viz.text = LangManager.getString("viz");
        ok.text = LangManager.getString("ok");
        cancel.text = LangManager.getString("cancel");

    }
}
