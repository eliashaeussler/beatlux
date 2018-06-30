/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button SettingsButton;

    public GameObject StartText;

    private void Start()
    {
        // Set start button
        if (Settings.Active.Visualization != null)
            StartText.GetComponent<Text>().text = Settings.MenuManager.LangManager.getString("continue");

        // Set start button click event
        StartText.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (Settings.Active.Visualization != null)
            {
                Settings.Selected.Visualization = Settings.Active.Visualization;
                Settings.MenuManager.StartVisualization();
            }
            else
            {
                Settings.MenuManager.StartLevel(2);
            }
        });

        if (!Settings.MenuManager.OpenSettings) return;

        // Open settings
        SettingsButton.onClick.Invoke();
        Settings.MenuManager.OpenSettings = false;
    }
}