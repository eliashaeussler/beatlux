/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    private string _currentLang = "English";

    private TextAsset _textAsset;

    public AudioSource Audio;
    public int DefaultStart = 1;

    public GameSettings GameSettings;
    public Lang LangManager;
    public GameObject Player;
    public GameObject PlayerControl;
    public Transform VizContents;

    public bool OpenSettings { get; set; }

    private void Start()
    {
        // Set MenuManager
        Settings.MenuManager = this;

        // Set components
        Audio = GetComponent<AudioSource>();

        // Load main menu
        StartLevel(DefaultStart);
    }

    private void OnEnable()
    {
        if (!File.Exists(Application.persistentDataPath + "/gamesettings.json"))
        {
            GameSettings = new GameSettings
            {
                Fullscreen = true,
                Tutorial = true,
                Language = 0,
                TextureQuality = 0,
                Antialiasing = 0,
                ResolutionIndex = Screen.resolutions.Length - 1,
                Mirrors = 2
            };

            var jsonData = JsonUtility.ToJson(GameSettings, true);
            File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
        }

        GameSettings =
            JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        _textAsset = Resources.Load("XML/lang") as TextAsset;

        if (File.Exists(Application.persistentDataPath + "/gamesettings.json"))
            switch (GameSettings.Language)
            {
                case 0:
                    _currentLang = "English";
                    break;

                case 1:
                    _currentLang = "German";
                    break;
            }

        LangManager = new Lang(_textAsset, _currentLang, false);
    }


    public void StartLevel(int level)
    {
        if (!Application.CanStreamedLevelBeLoaded(level)) return;

        // Destroy unused GameObjects
        DestroyOld();

        // Start level
        StartCoroutine(StartLevelAsync(level));
    }

    private IEnumerator StartLevelAsync(int level)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);

        while (!asyncLoad.isDone) yield return null;

        // Update skybox and unload last scene
        SetLevel(level);
    }

    private static VisualizationObj NextVisualization()
    {
        // Set default visualization
        if (Settings.Selected.Visualization == null) Settings.Selected.Visualization = Settings.Visualizations.First();

        // Set default color scheme
        if (Settings.Selected.ColorScheme == null
            && !Settings.Selected.Visualization.Equals(Settings.Defaults.Visualization))
            Settings.Selected.ColorScheme = ColorScheme.GetDefault(Settings.Selected.Visualization);

        // Set color scheme
        Settings.Active.ColorScheme = Settings.Selected.ColorScheme;

        // Select first file
        if (Settings.Selected.Playlist != null && Settings.Selected.Playlist.Files.Count > 0
                                               && Settings.Selected.File == null)
            Settings.Selected.File = Settings.Selected.Playlist.Files.First();

        // Start visualization level
        if (Settings.Selected.Visualization != null &&
            Application.CanStreamedLevelBeLoaded(Settings.Selected.Visualization.BuildNumber))
        {
            // Do nothing.
        }
        else if (Settings.Defaults.Visualization != null &&
                 Application.CanStreamedLevelBeLoaded(Settings.Defaults.Visualization.BuildNumber))
        {
            Settings.Selected.Visualization = Settings.Defaults.Visualization;
            Settings.Selected.ColorScheme = null;
        }
        else
        {
            return null;
        }

        return Settings.Selected.Visualization;
    }

    public void StartVisualization()
    {
        var viz = NextVisualization();

        if (viz != null)
            StartLevel(viz.BuildNumber);
        else
            GameObject.Find("MenuManager").GetComponent<Dialog>().ShowDialog(
                "Keine Visualisierung",
                "Bitte wählen Sie eine gültige Visualisierung aus, um zu starten."
            );
    }

    private void SetLevel(int level)
    {
        // Show or hide player skin
        Player.SetActive(IsVisualization(level));
        PlayerControl.SetActive(IsVisualization(level));

        // Set skybox
        if (!IsVisualization(level) || level == Settings.Defaults.Visualization.BuildNumber)
        {
            RenderSettings.skybox = Resources.Load<Material>("Skyboxes/Nebula");
        }
        else
        {
            var viz = Array.Find(Settings.Visualizations, x => x.BuildNumber == level);
            RenderSettings.skybox = viz.Skybox != null ? Resources.Load<Material>("Skyboxes/" + viz.Skybox) : null;
        }

        // Unload last scene
        if (Settings.Active.Scene > 0) SceneManager.UnloadSceneAsync(Settings.Active.Scene);
        Settings.Active.Scene = level;
    }

    private void Dismiss()
    {
        Settings.Selected.Visualization = Settings.Active.Visualization;
        Settings.Selected.ColorScheme = Settings.Active.ColorScheme;
        Settings.Selected.Playlist = Settings.Active.Playlist;
        Settings.Selected.File = Settings.Active.File;

        StartVisualization();
    }

    public void Close()
    {
        if (Settings.Active.Visualization != null)
            Dismiss();
        else
            StartLevel(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private static void DestroyOld()
    {
        for (var i = Settings.MenuManager.VizContents.childCount - 1; i >= 0; i--)
            DestroyImmediate(Settings.MenuManager.VizContents.GetChild(i).gameObject);
    }

    public static void SetSelected()
    {
        // Set selected playlist
        if (Settings.Selected.Playlist == null && Settings.Active.Playlist != null)
            Settings.Selected.Playlist = Settings.Active.Playlist;

        // Set selected file
        if (Settings.Selected.File == null && Settings.Active.File != null &&
            Settings.Selected.Playlist != null && Settings.Selected.Playlist.Files.Contains(Settings.Active.File))
            Settings.Selected.File = Settings.Active.File;

        // Set selected visualization
        if (Settings.Selected.Visualization == null && Settings.Active.Visualization != null)
            Settings.Selected.Visualization = Settings.Active.Visualization;
    }


    //-- HELPER METHODS

    private static bool IsVisualization(int level)
    {
        return level == Settings.Defaults.Visualization.BuildNumber ||
               Settings.Visualizations.Any(viz => viz.BuildNumber == level);
    }

    public static void ToggleStart()
    {
        // Get start button reference
        var start = GameObject.Find("Canvas/Wrapper/Start");

        // Show or hide start button
        start.SetActive(Settings.Selected.Playlist != null || Settings.Selected.Visualization != null);
    }
}