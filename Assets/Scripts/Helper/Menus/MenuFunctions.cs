using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour {

	public AudioSource audio;
	public GameObject player;
	public GameObject playerControl;
	public Transform vizContents;
	public int defaultStart = 1;

	private bool _sett = false;
	public bool openSettings {
		get { return _sett; }
		set { _sett = value; }
	}



	void Start ()
	{
		// Set MenuManager
		Settings.MenuManager = this;

		// Set components
		audio = GetComponent<AudioSource> ();

		// Load main menu
		StartLevel (defaultStart);
	}



	public void StartLevel (int level)
	{
		if (Application.CanStreamedLevelBeLoaded (level))
		{
			// Start level
			SceneManager.LoadScene (level, LoadSceneMode.Additive);

			// Destroy unused GameObjects
			SceneManager.sceneLoaded += delegate {

				DestroyOld ();
			};

			// Update skybox and unload last scene
			SceneManager.sceneLoaded += delegate {

				SetLevel (level);

			};
		}
    }

	public VisualizationObj NextVisualization ()
	{
		// Set default visualization
		if (Settings.Selected.Visualization == null) {
			Settings.Selected.Visualization = Settings.Visualizations.First ();
		}

		// Set default color scheme
		if (Settings.Selected.ColorScheme == null
			&& !Settings.Selected.Visualization.Equals (Settings.Defaults.Visualization)) {

			Settings.Selected.ColorScheme = ColorScheme.GetDefault (Settings.Selected.Visualization);
		}

		// Set color scheme
		Settings.Active.ColorScheme = Settings.Selected.ColorScheme;

		// Select first file
		if (Settings.Selected.Playlist != null && Settings.Selected.Playlist.Files.Count > 0
			&& Settings.Selected.File == null) {

			Settings.Selected.File = Settings.Selected.Playlist.Files.First ();
		}

		// Start visualization level
		if (Settings.Selected.Visualization != null && Application.CanStreamedLevelBeLoaded (Settings.Selected.Visualization.BuildNumber))
		{
			// Do nothing.
		}
		else if (Settings.Defaults.Visualization != null && Application.CanStreamedLevelBeLoaded (Settings.Defaults.Visualization.BuildNumber))
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

	public void StartVisualization ()
	{
		VisualizationObj viz = NextVisualization ();

		if (viz != null)
		{
			// Start visualization
			StartLevel (viz.BuildNumber);
		}
		else
		{
			// Show dialog
			GameObject.Find ("MenuManager").GetComponent<Dialog> ().ShowDialog (
				"Keine Visualisierung",
				"Bitte wählen Sie eine gültige Visualisierung aus, um zu starten."
			);
		}
	}

	private void SetLevel (int level)
	{
		// Show or hide player skin
		player.SetActive (IsVisualization (level));
		playerControl.SetActive (IsVisualization (level));

		// Set skybox
		if (!IsVisualization (level) || level == Settings.Defaults.Visualization.BuildNumber)
		{
			RenderSettings.skybox = Resources.Load<Material> ("Skyboxes/Nebula");
		}
		else
		{
			VisualizationObj viz = Array.Find (Settings.Visualizations, x => x.BuildNumber == level);

			if (viz.Skybox != null) {
				RenderSettings.skybox = Resources.Load<Material> ("Skyboxes/" + viz.Skybox);
			} else {
				RenderSettings.skybox = null;
			}
		}

		// Unload last scene
		SceneManager.UnloadScene (Settings.Active.Scene);
		Settings.Active.Scene = level;
	}

	public void Dismiss ()
	{
		Settings.Selected.Visualization = Settings.Active.Visualization;
		Settings.Selected.ColorScheme = Settings.Active.ColorScheme;
		Settings.Selected.Playlist = Settings.Active.Playlist;
		Settings.Selected.File = Settings.Active.File;

		StartVisualization ();
	}

	public void Close ()
	{
		if (Settings.Active.Visualization != null) {
			Dismiss ();
		} else {
			StartLevel (1);
		}
	}
		
    public void Quit ()
	{
		Application.Quit ();
    }

	private void DestroyOld ()
	{
		for (int i = Settings.MenuManager.vizContents.childCount - 1; i >= 0; i--) {
			GameObject.DestroyImmediate (Settings.MenuManager.vizContents.GetChild (i).gameObject);
		}
	}

	public static void SetSelected ()
	{
		// Set selected playlist
		if (Settings.Selected.Playlist == null && Settings.Active.Playlist != null) {
			Settings.Selected.Playlist = Settings.Active.Playlist;
		}

		// Set selected file
		if (Settings.Selected.File == null && Settings.Active.File != null &&
			Settings.Selected.Playlist != null && Settings.Selected.Playlist.Files.Contains (Settings.Active.File)) {

			Settings.Selected.File = Settings.Active.File;
		}

		// Set selected visualization
		if (Settings.Selected.Visualization == null && Settings.Active.Visualization != null) {
			Settings.Selected.Visualization = Settings.Active.Visualization;
		}
	}



	//-- HELPER METHODS

	public static bool IsVisualization (int level)
	{
		// Is default visualization?
		if (level == Settings.Defaults.Visualization.BuildNumber) return true;

		// Check all visualizations
		foreach (VisualizationObj viz in Settings.Visualizations)
			if (viz.BuildNumber == level) return true;

		return false;
	}

	public static void ToggleStart ()
	{
		// Get start button reference
		GameObject start = GameObject.Find ("Canvas/Wrapper/Start");

		// Show or hide start button
		start.SetActive (Settings.Selected.Playlist != null || Settings.Selected.Visualization != null);
	}
}