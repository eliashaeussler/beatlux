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
		Settings.ActivePlaylist = Settings.OpenedPlaylist;
		Settings.ActiveVisualization = Settings.OpenedVisualization;
		Settings.ActiveColorScheme = Settings.OpenedColorScheme;

		// Reset opened elements
		Settings.OpenedPlaylist = null;
		Settings.OpenedVisualization = null;
		Settings.OpenedColorScheme = null;

		// Start visualization level
		if (Settings.ActiveVisualization != null && Application.CanStreamedLevelBeLoaded (Settings.ActiveVisualization.BuildNumber)) {
			StartLevel (Settings.ActiveVisualization.BuildNumber);
		} else {
			StartLevel (Settings.Visualizations.First ().BuildNumber);
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
		string path = Settings.CurrentPath;
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