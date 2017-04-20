using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class SourceFolder : MonoBehaviour {

	public static string[] SupportedFormats = {
		".mp3",
		".ogg",
		".wav",
		".aif",
		".aiff"
	};


	
	void Start ()
	{
		// Set main path
		Settings.MainPath = @Environment.GetFolderPath (Environment.SpecialFolder.MyMusic);

		// Set current path
		if (Settings.CurrentPath == null) Settings.CurrentPath = Settings.MainPath;

		// Display files and folders for main path
		Initialize (Directory.Exists (Settings.CurrentPath) ? Settings.CurrentPath : Settings.MainPath);
	}

	public static void Initialize ()
	{
		Initialize (Settings.CurrentPath ?? Settings.MainPath);
	}

	public static void Initialize (string Path)
	{
		// path and objects are initialised
		Settings.CurrentPath = Path;

		// Get files and folders
		List<string> directories = GetDirs (Settings.CurrentPath);
		List<string> files = GetFiles (Settings.CurrentPath);

		// Show files and folders
		Display (directories, files, false);
	}


	public static void Display (List<string> Directories, List<string> Files, bool FromSearch)
	{
		// deletes all previous created objects
		DestroyAll ();

		// Scroll to top
		if (!FromSearch) GameObject.Find ("Files").GetComponent<ScrollRect> ().verticalScrollbar.value = 1;

		// Combine directories and folders
		List<string> results = new List<string> (Directories);
		int lastDirectory = results.Count;
		results.AddRange (Files);

		GameObject gameObject = GameObject.Find ("FileContent");
		GameObject folderObject;

		// for each folder and file an object is created
		foreach (string item in results)
		{
			// Test if item is directory
			bool isDir = Directory.Exists (item);

			// creates a gameobject with a recttransform to position it
			folderObject = new GameObject (item);
			folderObject.transform.SetParent (gameObject.transform);

			RectTransform trans = folderObject.AddComponent<RectTransform> ();
			trans.pivot = new Vector2 (0, 0.5f);

			// Add Layout Element
			LayoutElement layoutElement = folderObject.AddComponent<LayoutElement> ();
			layoutElement.minHeight = 30;
			layoutElement.preferredHeight = 30;

			// Add Drag Handler
			if (!isDir) folderObject.AddComponent<DragHandler> ();

			// creates and adds an button so the text is clickable
			Button button = folderObject.AddComponent<Button> ();
			button.transition = Selectable.Transition.Animation;

			string currentItem = item;
			if (isDir)
			{
				button.onClick.AddListener (delegate {
					Initialize (currentItem);
				});
			}
			else
			{
				button.onClick.AddListener (delegate {

					// Get reference to playlist object
					Playlist pl = Camera.main.GetComponent <Playlist> ();

					// Get file object if available
					FileObj file = pl.GetFile (currentItem);

					// TODO insert file into database (if not already exists), then set file as pl.activeFile

				});
			}

			// Add animator
			Animator animator = folderObject.AddComponent<Animator> ();
			animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");

			// adds a text to the gameobjects which is filled and modified 
			Text text = folderObject.AddComponent<Text> ();
			text.color = Color.white;
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.text = Path.GetFileName (item);
			text.fontSize = 30;
		}
	}

	public void HistoryBack ()
	{
		// Get user folder
		string userPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

		if (!Path.Equals (userPath, Settings.CurrentPath))
		{
			// Get new path
			string path = Path.GetFullPath (Path.Combine (Settings.CurrentPath, @".."));

			// Display file contents
			Initialize (path);
		}
	}

	public static void DestroyAll ()
	{
		Transform entries = GameObject.Find ("FileContent").transform;
		for (int i=entries.childCount - 1; i >= 0; i--) {
			GameObject.DestroyImmediate (entries.GetChild (i).gameObject);
		}
	}

	public static  List<string> GetDirs (string Path)
	{
		// Get directories
		string[] dirs = Directory.GetDirectories (Path).Where (x =>
			(new DirectoryInfo (x).Attributes & FileAttributes.Hidden) == 0
		).ToArray ();

		List<string> elements = new List<string> ();
		foreach (string subDir in dirs)
		{
			try {
				// Try to get files inside sub dir
				@Directory.GetFiles (subDir);

				// Add sub dir to list
				elements.Add (subDir);
			} catch {}
		}

		return elements;
	}

	public static List<string> GetFiles (string Path)
	{
		// Get files
		string[] files = Directory.GetFiles (Path).Where (x =>
			(new FileInfo (x).Attributes & FileAttributes.Hidden) == 0
			&& IsSupportedFile (x)
		).ToArray ();

		List<string> elements = new List<string> ();
		foreach (string file in files) {
			elements.Add (file);
		}

		return elements;
	}

	public static bool IsSupportedFile (string file)
	{
		return SupportedFormats.Contains (Path.GetExtension (file).ToLower ());
	}

}
