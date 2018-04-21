/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

	public GameObject start;
	public Button settings;

	void Start ()
	{
		// Set start button
		if (Settings.Active.Visualization != null) {
			start.GetComponent<Text> ().text = Settings.MenuManager.LangManager.getString ("continue");
		}

		// Set start button click event
		start.GetComponent<Button> ().onClick.AddListener (delegate {

			if (Settings.Active.Visualization != null) {
				Settings.Selected.Visualization = Settings.Active.Visualization;
				Settings.MenuManager.StartVisualization ();
			} else {
				Settings.MenuManager.StartLevel (2);
			}
			
		});

		// Open settings
		if (Settings.MenuManager.openSettings) {
			settings.onClick.Invoke ();
			Settings.MenuManager.openSettings = false;
		}
	}
}
