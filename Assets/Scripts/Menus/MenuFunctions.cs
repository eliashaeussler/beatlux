using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class MenuFunctions : MonoBehaviour {

	public static List<String> sDirs;
	public static List<String> sFiles;
	public static bool Searching;
	private BackgroundThread thread;
	public static List<VisualizationObj> tempViz;



	public void StartLevel (int level)
	{
		if (Application.CanStreamedLevelBeLoaded (level)) {
			SceneManager.LoadScene (level);
		}
    }

	public void StartVisualization ()
	{
		// Set active elements
		if (Settings.Selected.Playlist != null) Settings.Active.Playlist = Settings.Selected.Playlist;
		if (Settings.Selected.File != null) Settings.Active.File = Settings.Selected.File;
		if (Settings.Selected.Visualization != null) Settings.Active.Visualization = Settings.Selected.Visualization;
		if (Settings.Selected.ColorScheme != null) Settings.Active.ColorScheme = Settings.Selected.ColorScheme;

		// Set default color scheme
		if (Settings.Active.ColorScheme == null && Settings.Active.Visualization != null)
		{
			Settings.Selected.Visualization = Settings.Active.Visualization;
			Settings.Active.ColorScheme = ColorScheme.GetDefault ();
		}

		// Start visualization level
		bool started = false;
		if (Settings.Active.Visualization != null && Application.CanStreamedLevelBeLoaded (Settings.Active.Visualization.BuildNumber))
		{
			StartLevel (Settings.Active.Visualization.BuildNumber);
			started = true;
		}
		else if (Settings.Defaults.Visualization != null && Application.CanStreamedLevelBeLoaded (Settings.Defaults.Visualization.BuildNumber))
		{
			Settings.Active.Visualization = Settings.Defaults.Visualization;

			StartLevel (Settings.Defaults.Visualization.BuildNumber);
			started = true;
		}

		if (started)
		{
			// Reset opened elements
			Settings.Selected.Playlist = null;
			Settings.Selected.File = null;
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
		
    public void Quit () {
		Application.Quit ();
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