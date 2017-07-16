using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LangMainMenu : MonoBehaviour {
    
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

    public GameSettings gameSettings;
    private Lang LangManager;
    private string currentLang = "English";
    // Use this for initialization

    /**
     * Setting up language file, choosing language and setting texts
     * 
     * For more languages, add them in switch-statement
     **/
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
        
    }

    /**
     *  Sets all the text in the mainMenu to the specified language when called 
     **/
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

        //Add the values to all dropdowns for language support
        textureQualityDropdown.options.Clear();
        textureQualityDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("high")));
        textureQualityDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("medium")));
        textureQualityDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("low")));
        antialiasingDropdown.options.Clear();
        antialiasingDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("off")));
        antialiasingDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("medium")));
        antialiasingDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("high")));
        mirrorDropdown.options.Clear();
        mirrorDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("off")));
        mirrorDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("medium")));
        mirrorDropdown.options.Add(new Dropdown.OptionData(LangManager.getString("high")));
        Debug.Log("Texts set to " + _currentLang);
    }
}
