using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.IO;
using System.Linq;

public class Playlist : MonoBehaviour {

	// Dialog reference
	public Dialog Dialog;

	// Playlists
	public List<PlaylistObj> Playlists;

	// Last created playlist
	public long lastPlaylist;

	// Playlist to toggle
	public PlaylistObj togglePlaylist;

	// File to add to new playlist
	public FileObj fileToAdd;
	public bool showingDialog;




	void Start ()
	{
		// Select playlists from database
		Load ();

		// Set selected elements
		MenuFunctions.SetSelected ();

		// Display playlists
		Display ();

		// Close database connection
		Database.Close ();

		// Show or hide start button
		MenuManager.ToggleStart ();

		// Set start button
		if (Settings.Active.File != null) {
			GameObject.Find ("Start/Button/Text").GetComponent<Text> ().text = "Fortsetzen";
		}
	}

	void Update ()
	{
		// Toggle playlist
		if (togglePlaylist != null)
		{
			ToggleFiles (togglePlaylist, true);
			togglePlaylist = null;
		}

		// Create playlist and add file
		if (fileToAdd != null && lastPlaylist > 0)
		{
			// Get last playlist
			PlaylistObj last = Playlists.Find (x => x.ID == lastPlaylist);

			if (last != null) {
				AddFile (fileToAdd, last);
				togglePlaylist = last;
			}

			// Unset file to add
			fileToAdd = null;
			showingDialog = false;
		}

		if (fileToAdd != null && !showingDialog) 
		{
			showingDialog = true;
			ShowDialog ("PL_ADD");
		}

		// Reset last created playlist
		lastPlaylist = 0;
	}



	public void Display ()
	{
		// Remove all GameObjects
		for (int i=transform.childCount - 1; i >= 0; i--) {
			GameObject.DestroyImmediate (transform.GetChild (i).gameObject);
		}

		foreach (PlaylistObj p in Playlists)
		{
			// Display playlist
			DisplayPlaylist (p);

			// Display files
			foreach (FileObj f in p.Files) {
				DisplayFile (p, f);
			}

			// Hide playlist files
			Transform contents = transform.Find ("#" + p.ID + "/Contents");
			if (contents != null && !p.Equals(Settings.Selected.Playlist))
				contents.gameObject.SetActive (false);
		}
	}

	public void DisplayPlaylist (PlaylistObj playlist)
	{
		// Create GameObject
		GameObject gameObject = DisplayPlaylistOrFile (playlist, null);
		Text text = gameObject.transform.Find ("Main/Text").gameObject.GetComponent<Text> ();

		// Text settings
		text.text = playlist.Name;

		// Set scaling
		gameObject.GetComponent<RectTransform> ().localScale = Vector3.one;
		gameObject.transform.Find ("Main").GetComponent<RectTransform> ().localScale = Vector3.one;

		// Get Event Trigger
		EventTrigger events = gameObject.GetComponent<EventTrigger> ();

		// Add Click Event
		EventTrigger.Entry evtClick = new EventTrigger.Entry ();
		evtClick.eventID = EventTriggerType.PointerClick;
		events.triggers.Add (evtClick);

		evtClick.callback.AddListener ((eventData) => {
			ToggleFiles (playlist);
		});

		// Add Event to Button
		gameObject.transform.Find ("Main/Text").gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			ToggleFiles (playlist);
		});
	}

	public void DisplayFile (PlaylistObj playlist, FileObj file)
	{
		// Create GameObject
		GameObject gameObject = DisplayPlaylistOrFile (playlist, file);
		Text text = gameObject.transform.Find ("Text").gameObject.GetComponent<Text> ();

		// Text settings
		text.text = Path.GetFileName (file.Path);

		// Set scaling
		gameObject.GetComponent<RectTransform> ().localScale = Vector3.one;
		gameObject.transform.parent.GetComponent<RectTransform> ().localScale = Vector3.one;

		// Get Event Trigger
		EventTrigger events = gameObject.GetComponent<EventTrigger> ();

		// Add Click Event
		EventTrigger.Entry evtClick = new EventTrigger.Entry ();
		evtClick.eventID = EventTriggerType.PointerClick;
		events.triggers.Add (evtClick);

		evtClick.callback.AddListener ((eventData) => {
			UpdateSelectedFile (file, true);
		});

		// Add Event to Button
		gameObject.transform.Find ("Text").gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			UpdateSelectedFile (file, true);
		});
	}

	public GameObject DisplayPlaylistOrFile (PlaylistObj playlist, FileObj file)
	{
		if (playlist != null)
		{
			// Navigation for buttons
			Navigation nav = new Navigation ();
			nav.mode = Navigation.Mode.None;

			// Set name
			string name = "#" + playlist.ID + (file != null ? ("." + file.ID) : "");

			// Create main GameObject
			GameObject main = new GameObject ("Main");
			if (file != null) main.name = "Contents";

			// Check if Contents GameObject already exists
			Transform contents = transform.Find ("#" + playlist.ID + "/Contents");
			bool contentsExists = !ReferenceEquals (contents, null);

			if (contentsExists && file != null) {
				DestroyImmediate (main);
				main = contents.gameObject;
			}

			// Set parent of GameObject
			Transform parent = transform;
			if (file != null) parent = main.transform;

			// Create GameOject
			GameObject gameObject = new GameObject (name);
			gameObject.transform.SetParent (parent);

			// Add Vertical Layout Group
			if (!contentsExists || file == null) {
				VerticalLayoutGroup vlg = (file == null ? gameObject : main).AddComponent<VerticalLayoutGroup> ();
				vlg.spacing = 20;
				vlg.childForceExpandWidth = true;
				vlg.childForceExpandHeight = false;
			}



			// Set GameObject for return
			GameObject goReturn = gameObject;

			// Set parent of main GameObject
			parent = gameObject.transform;
			if (file != null) parent = transform.Find ("#" + playlist.ID);
			main.transform.SetParent (parent);


			// Change GameObjects if file is displayed to inherit from different GameObject
			if (file != null) main = gameObject;



			// Add Layout Element to GameObject
			LayoutElement mainLayout = main.AddComponent<LayoutElement> ();
			mainLayout.minHeight = 30;
			mainLayout.preferredHeight = mainLayout.minHeight;

			// Add image to GameObject
			Image mainImg = main.AddComponent<Image> ();
			mainImg.color = Color.clear;

			// Set transformations
			mainImg.rectTransform.pivot = new Vector2 (0, 0.5f);

			// Add Horizontal Layout Group
			HorizontalLayoutGroup mainHlg = main.AddComponent<HorizontalLayoutGroup> ();
			mainHlg.spacing = 10;
			mainHlg.childForceExpandWidth = false;
			mainHlg.childForceExpandHeight = false;
			mainHlg.childAlignment = TextAnchor.MiddleLeft;

			// Set padding right of Horizontal Layout Group
			mainHlg.padding = new RectOffset (0, (file == null ? 65 : 30), 0, 0);

			// Add Drop Handler script
			if (file == null) {
				//gameObject.AddComponent<DropHandler> ();
				gameObject.AddComponent<Image> ().color = Color.clear;
			}


			// Create arrow text GameObject
			GameObject mainArrow = new GameObject ("Arrow");
			mainArrow.transform.SetParent (main.transform);

			// Add text
			TextUnicode mainTextArrow = mainArrow.AddComponent<TextUnicode> ();
			mainTextArrow.color = Color.white;

			if (file == null) {
				mainTextArrow.text = playlist.Equals (Settings.Selected.Playlist)
					? IconFont.DROPDOWN_OPENED
					: IconFont.DROPDOWN_CLOSED;
			}

			// Set text alignment
			mainTextArrow.alignment = TextAnchor.MiddleLeft;

			// Font settings
			mainTextArrow.font = IconFont.font;
			mainTextArrow.fontSize = 20;

			// Add Layout Element
			LayoutElement mainLayoutElementArrow = mainArrow.AddComponent<LayoutElement> ();
			mainLayoutElementArrow.minWidth = 22;


			// Create listening text GameObject
			GameObject mainListening = new GameObject ("Listening");
			if (file != null) mainListening.transform.SetParent (main.transform);

			// Add text
			TextUnicode mainTextListening = mainListening.AddComponent<TextUnicode> ();


			if (playlist.Equals (Settings.Active.Playlist) && (file == null || (file != null && file.Equals (Settings.Active.File))))
			{
				mainTextListening.text = IconFont.LISTENING;
				mainTextListening.fontSize = 30;
				mainTextListening.color = file == null ? Color.white : Settings.GetColor (180, 180, 180);
			}
			else if (file != null && playlist.Equals (Settings.Selected.Playlist) && file.Equals (Settings.Selected.File))
			{
				mainTextListening.text = IconFont.DROPDOWN_CLOSED;
				mainTextListening.fontSize = 20;
				mainTextListening.color = Color.gray;
			}

			// Set text alignment
			mainTextListening.alignment = file == null ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;

			// Font settings
			mainTextListening.font = IconFont.font;

			// Add Layout Element
			LayoutElement mainLayoutElementListening = mainListening.AddComponent<LayoutElement> ();
			mainLayoutElementListening.minWidth = file == null ? 40 : 32;


			// Create text GameObject
			GameObject mainText = new GameObject ("Text");
			mainText.transform.SetParent (main.transform);

			// Add text
			Text text = mainText.AddComponent<Text> ();

			// Set text alignment
			text.alignment = TextAnchor.MiddleLeft;

			// Set text color
			if (file == null) {
				text.color = Color.white;
			} else if (playlist.Equals (Settings.Active.Playlist) && file.Equals (Settings.Active.File)) {
				text.color = Settings.GetColor (180, 180, 180);
			} else {
				text.color = Color.gray;
			}

			// Font settings
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.fontSize = 30;

			// Set transformations
			text.rectTransform.pivot = new Vector2 (0.5f, 0.5f);

			// Add button
			Button buttonText = mainText.AddComponent<Button> ();
			buttonText.transition = Selectable.Transition.Animation;
			buttonText.navigation = nav;

			// Add animator
			Animator animatorText = mainText.AddComponent<Animator> ();
			animatorText.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


			// Add listening element
			if (file == null) mainListening.transform.SetParent (main.transform);


			// Create edit icons GameObject
			GameObject editIcons = new GameObject ("Images");
			editIcons.transform.SetParent (main.transform);

			// Set transformations
			RectTransform editIconsTrans = editIcons.AddComponent<RectTransform> ();
			editIconsTrans.anchoredPosition = Vector2.zero;
			editIconsTrans.anchorMin = new Vector2 (1, 0.5f);
			editIconsTrans.anchorMax = new Vector2 (1, 0.5f);
			editIconsTrans.pivot = new Vector2 (1, 0.5f);

			// Add Layout Element
			LayoutElement editIconslayoutElement = editIcons.AddComponent<LayoutElement> ();
			editIconslayoutElement.ignoreLayout = true;

			// Add Content Size Fitter
			ContentSizeFitter editIconsCsf = editIcons.AddComponent<ContentSizeFitter> ();
			editIconsCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			editIconsCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			// Add Layout Group
			HorizontalLayoutGroup editIconsHlgImg = editIcons.AddComponent<HorizontalLayoutGroup> ();
			editIconsHlgImg.childAlignment = TextAnchor.MiddleRight;
			editIconsHlgImg.spacing = 5;
			editIconsHlgImg.childForceExpandWidth = false;
			editIconsHlgImg.childForceExpandHeight = false;

			// Disable edit icons GameObject
			editIcons.SetActive (false);


			if (file == null)
			{
				// Create edit text GameObject
				GameObject edit = new GameObject ("Edit");
				edit.transform.SetParent (editIcons.transform);

				// Add text
				TextUnicode editText = edit.AddComponent<TextUnicode> ();
				editText.text = IconFont.EDIT;
				editText.color = Color.white;

				// Set text alignment
				editText.alignment = TextAnchor.MiddleRight;

				// Set transformations
				editText.rectTransform.sizeDelta = new Vector2 (20, 30);

				// Font settings
				editText.font = IconFont.font;
				editText.fontSize = 30;

				// Add button
				Button buttonEditEvt = edit.AddComponent<Button> ();
				buttonEditEvt.transition = Selectable.Transition.Animation;
				buttonEditEvt.navigation = nav;

				// Add button onclick event
				buttonEditEvt.onClick.AddListener (delegate {
					ShowDialog ("PL_EDIT", gameObject);
				});

				// Add animator
				Animator animatorEditEvt = edit.AddComponent<Animator> ();
				animatorEditEvt.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");
			}


			// Create delete text GameObject
			GameObject delete = new GameObject ("Delete");
			delete.transform.SetParent (editIcons.transform);

			// Add text
			Text deleteText = delete.AddComponent<Text> ();
			deleteText.text = IconFont.TRASH;
			deleteText.color = Color.white;

			// Set text alignment
			deleteText.alignment = TextAnchor.MiddleRight;

			// Set transformations
			deleteText.rectTransform.sizeDelta = new Vector2 (20, 30);

			// Font settings
			deleteText.font = IconFont.font;
			deleteText.fontSize = 30;

			// Add button
			Button buttonDeleteEvt = delete.AddComponent<Button> ();
			buttonDeleteEvt.transition = Selectable.Transition.Animation;
			buttonDeleteEvt.navigation = nav;

			// Add button onclick event
			buttonDeleteEvt.onClick.AddListener (delegate {
				if (FindFile (gameObject) == null) {
					ShowDialog ("PL_DEL", gameObject);
				} else {
					Delete (gameObject);
				}
			});

			// Add animator
			Animator animatorDeleteEvt = delete.AddComponent<Animator> ();
			animatorDeleteEvt.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


			// Create GameObject Event Triggers
			EventTrigger evtWrapper = gameObject.AddComponent<EventTrigger> ();

			// Add Hover Enter Event
			EventTrigger.Entry evtHover = new EventTrigger.Entry ();
			evtHover.eventID = EventTriggerType.PointerEnter;
			evtWrapper.triggers.Add (evtHover);

			evtHover.callback.AddListener ((eventData) => {
				editIcons.SetActive (true);
			});

			// Add Hover Exit Event
			EventTrigger.Entry evtExit = new EventTrigger.Entry ();
			evtExit.eventID = EventTriggerType.PointerExit;
			evtWrapper.triggers.Add (evtExit);

			evtExit.callback.AddListener ((eventData) => {
				editIcons.SetActive (false);
			});


			return goReturn;
		}

		return null;
	}

	public void ToggleFiles (PlaylistObj playlist)
	{
		ToggleFiles (playlist, false);
	}

	public void ToggleFiles (PlaylistObj playlist, bool forceOpen)
	{
		// Show or hide playlist files
		bool opened = false;
		foreach (PlaylistObj p in Playlists)
		{
			// Get GameObject for current file
			Transform contents = transform.Find ("#" + p.ID + "/Contents");
			GameObject main = contents != null ? contents.gameObject : null;

			// Get arrow
			Text arr = transform.Find ("#" + p.ID + "/Main/Arrow").GetComponent<Text> ();

			// Toggle files for GameObject
			if (main != null)
			{
				if (p == playlist && p.Files.Count > 0)
				{
					main.SetActive (!forceOpen ? !main.activeSelf : true);
					opened = main.activeSelf;
				}
				else
				{
					main.SetActive (false);

					if (arr.text == IconFont.DROPDOWN_OPENED) {
						arr.text = IconFont.DROPDOWN_CLOSED;
					}
				}
			}
			else if (p == playlist)
			{
				opened = true;
			}

			// Change arrows
			if (!forceOpen && p != playlist) {
				arr.text = IconFont.DROPDOWN_CLOSED;
			}
		}

		// Change arrow image
		TextUnicode arrow = transform.Find ("#" + playlist.ID + "/Main/Arrow").GetComponent<TextUnicode> ();
		if (arrow != null) {
			arrow.text = opened ? IconFont.DROPDOWN_OPENED : IconFont.DROPDOWN_CLOSED;
		}

		if (!opened && Settings.Selected.File != null)
		{
			// Remove icon from selected file
			Transform file = transform.Find ("#" + Settings.Selected.Playlist.ID + "/Contents/#" +
				Settings.Selected.Playlist.ID + "." + Settings.Selected.File.ID + "/Listening");
			if (file != null) file.GetComponent<Text> ().text = "";

			// Unset selected file
			Settings.Selected.File = null;
		}

		if ((opened && Settings.Selected.Playlist != playlist) || forceOpen)
		{
			// Set selected playlist
			Settings.Selected.Playlist = playlist;

			// Set selected file
			if (Settings.Selected.Playlist.Files.Count > 0)
			{
				UpdateSelectedFile (Settings.Selected.Playlist.Files.First (),
					Settings.Active.File == null || Settings.Active.Playlist == null
					|| (Settings.Active.File != null && !Settings.Active.Playlist.Equals (Settings.Selected.Playlist)));
			}
			else
			{
				Settings.Selected.File = null;
			}
		}

		// Unset selected playlist
		if (!opened && !forceOpen) Settings.Selected.Playlist = null;

		// Scroll to top if scrollbar is hidden
		ScrollToTop ();

		// Toggle start button
		MenuManager.ToggleStart ();
	}

	public void UpdateSelectedFile (FileObj file, bool updateFile)
	{
		// Update selected file
		if (updateFile) Settings.Selected.File = file;

		// Re-display files and playlists
		Display ();
	}

	public void ScrollToTop ()
	{
		// Force canvas to update elements
		Canvas.ForceUpdateCanvases ();

		// Scroll to top if scrollbar is not visible
		if (transform.GetComponent<RectTransform> ().sizeDelta.y < 0)
			gameObject.transform.parent.GetComponent<ScrollRect> ().verticalScrollbar.value = 1;
	}

	public long NewPlaylist (string name)
	{
		if (name.Length > 0)
		{
			// Create playlist object
			PlaylistObj playlist = new PlaylistObj (name);

			// Create object in database
			long id = Create (playlist);

			// Reload playlists
			if (id > 0)
			{
				Load ();
				Display ();
			}

			return id;
		}

		return (long) Database.Constants.EmptyInputValue;
	}

	public long EditPlaylist (PlaylistObj playlist, string name)
	{
		if (name.Length > 0)
		{
			// Clone playlist
			PlaylistObj pl = (PlaylistObj) playlist.Clone ();
			pl.Name = name;

			// Edit playlist
			long id = Edit (pl);

			if (id == (long) Database.Constants.Successful)
			{
				// Update playlist objects
				if (Settings.Active.Playlist != null && Settings.Active.Playlist.Equals (playlist)) {
					Settings.Active.Playlist.Name = name;
				}
				if (Settings.Selected.Playlist != null && Settings.Selected.Playlist.Equals (playlist)) {
					Settings.Selected.Playlist.Name = name;
				}

				// Reload playlists
				Load ();
				Display ();
			}

			return id;
		}

		return (long) Database.Constants.EmptyInputValue;
	}

	public bool Delete (GameObject gameObject)
	{
		// Get file
		PlaylistObj playlist = FindPlaylist (gameObject);
		FileObj file = FindFile (gameObject);

		bool deleted = playlist != null ? (file != null ? DeleteFile (playlist, file) : DeletePlaylist (playlist)) : false;

		if (deleted)
		{
			// Unset selected file
			if (playlist.Files.Contains (Settings.Selected.File) && (file == null || file.Equals (Settings.Selected.File))) {
				Settings.Selected.File = null;
			}

			// Delete extant files
			FileObj [] files = file == null ? playlist.Files.ToArray () : new FileObj [] { file };
			foreach (FileObj f in files) DeleteExtantFile (f, playlist);

			if (file == null)
			{
				// Remove from list
				Playlists.Remove (playlist);

				// Unset active and opened playlist
				if (playlist.Equals (Settings.Active.Playlist)) Settings.Active.Playlist = null;
				if (playlist.Equals (Settings.Selected.Playlist)) Settings.Selected.Playlist = null;
			}
			else
			{
				// Remove files from playlists
				playlist.Files.Remove (file);
				if (playlist.Equals (Settings.Active.Playlist)) Settings.Active.Playlist.Files.Remove (file);
				if (playlist.Equals (Settings.Selected.Playlist)) Settings.Selected.Playlist.Files.Remove (file);

				// Toggle files
				if (playlist.Files.Count == 0) {
					ToggleFiles (playlist);
				}
			}

			// Display playlists
			Display ();
		}

		return deleted;
	}

	public PlaylistObj FindPlaylist (GameObject gameObject)
	{
		if (gameObject != null)
		{
			// Get playlist id
			string[] name = gameObject.name.Split ('.');
			long playlistID = Int64.Parse (name [0].Split ('#') [1]);

			// Get playlist
			return Playlists.Find (x => x.ID == playlistID);
		}

		return null;
	}

	public FileObj FindFile (GameObject gameObject)
	{
		if (gameObject != null)
		{
			// Get playlist and file id
			string[] name = gameObject.name.Split ('.');
			long fileID = name.Length > 1 ? Int64.Parse (name [1]) : 0;

			// Get playlist
			PlaylistObj playlist = FindPlaylist (gameObject);

			// Get file
			if (playlist != null) {
				return playlist.Files.Find (x => x.ID == fileID);
			}
		}

		return null;
	}

	public void ShowDialog (string type) {
		ShowDialog (type, null);
	}

	public void ShowDialog (string type, GameObject obj)
	{
		if (Dialog != null)
		{
			// Playlist object
			PlaylistObj playlist = FindPlaylist (obj);

			// Button
			Button button = Dialog.ButtonOK;
			Text buttonText = Dialog.GetButtonText ();

			// Remove listener
			button.onClick.RemoveAllListeners ();


			switch (type) {

			// New playlist
			case "PL_ADD":
				
				// UI elements
				Dialog.SetHeading ("Neue Playlist erstellen");
				Dialog.SetInputField ("", "Wie soll die neue Playlist heißen?");

				// Events
				button.onClick.AddListener (delegate {

					// Create playlist
					long result = NewPlaylist (Dialog.GetInputText ().Trim ());

					// Handle database result
					switch (result) {

					// Playlist name already taken
					case (long) Database.Constants.DuplicateFound:

						Dialog.SetInfo ("Eine Playlist mit diesem Namen ist bereits vorhanden.");
						break;

					// Database query failed
					case (long) Database.Constants.QueryFailed:
						
						Dialog.SetInfo ("Die Playlist konnte nicht erstellt werden.");
						break;

					// No user input
					case (long) Database.Constants.EmptyInputValue:

						Dialog.SetInfo ("Bitte geben Sie einen Namen für die Playlist ein.");
						break;

					default:
						
						Dialog.HideDialog ();
						break;

					}

				});

				break;



			// Edit playlist
			case "PL_EDIT":
				
				if (playlist != null)
				{
					// UI elements
					Dialog.SetHeading ("Playlist bearbeiten");
					Dialog.SetInputField (playlist.Name, playlist.Name);

					// Events
					button.onClick.AddListener (delegate {

						// Edit playlist
						long result = EditPlaylist (playlist, Dialog.GetInputText ().Trim ());

						// Handle database result
						switch (result) {

						// Playlist name already taken
						case (long) Database.Constants.DuplicateFound:

							Dialog.SetInfo ("Eine Playlist mit diesem Namen ist bereits vorhanden.");
							break;

						// Database query failed
						case (long) Database.Constants.QueryFailed:

							Dialog.SetInfo ("Die Playlist konnte nicht aktualisiert werden.");
							break;

						// No user input
						case (long) Database.Constants.EmptyInputValue:

							Dialog.SetInfo ("Bitte geben Sie einen Namen für die Playlist ein.");
							break;

						default:

							Dialog.HideDialog ();
							break;

						}
					});
				}
				else
				{
					return;
				}

				break;



			// Delete playlist
			case "PL_DEL":
				
				if (playlist != null)
				{
					// UI elements
					Dialog.SetHeading ("Playlist löschen");
					Dialog.SetText ("Playlist \"" + playlist.Name + "\" endgültig löschen?");

					// Events
					button.onClick.AddListener (delegate {
						
						Delete (obj);

						Load ();
						Display ();

						Dialog.HideDialog ();

					});
				}
				else
				{
					return;
				}

				break;



			default:
				return;

			}

			// Show dialog
			Dialog.dialog.SetActive (true);
		}

		return;
	}



	//-- DATABASE METHODS

	public void Load ()
	{
		if (Database.Connect ())
		{
			// Reset playlists list
			Playlists = new List<PlaylistObj> ();

			// Database command
			SqliteCommand cmd = new SqliteCommand (Database.Connection);

			// Query statement
			string sql = "SELECT id,name,files FROM playlist ORDER BY name ASC";
			cmd.CommandText = sql;

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read sql results
			while (reader.Read ())
			{
				// Create playlist object
				PlaylistObj obj = new PlaylistObj (reader.GetString (1));

				// Set ID
				obj.ID = reader.GetInt64 (0);

				// Get file IDs
				string[] fileIDs = !reader.IsDBNull (2) ? reader.GetString (2).Split (new Char[] { ',', ' ' }) : new string[0];

				// Select files
				List<FileObj> files = new List<FileObj> ();
				foreach (string id in fileIDs)
				{
					FileObj file = GetFile (Int64.Parse (id), false);
					if (file != null && File.Exists (file.Path)) {
						files.Add (file);
					}
				}

				// Sort files
				files.Sort ((lhs, rhs) => Path.GetFileName (lhs.Path).CompareTo (Path.GetFileName (rhs.Path)));

				// Set files
				obj.Files = files;

				// Add contents to playlists array
				Playlists.Add (obj);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();
		}

		// Close database connection
		Database.Close ();
	}

	public long Create (PlaylistObj playlist)
	{
		if (Database.Connect () && playlist != null && playlist.Name.Length > 0)
		{
			// SQL settings
			SqliteCommand cmd = null;
			SqliteDataReader reader = null;
			string sql = "";

			// Check if playlist files are already in database
			foreach (FileObj file in playlist.Files)
			{
				// Query statement
				sql = "SELECT id FROM file WHERE path = @Path";
				cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

				// Get sql results
				reader = cmd.ExecuteReader ();

				// Read and add file IDs
				int count = 0;
				while (reader.Read ()) {
					file.ID = reader.GetInt64 (0);
					count++;
				}

				// Close reader
				reader.Close ();
				cmd.Dispose ();

				// Add file to database if not already exists
				if (count == 0)
				{
					// Query statement
					sql = "INSERT INTO file (path) VALUES(@Path); " +
						"SELECT last_insert_rowid()";
					cmd = new SqliteCommand (sql, Database.Connection);

					// Add Parameters to statement
					cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

					// Execute statement
					file.ID = (long) cmd.ExecuteScalar ();
					cmd.Dispose ();
				}
			}

			// Format file IDs
			string files = FormatFileIDs (playlist.Files);

			// Insert playlist into database
			try
			{
				sql = "INSERT INTO playlist (name,files) VALUES(@Name, @Files); " +
					"SELECT last_insert_rowid()";
				cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Name", playlist.Name));
				cmd.Parameters.Add (new SqliteParameter ("Files", files));

				// Execute insert statement and get ID
				long id = (long) cmd.ExecuteScalar ();

				// Close database connection
				cmd.Dispose ();
				Database.Close ();

				// Set last created playlist
				lastPlaylist = id;

				return id;
			}
			catch (SqliteException)
			{
				// Close database connection
				Database.Close ();

				return (long) Database.Constants.DuplicateFound;
			}
		}

		// Close database connection
		Database.Close ();

		return (long) Database.Constants.QueryFailed;
	}

	public long Edit (PlaylistObj playlist)
	{
		if (Database.Connect () && playlist != null && playlist.Name.Length > 0)
		{
			try
			{
				// Query statement
				string sql = "UPDATE playlist SET name = @Name, files = @Files WHERE id = @ID";
				SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Name", playlist.Name));
				cmd.Parameters.Add (new SqliteParameter ("Files", FormatFileIDs (playlist.Files)));
				cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				Database.Close ();

				return (long) (result > 0 ? Database.Constants.Successful : Database.Constants.QueryFailed);
			}
			catch (SqliteException)
			{
				return (long) Database.Constants.DuplicateFound;
			}
		}

		// Close database connection
		Database.Close ();

		return (long) (playlist.Name.Length > 0 ? Database.Constants.QueryFailed : Database.Constants.EmptyInputValue);
	}

	public long AddFile (FileObj file, PlaylistObj playlist)
	{
		if (Database.Connect () && file != null && playlist != null)
		{
			// Reset file ID
			file.ID = 0;

			// Update file ID: Query statement
			string sql = "SELECT id FROM file WHERE path = @Path";
			SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read id
			while (reader.Read ()) {
				file.ID = reader.GetInt64 (0);
			}

			// Close reader
			reader.Close();
			cmd.Dispose ();

			// Add file if ID is still not valid
			if (!(file.ID > 0))
			{
				// Query statement
				sql = "INSERT INTO file (path) VALUES (@Path); " +
					"SELECT last_insert_rowid()";
				cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Path", file.Path));

				// Send query
				file.ID = (long) cmd.ExecuteScalar ();
				cmd.Dispose ();
			}

			if (!playlist.Files.Contains (file))
			{
				// Add file to playlist
				playlist.Files.Add (file);

				// Sort files
				playlist.Files.Sort ((lhs, rhs) => Path.GetFileName (lhs.Path).CompareTo (Path.GetFileName (rhs.Path)));

				// Set file IDs
				string files = FormatFileIDs (playlist.Files);

				// Query statement
				sql = "UPDATE playlist SET files = @Files WHERE id = @ID";
				cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Files", files));
				cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				Database.Close ();

				return (long) (result > 0 ? Database.Constants.Successful : Database.Constants.QueryFailed);
			}
			else
			{
				return (long) Database.Constants.DuplicateFound;
			}
		}

		// Close database connection
		Database.Close ();

		return (long) Database.Constants.QueryFailed;
	}

	public FileObj GetFile (long id, bool closeConnection)
	{
		if (Database.Connect ())
		{
			// Send database query
			SqliteCommand cmd = new SqliteCommand (Database.Connection);
			cmd.CommandText = "SELECT id,path FROM file WHERE id = @ID";

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", id));

			SqliteDataReader reader = cmd.ExecuteReader ();
			FileObj file = null;

			// Read and add file
			while (reader.Read ())
			{
				file = new FileObj ();

				file.ID = reader.GetInt64 (0);
				file.Path = reader.GetString (1);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();
			if (closeConnection) Database.Close ();

			return file;
		}

		// Close database connection
		if (closeConnection) Database.Close ();

		return null;
	}

	public FileObj GetFile (string path)
	{
		if (Database.Connect ())
		{
			// Send database query
			SqliteCommand cmd = new SqliteCommand (Database.Connection);
			cmd.CommandText = "SELECT id,path FROM file WHERE path = @File";

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("File", path));

			SqliteDataReader reader = cmd.ExecuteReader ();
			FileObj file = null;

			// Read and add file
			while (reader.Read ()) {
				file = new FileObj ();

				file.ID = reader.GetInt64 (0);
				file.Path = reader.GetString (1);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();
			Database.Close ();

			return file;
		}

		// Close database connection
		Database.Close ();

		return null;
	}

	public bool DeletePlaylist (PlaylistObj playlist)
	{
		if (Database.Connect () && playlist != null)
		{
			// Query statement
			string sql = "DELETE FROM playlist WHERE id = @ID";
			SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

			// Result
			int result = cmd.ExecuteNonQuery ();

			// Close database connection
			cmd.Dispose ();
			Database.Close ();

			return result > 0;
		}

		// Close database connection
		Database.Close ();

		return false;
	}

	public bool DeleteFile (PlaylistObj playlist, FileObj file)
	{
		if (Database.Connect () && playlist != null && file != null && playlist.Files.Contains (file))
		{
			// Select files of playlist
			string sql = "SELECT files FROM playlist WHERE id = @ID";
			SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read file IDs
			List<string> fileIDs = null;
			while (reader.Read ()) {
				if (!reader.IsDBNull (0)) {
					fileIDs = new List<string> (reader.GetString (0).Split (new Char[] { ',', ' ' }));
				}
			}

			// Close reader
			reader.Close();
			cmd.Dispose ();

			if (fileIDs != null && fileIDs.Contains (file.ID.ToString ()))
			{
				// Remove file
				fileIDs.Remove (file.ID.ToString ());

				// Query statement
				sql = "UPDATE playlist SET files = @Files WHERE id = @ID";
				cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Files", FormatFileIDs (fileIDs)));
				cmd.Parameters.Add (new SqliteParameter ("ID", playlist.ID));

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				Database.Close ();

				return result > 0;
			}
		}

		// Close database connection
		Database.Close ();

		return false;
	}

	public void DeleteExtantFile (FileObj file, PlaylistObj exclude)
	{
		if (Database.Connect () && !IsFileUsed (file, exclude))
		{
			// Query statement
			string sql = "DELETE FROM file WHERE id = @ID";
			SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", file.ID));

			// Send query
			cmd.ExecuteNonQuery ();

			// Dispose command
			cmd.Dispose ();
		}

		// Close database connection
		Database.Close ();
	}

	private bool IsFileUsed (FileObj file, PlaylistObj exclude)
	{
		if (file != null)
		{
			// Query statement
			string sql = "SELECT files FROM playlist WHERE files LIKE @Files" + (exclude != null ? " AND id != @ID" : "");
			SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("Files", file.ID));
			if (exclude != null) cmd.Parameters.Add (new SqliteParameter ("ID", exclude.ID));

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read file IDs
			while (reader.Read ())
			{
				if (!reader.IsDBNull (0))
				{
					// Get file IDs
					List<string> fileIDs = new List<string> (reader.GetString (0).Split (new Char[] { ',', ' ' }));

					// Check if list contains file
					if (fileIDs.Contains (file.ID.ToString ())) return true;
				}
			}
		}

		return false;
	}



	//-- HELPER METHODS

	public string FormatFileIDs (List<string> fileIDs)
	{
		// Create FileObj list
		List<FileObj> files = new List<FileObj> (fileIDs.Count);
		foreach (string id in fileIDs) {
			FileObj file = new FileObj ();
			file.ID = Int64.Parse (id);
			files.Add (file);
		}

		return FormatFileIDs (files);
	}

	public string FormatFileIDs (List<FileObj> files)
	{
		// Output
		List<string> IDs = new List<string> ();

		// Add IDs
		foreach (FileObj file in files) {
			if (file.ID != 0) {
				IDs.Add (file.ID.ToString ());
			}
		}

		return IDs.Count > 0 ? String.Join (",", IDs.ToArray ()) : null;
	}
}