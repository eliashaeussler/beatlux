using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class SourceFolder : MonoBehaviour {

	public static string[] SupportedFormats =
	{
		".mp3",
		".ogg",
		".wav",
		".aif",
		".aiff"
	};

	public static List<String> sDirs;
	public static List<String> sFiles;
	public static bool Searching;
	private BackgroundThread thread;

	private Coroutine dblClick;
	private GameObject dblClickGo;
	private float dblClickWait = 0.5f;


	
	void Start ()
	{
		// Set main path
		Settings.Source.Main = @Environment.GetFolderPath (Environment.SpecialFolder.MyMusic);

		// Set current path
		if (Settings.Source.Current == null) Settings.Source.Current = Settings.Source.Main;

		// Display files and folders for main path
		Initialize (Directory.Exists (Settings.Source.Current) ? Settings.Source.Current : Settings.Source.Main);
	}

	public static void Initialize ()
	{
		Initialize (Settings.Source.Current ?? Settings.Source.Main);
	}

	public static void Initialize (string Path)
	{
		// path and objects are initialised
		Settings.Source.Current = Path;

		// Get files and folders
		List<string> directories = GetDirs (Settings.Source.Current);
		List<string> files = GetFiles (Settings.Source.Current);

		// Show files and folders
		Display (directories, files, false);
	}


	public static void Display (List<string> Directories, List<string> Files, bool FromSearch)
	{
		// Delete all previous created GameObjects
		DestroyAll ();

		// Scroll to top
		if (!FromSearch) GameObject.Find ("Files").GetComponent<ScrollRect> ().verticalScrollbar.value = 1;

		// Combine directories and folders
		List<string> results = new List<string> (Directories);
		int lastDirectory = results.Count;
		results.AddRange (Files);

		// Get current GameObject
		GameObject gameObject = GameObject.Find ("FileContent");

		// Create item for ech entry in results
		foreach (string item in results)
		{
			// Test if item is directory
			bool isDir = Directory.Exists (item);

			// Create GameObject
			GameObject obj = new GameObject (item);
			obj.transform.SetParent (gameObject.transform);

			// Add Horizontal Layout Group
			HorizontalLayoutGroup hlg = obj.AddComponent<HorizontalLayoutGroup> ();
			hlg.spacing = 20;
			hlg.childAlignment = TextAnchor.MiddleLeft;
			hlg.childForceExpandWidth = false;
			hlg.childForceExpandHeight = true;

			// Set RectTransform
			RectTransform trans = obj.GetComponent<RectTransform> ();
			trans.localScale = Vector3.one;


			// Create image GameObject
			GameObject goImage = new GameObject ("Image");
			goImage.transform.SetParent (obj.transform);

			// Add text
			TextUnicode textImage = goImage.AddComponent<TextUnicode> ();

			textImage.color = Settings.GetColorFromRgb (180, 180, 180);
			textImage.text = isDir ? IconFont.FOLDER : IconFont.MUSIC;
			textImage.alignment = TextAnchor.MiddleLeft;
			textImage.font = IconFont.font;
			textImage.fontSize = 30;

			// Add RectTransform
			RectTransform imageTrans = goImage.GetComponent<RectTransform> ();
			imageTrans.localScale = Vector3.one;

			// Add Layout Element
			LayoutElement imageLayout = goImage.AddComponent<LayoutElement> ();
			imageLayout.minWidth = 30;


			// Create text GameObject
			GameObject goText = new GameObject ("Text");
			goText.transform.SetParent (obj.transform);

			// Add RectTransform element
			RectTransform textTrans = goText.AddComponent<RectTransform> ();
			textTrans.pivot = new Vector2 (0.5f, 0.5f);
			textTrans.localScale = Vector3.one;

			// Add Layout Element
			LayoutElement layoutElement = goText.AddComponent<LayoutElement> ();
			layoutElement.minHeight = 30;
			layoutElement.preferredHeight = 30;

			// Add Drag Handler
			if (!isDir) goText.AddComponent<DragHandler> ();

			// Add Button
			Button button = goText.AddComponent<Button> ();
			button.transition = Selectable.Transition.Animation;

			Navigation nav = new Navigation ();
			nav.mode = Navigation.Mode.None;
			button.navigation = nav;

			// Add OnClick Handler
			string currentItem = item;
			if (isDir)
			{
				button.onClick.AddListener (delegate {
					Initialize (currentItem);
				});
			}
			else
			{
				string currentFile = item;
				button.onClick.AddListener (delegate {

					// Get reference to playlist object
					Playlist pl = GameObject.Find ("PlaylistContent").GetComponent <Playlist> ();

					// Get file object if available
					FileObj file = pl.GetFile (currentItem);

					// Get source folder object
					SourceFolder sf = GameObject.Find ("FileContent").GetComponent<SourceFolder> ();

					if (sf.DoubleClicked (goText))
					{
						// Get drop area
						GameObject dropObj = GameObject.FindGameObjectWithTag ("PlaylistDrop");

						// Insert file
						DropHandler.InsertFile (currentFile, dropObj, dropObj);
					}

				});
			}

			// Add Animator
			Animator animator = goText.AddComponent<Animator> ();
			animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");

			// Add Text
			Text text = goText.AddComponent<Text> ();

			text.color = Color.white;
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.text = Path.GetFileName (item);
			text.fontSize = 30;
			text.alignment = TextAnchor.MiddleLeft;
		}
	}

	public void HistoryBack ()
	{
//		// Get user folder
//		string userPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
//
//		if (!Path.Equals (userPath, Settings.Source.Current))
//		{
			// Get new path
			string path = Path.GetFullPath (Path.Combine (Settings.Source.Current, @".."));

			// Display file contents
			Initialize (path);
//		}
	}

	public static void DestroyAll ()
	{
		Transform entries = GameObject.Find ("FileContent").transform;
		for (int i=entries.childCount - 1; i >= 0; i--) {
			GameObject.DestroyImmediate (entries.GetChild (i).gameObject);
		}
	}

	public static List<string> GetDirs (string Path)
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



	//-- HELPER METHODS

	public static bool IsSupportedFile (string file)
	{
		return SupportedFormats.Contains (Path.GetExtension (file).ToLower ());
	}

	public bool DoubleClicked (GameObject obj)
	{
		// Stop coroutine
		if (dblClick != null) {
			StopCoroutine (dblClick);
		}

		if (dblClickGo != null && dblClickGo == obj)
		{
			// Second click
			HideDoubleClickImmediate ();
			return true;
		}
		else
		{
			// First click
			StartCoroutine (HideDoubleClick (obj));
			return false;
		}
	}

	private IEnumerator HideDoubleClick (GameObject obj)
	{
		// Set clicked game object
		dblClickGo = obj;
		
		// Wait
		yield return new WaitForSeconds (dblClickWait);

		// Hide double click
		HideDoubleClickImmediate ();
	}

	private void HideDoubleClickImmediate ()
	{
		dblClickGo = null;
	}

}
