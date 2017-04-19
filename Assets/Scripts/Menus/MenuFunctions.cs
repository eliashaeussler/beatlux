using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading;

public class MenuFunctions : MonoBehaviour {
    
	public void startLevel (int level)
    {
        SceneManager.LoadScene (level);
    }

    /**
    If started through Unity Editor, end play
        If Started through Build Application, Quit Application
    **/
    public void end()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Quitting Editor");
#else
        Debug.Log("Quitting App");
        Application.Quit();
#endif
    }

	public static List<String> searchDirs;
	public static List<String> searchFiles;
    public static string pathF;

    public void GetInput(string s)
    {
        if (s == "")
        {
			pathF = @SourceFolder.mainPath;
			Invoke ("HideProgress", 0.01f);
        }
        else
        {
			// Reset results
			searchDirs = new List<String> ();
			searchFiles = new List<String> ();

			ThreadStart start = delegate {

				// Get search results
				GetResults (s);

				// Hide progress
				MainThreadDispatcher.Instance ().Enqueue (HideProgress);
			};

			Thread thread = new Thread (start) { IsBackground = true };
			thread.Start ();
        }
    }

	private void GetResults (string pattern)
	{
		// Get results
		string path = SourceFolder.currentPath;
		Search (path, pattern);

		// Remove hidden files and folders
		searchDirs = SourceFolder.RemoveHidden (searchDirs);
		searchFiles = SourceFolder.RemoveHidden (searchFiles);
	}

	private void Search (string folder, string pattern)
	{
		if (Path.GetFileName (folder).IndexOf (pattern, StringComparison.OrdinalIgnoreCase) >= 0) {
			searchDirs.Add (folder);
		}

		try {
			foreach (string item in Directory.GetFileSystemEntries (folder))
			{
				if (Directory.Exists (item)) {

					// Jump into sub directory
					Search (item, pattern);

				} else {

					// Add file if file name contains pattern
					if (Path.GetFileName (item).IndexOf (pattern, StringComparison.OrdinalIgnoreCase) >= 0) {
						searchFiles.Add (item);
					}

				}
			}
		} catch {}
	}

	public void HideProgress ()
	{
		GameObject.Find ("FileSearch/Input/Progress").SetActive (false);
	}
}