using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;

public class SourceFolder : MonoBehaviour {
	
	public static string currentPath;
	public static string mainPath;
	int i;


	// Use this for initialization
	void Start ()
	{
		// Set main path
		mainPath = @Environment.GetFolderPath (Environment.SpecialFolder.MyMusic);
		currentPath = mainPath;

		// Display files and folders for main path
		init (mainPath);
	}

	// Update is called once per frame
	void Update ()
	{
		// If the search entry is deleted, init() is called and the strings are set to null so update wont be called 
		if (MenuFunctions.pathF != null)
		{
			init (currentPath);
			MenuFunctions.pathF = null;
			MenuFunctions.searchDirs = null;
		}

		// If something was searched 
		else if (MenuFunctions.searchDirs != null)
		{
			display (MenuFunctions.searchDirs, MenuFunctions.searchFiles, true);
		}
	}


	public void init (string pathFolder)
	{
		// path and objects are initialised
		currentPath = pathFolder;

		// Get files and folders
		List<String> directories = GetDirs (currentPath, true);
		List<String> files = GetFiles (currentPath, true);

		// Show files and folders
		display (directories, files, false);
	}


	// init creates all the objects
	public void display (List<String> directories, List<String> files, bool fromSearch)
	{
		// deletes all previous created objects
		if (i >= 0) {
			Transform entries = GameObject.Find ("FileContent").transform;
			foreach (Transform t in entries.transform) {
				Destroy (t.gameObject);
			}
		}

		// Reset file index
		i = 0;

		// Clear search input
		if (!fromSearch)
			GameObject.Find ("FileSearch").transform.Find ("Input").gameObject.GetComponent<InputField> ().text = "";

		// Scroll to top
		if (!fromSearch)
			GameObject.Find ("Files").GetComponent<ScrollRect> ().verticalScrollbar.value = 1;

		// Combine directories and folders
		List<String> results = new List<String> (directories);
		int lastDirectory = results.Count;
		results.AddRange (files);

		GameObject gameObject = GameObject.Find("FileContent");
		GameObject folderObject;

		// for each folder and file an object is created
		foreach (string s in results)
		{
			i++;

			// creates a gameobject with a recttransform to position it
			folderObject = new GameObject(s);
			folderObject.transform.SetParent(gameObject.transform);

			RectTransform trans = folderObject.AddComponent<RectTransform>();
			trans.pivot = new Vector2 (0, 0.5f);

			// Add Layout Element
			LayoutElement layoutElement = folderObject.AddComponent<LayoutElement> ();
			layoutElement.minHeight = 30;
			layoutElement.preferredHeight = 30;

			// Add Drag Handler
			if (i > lastDirectory) folderObject.AddComponent<DragHandler> ();

			// creates and adds an button so the text is clickable
			Button button = folderObject.AddComponent<Button> ();
			button.transition = Selectable.Transition.Animation;

			string dir = s;
			if (i <= lastDirectory)
			{
				button.onClick.AddListener (delegate {
					init (dir);
				});
			}
			else
			{
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
			text.text = Path.GetFileName (s);
			text.fontSize = 30;
		}

	}

	public void HistoryBack ()
	{
		// Get user folder
		string userPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

		if (!Path.Equals (userPath, currentPath))
		{
			// Clear search input
			GameObject.Find ("FileSearch").transform.Find ("Input").gameObject.GetComponent<InputField> ().text = "";

			// Get new path
			string path = Path.GetFullPath (Path.Combine (@currentPath, @".."));

			// Display file contents
			init (path);
		}
	}

	private List<String> GetDirs (string folder, bool cleaned)
	{
		List<String> elements = new List<String> ();
		foreach (string subDir in Directory.GetDirectories (folder))
		{
			try {
				// Try to get files inside sub dir
				Directory.GetFiles (subDir);

				// Add sub dir to list
				elements.Add (subDir);
			} catch {}
		}

		// Remove hidden folders
		if (cleaned) elements = RemoveHidden (elements);

		return elements;
	}

	private List<String> GetFiles (string folder, bool cleaned)
	{
		List<String> elements = new List<String> ();
		foreach (string file in Directory.GetFiles (folder)) {
			elements.Add (file);
		}

		// Remove hidden files
		if (cleaned) elements = RemoveHidden (elements);

		return elements;
	}

	public static List<String> RemoveHidden (List<String> list)
	{
		for (int i=0; i < list.Count; i++)
		{
			string file = Path.GetFileName (list[i]);

			if (file.StartsWith (".")) {
				list.RemoveAt (i);
				i = -1;
			}
		}

		return list;
	}

}
