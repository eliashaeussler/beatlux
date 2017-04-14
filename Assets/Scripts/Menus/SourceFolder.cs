using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
//using UnityEditor;
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
		List<String> directories = new List<String> (Directory.GetDirectories(currentPath));
		List<String> files = new List<String> (Directory.GetFiles(currentPath));

		// Show files and folders
		display (directories, files, false);
	}


	// init creates all the objects
	void display (List<String> directories, List<String> files, bool fromSearch)
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
			// creates a gameobject with a recttransform to position it
			folderObject = new GameObject(i++.ToString ());
			folderObject.transform.SetParent(gameObject.transform);
			RectTransform trans = folderObject.AddComponent<RectTransform>();

			// Add Layout Element
			LayoutElement layoutElement = folderObject.AddComponent<LayoutElement> ();
			layoutElement.minHeight = 30;
			layoutElement.preferredHeight = 30;

			// creates and adds an eventtrigger so the text is clickable
			folderObject.AddComponent<EventTrigger> ();
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;

			if (i <= lastDirectory)
			{
				string dir = s;
				entry.callback.AddListener ((eventData) => {
					init (dir);
				});
			}
			else
			{
				// TODO file onclick
			}

			folderObject.GetComponent<EventTrigger> ().triggers.Add (entry);

			// adds a text to the gameobjects which is filled and modified 
			Text text = folderObject.AddComponent<Text> ();
			text.color = Color.white;
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.text = Path.GetFileName (s);
			text.fontSize = 30;
		}

	}

	public void HistoryBack()
	{
		// Clear search input
		GameObject.Find ("FileSearch").transform.Find ("Input").gameObject.GetComponent<InputField> ().text = "";

		// Display file contents
		init (Path.GetFullPath(Path.Combine(@currentPath, @"..")));
	}

}
