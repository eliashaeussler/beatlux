using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class LangMainMenu : MonoBehaviour {
    public GameSettings gameSettings;

    public Text start;
    public Text settings;
    public Text credits;
    public Text exit;

    public Text fullscreen;
    public Text tutorial;
    public Text language;
    public Text resolution;
    public Text texture;
    public Text aa;
    public Text mirrors;
    public Text back;
    public Text save;

    public Dropdown textureQualityDropdown;
    public Dropdown antialiasingDropdown;
    public Dropdown mirrorDropdown;


    private Lang LangManager;
    private string currentLang = "English";
    // Use this for initialization


    void OnEnable () {
        

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
       

        /**
            DROPDOWNS NOT WORKING YET!!!!!!!!!!
        textureQualityDropdown.options.Clear();
        //textureQualityDropdown.options.Add(new Dropdown.OptionsData() { text = LangManager.getString("low") });
        textureQualityDropdown.options.Add(new Dropdown.OptionsData(LangManager.getString("low")));
        **/
    }
	public void setTexts(string _currentLang)
    {
        LangManager.setLanguage(Path.Combine(Application.dataPath, "Resources/XML/lang.xml"), _currentLang);

        start.text = LangManager.getString("start");
        settings.text = LangManager.getString("settings");
        credits.text = LangManager.getString("credits");
        exit.text = LangManager.getString("exit");
        fullscreen.text = LangManager.getString("fullscreen");
        tutorial.text = LangManager.getString("tutorial");
        language.text = LangManager.getString("language");
        resolution.text = LangManager.getString("resolution");
        texture.text = LangManager.getString("texture");
        aa.text = LangManager.getString("aa");
        mirrors.text = LangManager.getString("mirrors");
        back.text = LangManager.getString("back");
        save.text = LangManager.getString("save");
        Debug.Log("Texts set to " + _currentLang);
    }
	// Update is called once per frame
	void Update () {
        

    }
}
