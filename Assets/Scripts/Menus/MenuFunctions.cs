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
	public Transform vizContents;
	public int defaultStart = 1;

	public static List<String> sDirs;
	public static List<String> sFiles;
	public static bool Searching;
	private BackgroundThread thread;
	public static List<VisualizationObj> tempViz;

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
			SceneManager.sceneLoaded += delegate
			{
				// Show or hide player skin
				if (player != null) player.SetActive (IsVisualization (level));

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
			};
		}
    }

	public VisualizationObj NextVisualization ()
	{
		// Set default visualization
		if (Settings.Selected.Visualization == null) {
			Settings.Selected.Visualization = Settings.Defaults.Visualization;
		}

		// Set default color scheme
		if (Settings.Selected.ColorScheme == null
			&& !Settings.Selected.Visualization.Equals (Settings.Defaults.Visualization)) {

			Settings.Selected.ColorScheme = ColorScheme.GetDefault (Settings.Selected.Visualization);
		}

		// Set visualization and color scheme
		Settings.Active.Visualization = Settings.Selected.Visualization;
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
			Settings.Active.Visualization = Settings.Defaults.Visualization;
			Settings.Active.ColorScheme = null;
		}
		else
		{
			return null;
		}

		return Settings.Active.Visualization;
	}

	public void StartVisualization ()
	{
		VisualizationObj viz = NextVisualization ();

		if (viz != null)
		{
			// Start visualization
			StartLevel (viz.BuildNumber);

			// Reset visualization elements
			Settings.Selected.Visualization = null;
			Settings.Selected.ColorScheme = null;
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

	public void Dismiss ()
	{
		Settings.Selected.Visualization = null;
		Settings.Selected.ColorScheme = null;
		Settings.Selected.Playlist = null;
		Settings.Selected.File = null;

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



	//-- FILE SEARCH

	public void SearchFiles (string s)
    {
		Searching = s.Length > 0;

        if (!Searching)
        {
			// Search done
			Invoke ("HideProgress", 0.01f);
			SourceFolder.Initialize ();
        }
        else
        {
			// Reset results
			sDirs = new List<String> ();
			sFiles = new List<String> ();

			// Dispose current thread
			if (thread != null && thread.IsBusy) {
				thread.Abort ();
				thread.Dispose ();
			}

			// Initalize thread
			thread = new BackgroundThread ();
			thread.WorkerSupportsCancellation = true;
			thread.DoWork += delegate {

				// Destroy elements
				MainThreadDispatcher.Instance ().Enqueue (SourceFolder.DestroyAll);

				// Get search results
				GetResults (s);

				// Display search results
				MainThreadDispatcher.Instance ().Enqueue (delegate {
					SourceFolder.Display (sDirs, sFiles, true);
				});

				// Hide progress
				MainThreadDispatcher.Instance ().Enqueue (HideProgress);

			};

			// Run thread
			thread.RunWorkerAsync ();
        }
    }

	private void GetResults (string pattern)
	{
		// Get results
		string path = Settings.Source.Current;
		FileSearch (path, pattern);
	}

	private void FileSearch (string folder, string pattern)
	{
		if (Path.GetFileName (folder).IndexOf (pattern, StringComparison.OrdinalIgnoreCase) >= 0) {
			sDirs.Add (folder);
		}

		// Get files
		string[] files = Directory.GetFiles (folder).Where (x =>
			(new FileInfo (x).Attributes & FileAttributes.Hidden) == 0
			&& Path.GetFileName (x).IndexOf (pattern, StringComparison.OrdinalIgnoreCase) >= 0
			&& SourceFolder.IsSupportedFile (x)
		).ToArray ();

		// Add file if file name contains pattern
		foreach (string file in files) {
			sFiles.Add (file);
		}

		// Get directories
		string[] dirs = Directory.GetDirectories (folder).Where (x =>
			(new DirectoryInfo (x).Attributes & FileAttributes.Hidden) == 0
		).ToArray ();

		// Jump into sub directory
		foreach (string dir in dirs) {
			FileSearch (dir, pattern);
		}
	}

	public void HideProgress () {
		GameObject.Find ("FileSearch/Input/Progress").SetActive (false);
	}



	//-- VISUALIZATION SEARCH

	public void SearchVisualizations (string s)
	{
		// Get Visualization object
		Visualization viz = GameObject.Find ("VizContent").GetComponent<Visualization> ();

		// Do search or reset
		if (s.Length > 0) {
			viz.Visualizations = tempViz.Where (x => x.Name.IndexOf (s, StringComparison.OrdinalIgnoreCase) >= 0).ToList ();
		} else {
			viz.Visualizations = tempViz;
		}

		// Display visualizations
		viz.Display ();
	}
}