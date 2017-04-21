using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class MenuFunctions : MonoBehaviour {
    
	public static List<String> searchDirs;
	public static List<String> searchFiles;
	public static bool Searching;

	private BackgroundThread thread;



	public void StartLevel (int level) {
        SceneManager.LoadScene (level);
    }
		
    public void Quit ()
    {
		Application.Quit ();
    }

	public void GetInput (string s)
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
			searchDirs = new List<String> ();
			searchFiles = new List<String> ();

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
					SourceFolder.Display (searchDirs, searchFiles, true);
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
		Search (path, pattern);
	}

	private void Search (string folder, string pattern)
	{
		if (Path.GetFileName (folder).IndexOf (pattern, StringComparison.OrdinalIgnoreCase) >= 0) {
			searchDirs.Add (folder);
		}

		// Get files
		string[] files = Directory.GetFiles (folder).Where (x =>
			(new FileInfo (x).Attributes & FileAttributes.Hidden) == 0
			&& Path.GetFileName (x).IndexOf (pattern, StringComparison.OrdinalIgnoreCase) >= 0
			&& SourceFolder.IsSupportedFile (x)
		).ToArray ();

		// Add file if file name contains pattern
		foreach (string file in files) {
			searchFiles.Add (file);
		}

		// Get directories
		string[] dirs = Directory.GetDirectories (folder).Where (x =>
			(new DirectoryInfo (x).Attributes & FileAttributes.Hidden) == 0
		).ToArray ();

		// Jump into sub directory
		foreach (string dir in dirs) {
			Search (dir, pattern);
		}
	}

	public void HideProgress ()
	{
		GameObject.Find ("FileSearch/Input/Progress").SetActive (false);
	}

    // gets the current selected color from the picker
    public static int num2;
    public static Color color;
    public void GetColor()
    {
        color = HexColorField.ColorGet;
        GameObject obj = GameObject.Find("ColorPicker");
        obj.SetActive(false);

    }

    // gets the current selected lvl
    public void GetLvl()
    {
        if (ColorShow.VizId != 0)
        {
            Application.LoadLevel(ColorShow.VizId);
        }

    }

    public static string stri = "";
    public static int num = 0;

    public void GetData(string s)
    {
        stri = s;
        GameObject ob = GameObject.Find("BackgroundImg");
        ob.SetActive(false);
    }

    public static string viz = "";
    public static List<DataObj> cloneList;

    public void GetViz(string o)
    {

        if (o != "")
        {
            viz = o;
        }
        else
        {
            viz = null;
        }

    }
}