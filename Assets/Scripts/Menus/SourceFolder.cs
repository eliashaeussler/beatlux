using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEditor;
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

		// Display files and folders for main path
		init (mainPath);
	}

	// Update is called once per frame
	void Update ()
	{
		// If the search entry is deleted, init() is called and the strings are set to null so update wont be called 
		if (MenuFunctions.pathF != null)
		{
			init (mainPath);
			MenuFunctions.pathF = null;
			MenuFunctions.searchResults = null;
		}

		// If something was searched 
		else if (MenuFunctions.searchResults != null)
		{
			// deletes all previous created objects
			for (int n=0; n <= i; n++) Destroy(GameObject.Find(n.ToString ()));

			// Get search results
			GameObject gameObject = GameObject.Find("FileContent");
			List<String> filePaths = MenuFunctions.searchResults;

			// Display files and folders
			i = 0;
			foreach (string paths in filePaths)
			{
				i++;

				// creates a gameobject with a recttransform to position it
				GameObject fileObject = new GameObject(i.ToString ());
				fileObject.transform.SetParent(gameObject.transform);

				RectTransform trans = fileObject.AddComponent<RectTransform>();

				// adds a text to the gameobjects which is filled and modified 
				Text text = fileObject.AddComponent<Text>();
				text.color = Color.white;
				text.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
				text.fontSize = 30;
			}
		}
	}



	// init creates all the objects
	void init (string pathFolder)
	{
		// deletes all previous created objects
		if (i >= 0) {
			for (int n=0; n <= i; n++) Destroy(GameObject.Find(n.ToString ()));
		}

		// Reset file index
		i = 0;

		// Scroll to top
		GameObject.Find ("Files").GetComponent<ScrollRect> ().verticalScrollbar.value = 1;

		// path and objects are initialised
		currentPath = pathFolder;

		// Get files and folders
		List<String> results = new List<String> ();
		results.AddRange ( Directory.GetDirectories(currentPath) );
		int lastDirectory = results.Count;
		results.AddRange ( Directory.GetFiles(currentPath) );

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
			Text myText = folderObject.AddComponent<Text>();
			myText.color = Color.white;
			myText.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
			myText.text = Path.GetFileName(s);
			myText.fontSize = 30;
		}

	}

	public void HistoryBack()
	{
		// TODO clear input field

		init(Path.GetFullPath(Path.Combine(@currentPath, @"..")));
	}

}
