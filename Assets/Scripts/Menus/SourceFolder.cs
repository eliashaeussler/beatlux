using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class SourceFolder : MonoBehaviour {
	
	public static string CurrentPath;
	public static string MainPath;



	void Start ()
	{
		// Set main path
		MainPath = @Environment.GetFolderPath (Environment.SpecialFolder.MyMusic);
		CurrentPath = MainPath;

		// Display files and folders for main path
		Initialize (MainPath);
	}

	public static void Initialize ()
	{
		Initialize (CurrentPath);
	}

	public static void Initialize (string pathFolder)
	{
		// path and objects are initialised
		CurrentPath = pathFolder;

		// Get files and folders
		List<string> directories = GetDirs (CurrentPath, true);
		List<string> files = GetFiles (CurrentPath, true);

		// Show files and folders
		Display (directories, files, false);
	}


	public static void Display (List<string> directories, List<string> files, bool fromSearch)
	{
		// deletes all previous created objects
		DestroyAll ();

		// Clear search input
		if (!fromSearch)
			GameObject.Find ("FileSearch").transform.Find ("Input").gameObject.GetComponent<InputField> ().text = "";

		// Scroll to top
		if (!fromSearch)
			GameObject.Find ("Files").GetComponent<ScrollRect> ().verticalScrollbar.value = 1;

		// Combine directories and folders
		List<string> results = new List<string> (directories);
		int lastDirectory = results.Count;
		results.AddRange (files);

		GameObject gameObject = GameObject.Find ("FileContent");
		GameObject folderObject;

		// for each folder and file an object is created
		for (int i = 0; i < results.Count; i++) {
			// Current dir
			string dir = results [i];

			// creates a gameobject with a recttransform to position it
			folderObject = new GameObject (dir);
			folderObject.transform.SetParent (gameObject.transform);

			RectTransform trans = folderObject.AddComponent<RectTransform> ();
			trans.pivot = new Vector2 (0, 0.5f);

			// Add Layout Element
			LayoutElement layoutElement = folderObject.AddComponent<LayoutElement> ();
			layoutElement.minHeight = 30;
			layoutElement.preferredHeight = 30;

			// Add Drag Handler
			if (i > lastDirectory)
				folderObject.AddComponent<DragHandler> ();

			// creates and adds an button so the text is clickable
			Button button = folderObject.AddComponent<Button> ();
			button.transition = Selectable.Transition.Animation;

			string currentDir = dir;
			if (i <= lastDirectory) {
				button.onClick.AddListener (delegate {
					Initialize (currentDir);
				});
			} else {
				button.onClick.AddListener (delegate {

					// Get reference to playlist object
					Playlist pl = Camera.main.GetComponent <Playlist> ();

					// Get file object if available
					FileObj file = pl.GetFile (dir);

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
			text.text = Path.GetFileName (dir);
			text.fontSize = 30;
		}
	}

	public void HistoryBack ()
	{
		// Get user folder
		string userPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

		if (!Path.Equals (userPath, CurrentPath))
		{
			// Clear search input
			GameObject.Find ("FileSearch").transform.Find ("Input").gameObject.GetComponent<InputField> ().text = "";

			// Get new path
			string path = Path.GetFullPath (Path.Combine (CurrentPath, @".."));

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

	public static  List<string> GetDirs (string folder, bool cleaned)
	{
		// Get directories
		string[] dirs = Directory.GetDirectories (folder).Where (x =>
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

	public static List<string> GetFiles (string folder, bool cleaned)
	{
		// Get files
		string[] files = Directory.GetFiles (folder).Where (x =>
			(new FileInfo (x).Attributes & FileAttributes.Hidden) == 0
		).ToArray ();

		List<string> elements = new List<string> ();
		foreach (string file in files) {
			elements.Add (file);
		}

		return elements;
	}

}
