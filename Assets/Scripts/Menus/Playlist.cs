using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class Playlist : MonoBehaviour {

	// Reference to transformation
	public Transform trans;

	// Dialog reference
	public GameObject dialogRef;
	public Dialog dialog;

	// Database connection
	public SqliteConnection db;

	// Playlists
	public List<PlaylistObj> playlists;

	// Playlist List GameObject
	public GameObject playlist;

	// Active playlist
	public static PlaylistObj activePlaylist;

	// Active file
	public static FileObj activeFile;



	public void Start ()
	{
		// Set references
		trans = transform;
		playlist = GameObject.Find ("PlaylistContent");

		// Set dialog
		dialog = new Dialog (dialogRef);

		// Connect to database
		DbConnect ();

		// Select playlists from database
		Load ();

		// Display playlists
		Display ();

		// Close database connection
		DbClose ();
	}



	public void Display ()
	{
		// Remove all GameObjects
		foreach (Transform pl in playlist.transform) {
			Destroy (pl.gameObject);
		}

		foreach (PlaylistObj p in playlists)
		{
			// Create GameOject
			GameObject playlist = DisplayPlaylist (p);
			GameObject goText = playlist.transform.Find ("Text").gameObject;
			Text textPlaylist = goText.GetComponent<Text> ();

			// Text settings
			textPlaylist.text = p.Name;

			// Create Event Triggers
			EventTrigger events = playlist.AddComponent<EventTrigger> ();

			// Add Click Event
			EventTrigger.Entry evtClick = new EventTrigger.Entry ();
			evtClick.eventID = EventTriggerType.PointerClick;
			events.triggers.Add (evtClick);

			evtClick.callback.AddListener ((eventData) => {
				ToggleFiles(playlist);
			});

			// Add files
			foreach (FileObj f in p.Files)
			{
				// Create GameObject
				GameObject file = DisplayPlaylist (p, f);
				GameObject goFileText = file.transform.Find ("Text").gameObject;
				Text textFile = goFileText.GetComponent<Text> ();

				// Text settings
				textFile.text = Path.GetFileName (f.Path);

				// Hide GameObject
				file.SetActive (false);
			}
		}
	}

	public GameObject DisplayPlaylist (PlaylistObj playlist)
	{
		return DisplayPlaylist (playlist, null);
	}

	public GameObject DisplayPlaylist (PlaylistObj playlist, FileObj file)
	{
		if (playlist != null)
		{
			// Construct name
			string name = "#" + playlist.ID;
			if (file != null && playlist.Files.Contains (file)) {
				name += "." + file.ID;
			}

			// Create GameOject
			GameObject gameObject = new GameObject (name);
			gameObject.transform.SetParent (this.playlist.transform);

			// Add Layout Element to GameObject
			LayoutElement goLayout = gameObject.AddComponent<LayoutElement> ();
			goLayout.minHeight = 30;
			goLayout.preferredHeight = goLayout.minHeight;

			// Add image to GameObject
			Image goImg = gameObject.AddComponent<Image> ();
			goImg.color = Color.clear;

			// Set transformations
			goImg.rectTransform.pivot = Vector2.up;

			// Add Horizontal Layout Group
			HorizontalLayoutGroup hlg = gameObject.AddComponent<HorizontalLayoutGroup> ();
			hlg.spacing = 10;
			hlg.childForceExpandWidth = false;
			hlg.childForceExpandHeight = false;
			hlg.childAlignment = TextAnchor.MiddleLeft;


			// Create arrow text GameObject
			GameObject goArrow = new GameObject ("Arrow");
			goArrow.transform.SetParent (gameObject.transform);

			// Add text
			TextUnicode textArrow = goArrow.AddComponent<TextUnicode> ();
			if (playlist.Equals(activePlaylist) && file == null) {
				textArrow.text = IconFont.DROPDOWN_CLOSED;
			}

			// Set text alignment
			textArrow.alignment = TextAnchor.MiddleLeft;

			// Font settings
			textArrow.font = IconFont.font;
			textArrow.fontSize = 30;

			// Add Layout Element
			LayoutElement layoutElementArrow = goArrow.AddComponent<LayoutElement> ();
			layoutElementArrow.minWidth = 30;


			// Create text GameObject
			GameObject goText = new GameObject ("Text");
			goText.transform.SetParent (gameObject.transform);

			// Add text
			Text text = goText.AddComponent<Text> ();

			// Set text alignment
			text.alignment = TextAnchor.MiddleLeft;

			// Set text color
			text.color = file == null ? Color.white : Color.gray;

			// Font settings
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.fontSize = 30;


			if (file != null)
			{	
				// Create listening text GameObject
				GameObject goListening = new GameObject ("Listening");
				goListening.transform.SetParent (gameObject.transform);

				// Add text
				TextUnicode textListening = goListening.AddComponent<TextUnicode> ();
				if (playlist.Equals (activePlaylist) && file.Equals (activeFile)) {
					textListening.text = IconFont.LISTENING;
				}

				// Set text alignment
				textListening.alignment = TextAnchor.MiddleRight;

				// Font settings
				textListening.font = IconFont.font;
				textListening.fontSize = 30;
			}


			// Create edit icons GameObject
			GameObject goEditIcons = new GameObject ("Images");
			goEditIcons.transform.SetParent (gameObject.transform);

			// Set transformations
			RectTransform editIconsTrans = goEditIcons.AddComponent<RectTransform> ();
			editIconsTrans.anchoredPosition = Vector2.zero;
			editIconsTrans.anchorMin = new Vector2 (1.0f, 0.5f);
			editIconsTrans.anchorMax = new Vector2 (1.0f, 0.5f);
			editIconsTrans.pivot = new Vector2 (1.0f, 0.5f);

			// Add Layout Element
			LayoutElement layoutElementEditIcons = goEditIcons.AddComponent<LayoutElement> ();
			layoutElementEditIcons.ignoreLayout = true;

			// Add Content Size Fitter
			ContentSizeFitter csfEditIcons = goEditIcons.AddComponent<ContentSizeFitter> ();
			csfEditIcons.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			csfEditIcons.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			// Add Layout Group to GameObject
			HorizontalLayoutGroup hlgImg = goEditIcons.AddComponent<HorizontalLayoutGroup> ();
			hlgImg.childAlignment = TextAnchor.MiddleRight;
			hlgImg.spacing = 5;
			hlgImg.childForceExpandWidth = false;
			hlgImg.childForceExpandHeight = false;

			// Disable edit icons GameObject
			goEditIcons.SetActive (false);


			if (file == null)
			{
				// Create edit text GameObject
				GameObject goEdit = new GameObject ("Edit");
				goEdit.transform.SetParent (goEditIcons.transform);

				// Add text
				TextUnicode editText = goEdit.AddComponent<TextUnicode> ();
				editText.text = IconFont.EDIT;

				// Set text alignment
				editText.alignment = TextAnchor.MiddleRight;

				// Set transformations
				editText.rectTransform.sizeDelta = new Vector2 (20, 30);

				// Font settings
				editText.font = IconFont.font;
				editText.fontSize = 30;

				// Create edit text Event Trigger
				EventTrigger evtTextEdit = goEdit.AddComponent<EventTrigger> ();

				// Add Edit Click Event
				EventTrigger.Entry evtTextEditClick = new EventTrigger.Entry ();
				evtTextEditClick.eventID = EventTriggerType.PointerClick;
				evtTextEdit.triggers.Add (evtTextEditClick);

				evtTextEditClick.callback.AddListener ((eventData) => {
					ShowDialog ("PL_EDIT", gameObject);
				});
			}


			// Create delete text GameObject
			GameObject goDelete = new GameObject ("Delete");
			goDelete.transform.SetParent (goEditIcons.transform);

			// Add text
			Text deleteText = goDelete.AddComponent<Text> ();
			deleteText.text = IconFont.TRASH;

			// Set text alignment
			deleteText.alignment = TextAnchor.MiddleRight;

			// Set transformations
			deleteText.rectTransform.sizeDelta = new Vector2 (20, 30);

			// Font settings
			deleteText.font = IconFont.font;
			deleteText.fontSize = 30;


			// Create delete text Event Trigger
			EventTrigger evtTextDel = goDelete.AddComponent<EventTrigger> ();

			// Add Delete Click Event
			EventTrigger.Entry evtTextDelClick = new EventTrigger.Entry ();
			evtTextDelClick.eventID = EventTriggerType.PointerClick;
			evtTextDel.triggers.Add (evtTextDelClick);

			evtTextDelClick.callback.AddListener ((eventData) => {
				if (FindFile (gameObject) == null) {
					ShowDialog ("PL_DEL", gameObject);
				} else {
					Delete (gameObject);
				}
			});


			// Create GameObject Event Triggers
			EventTrigger evtWrapper = gameObject.AddComponent<EventTrigger> ();

			// Add Hover Enter Event
			EventTrigger.Entry evtHover = new EventTrigger.Entry ();
			evtHover.eventID = EventTriggerType.PointerEnter;
			evtWrapper.triggers.Add (evtHover);

			evtHover.callback.AddListener ((eventData) => {
				goEditIcons.SetActive (true);
			});

			// Add Hover Exit Event
			EventTrigger.Entry evtExit = new EventTrigger.Entry ();
			evtExit.eventID = EventTriggerType.PointerExit;
			evtWrapper.triggers.Add (evtExit);

			evtExit.callback.AddListener ((eventData) => {
				goEditIcons.SetActive (false);
			});


			return gameObject;
		}

		return null;
	}

	public void ToggleFiles (GameObject gameObject)
	{
		// Get playlist
		PlaylistObj playlist = FindPlaylist (gameObject);

		// Set playlist as active playlist
		activePlaylist = playlist;

		// Show or hide playlist files
		bool opened = false;
		foreach (PlaylistObj p in playlists)
		{
			foreach (FileObj f in p.Files)
			{
				// Get GameObject for current file
				GameObject file = this.playlist.transform.Find ("#" + p.ID + "." + f.ID).gameObject;

				// Toggle files for GameObject
				if (file != null) {
					if (p == playlist) {
						file.SetActive (!file.activeSelf);
						opened = file.activeSelf;
					} else {
						file.SetActive (false);
					}
				}
			}

			// Change arrows
			Text arr = this.playlist.transform.Find ("#" + p.ID).transform.Find ("Arrow").GetComponent<Text>();
			if (p != playlist) {
				arr.text = "";
			}
		}

		// Change arrow image
		Text arrow = gameObject.transform.Find ("Arrow").GetComponent<Text> ();
		if (arrow != null) {
			arrow.text = opened ? IconFont.DROPDOWN_OPENED : IconFont.DROPDOWN_CLOSED;
		}
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
			Load ();
			Display ();

			return id;
		}

		return (long) Database.Constants.QueryFailed;
	}

	public bool Delete (GameObject gameObject)
	{
		// Get file
		PlaylistObj playlist = FindPlaylist (gameObject);
		FileObj file = FindFile (gameObject);

		bool deleted = playlist != null ? (file != null ? DeleteFile (playlist, file) : DeletePlaylist (playlist)) : false;

		if (deleted) {
			// Remove from interface
			Destroy (gameObject);

			if (file == null) {
				// Remove from list
				playlists.Remove (playlist);

				// Unset active playlist
				if (playlist == activePlaylist) activePlaylist = null;
			}
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
			return playlists.Find (x => x.ID == playlistID);
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
		if (dialog != null)
		{
			// Playlist object
			PlaylistObj playlist = FindPlaylist (obj);

			// Content elements
			Transform header = dialog.wrapper.transform.Find ("Header");
			Transform main = dialog.wrapper.transform.Find ("Main");
			Transform footer = dialog.wrapper.transform.Find ("Footer");

			// Header elements
			Text heading = header.Find ("Heading").GetComponent<Text> ();

			// Main elements
			Text text;
			InputField inputField;
			Text inputText;

			// Footer elements
			Button buttonOK = footer.Find ("Button_OK").GetComponent<Button> ();
			Text buttonOKText = footer.Find ("Button_OK").Find ("Text").GetComponent<Text> ();
			Button buttonCancel = footer.Find ("Button_Cancel").GetComponent<Button> ();
			Text buttonCancelText = footer.Find ("Button_Cancel").Find ("Text").GetComponent<Text> ();


			switch (type) {

			// New playlist
			case "PL_ADD":
				
				// UI elements
				heading.text = "Neue Playlist erstellen";
				inputField = dialog.AddInputField ("", "Wie soll die neue Playlist heißen?");
				inputText = inputField.transform.Find ("Text").gameObject.GetComponent<Text> ();

				// Events
				buttonOK.onClick.AddListener (delegate {
					long id = NewPlaylist (inputText.text);

					switch (id) {
					case (long) Database.Constants.DuplicateFound:
						// TODO
						print("Bereits vorhanden.");
						break;

					case (long) Database.Constants.QueryFailed:
						// TODO
						print("Fehlgeschlagen.");
						break;

					default:
						HideDialog ();
						break;

					}
				});

				break;



			// Edit playlist
			case "PL_EDIT":
				
				if (playlist != null)
				{
					// UI elements
					heading.text = "Playlist bearbeiten";
					inputField = dialog.AddInputField (playlist.Name, playlist.Name);
					inputText = inputField.transform.Find ("Text").gameObject.GetComponent<Text> ();

					// Events
					buttonOK.onClick.AddListener (delegate {
						
						playlist.Name = inputText.text;
						bool edited = Edit (playlist);

						if (edited) {
							Load ();
							Display ();
						}

						HideDialog ();
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
					heading.text = "Playlist löschen";
					text = dialog.AddText ("Playlist \"" + playlist.Name + "\" endgültig löschen?");

					// Events
					buttonOK.onClick.AddListener (delegate {
						
						Delete (obj);
						Load ();
						Display ();
						HideDialog ();
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
			dialog.dialog.SetActive (true);

			return;
		}

		return;
	}

	public void HideDialog ()
	{
		if (dialog != null) dialog.HideDialog ();
	}



	//-- DATABASE METHODS

	public void Load ()
	{
		if (DbConnect ())
		{
			// Reset playlists list
			playlists = new List<PlaylistObj> ();

			// Database command
			SqliteCommand cmd = new SqliteCommand (db);

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
					if (file != null) {
						files.Add (file);
					}
				}

				// Set files
				obj.Files = files;

				// Add contents to playlists array
				playlists.Add (obj);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();

			// Close database connection
			DbClose ();
		}
	}

	public long Create (PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null && playlist.Name.Length > 0)
		{
			// SQL settings
			SqliteCommand cmd = null;
			SqliteDataReader reader = null;
			string sql = "";

			// Check if playlist files are already in database
			foreach (FileObj file in playlist.Files)
			{
				// Query statement
				sql = "SELECT id FROM file WHERE path = '" + file.Path + "'";
				cmd = new SqliteCommand (sql, db);

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
					sql = "INSERT INTO file (path) VALUES('" + file.Path + "'); " +
						"SELECT last_insert_rowid()";
					cmd = new SqliteCommand (sql, db);

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
				sql = "INSERT INTO playlist (name,files) VALUES(" +
					"'" + playlist.Name + "'," +
					files + "); SELECT last_insert_rowid()";
				cmd = new SqliteCommand (sql, db);

				// Execute insert statement and get ID
				long id = (long) cmd.ExecuteScalar ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				return id;
			}
			catch (SqliteException)
			{
				// Close database connection
				DbClose ();

				return (long) Database.Constants.DuplicateFound;
			}
		}

		// Close database connection
		DbClose ();

		return (long) Database.Constants.QueryFailed;
	}

	public bool Edit(PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null && playlist.Name.Length > 0)
		{
			try
			{
				// Query statement
				string sql = "UPDATE playlist SET " +
					"name = '" + playlist.Name + "', " +
					"files = " + FormatFileIDs (playlist.Files) + " " +
					"WHERE id = '" + playlist.ID + "'";
				SqliteCommand cmd = new SqliteCommand (sql, db);

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				return result > 0;
			}
			catch (SqliteException) {}
		}

		// Close database connection
		DbClose ();

		return false;
	}

	public bool AddFile (FileObj file, PlaylistObj playlist)
	{
		if (DbConnect () && file != null && playlist != null)
		{
			// Reset file ID
			file.ID = 0;

			// Update file ID: Query statement
			string sql = "SELECT id FROM file WHERE path = '" + file.Path + "'";
			SqliteCommand cmd = new SqliteCommand (sql, db);

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
				sql = "INSERT INTO file (path) VALUES ('" + file.Path + "'); " +
					"SELECT last_insert_rowid()";
				cmd = new SqliteCommand (sql, db);

				// Send query
				file.ID = (long) cmd.ExecuteScalar ();
				cmd.Dispose ();
			}

			if (!playlist.Files.Contains (file))
			{
				// Add file to playlist
				playlist.Files.Add (file);

				// Set file IDs
				string files = FormatFileIDs (playlist.Files);

				// Query statement
				sql = "UPDATE playlist SET files = " + files + " WHERE id = '" + playlist.ID + "'";
				cmd = new SqliteCommand (sql, db);

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				return result > 0;
			}
		}

		// Close database connection
		DbClose ();

		return false;
	}

	public FileObj GetFile (long id, bool closeConnection)
	{
		if (DbConnect ())
		{
			// Send database query
			SqliteCommand cmd = new SqliteCommand (db);
			cmd.CommandText = "SELECT id,path FROM file WHERE id = '" + id.ToString () + "'";
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
			if (closeConnection) DbClose ();

			return file;
		}

		// Close database connection
		if (closeConnection) DbClose ();

		return null;
	}

	public FileObj GetFile (string path)
	{
		if (DbConnect ())
		{
			// Send database query
			SqliteCommand cmd = new SqliteCommand (db);
			cmd.CommandText = "SELECT id,path FROM file WHERE path = '" + path + "'";
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
			DbClose ();

			return file;
		}

		// Close database connection
		DbClose ();

		return null;
	}

	public bool DeletePlaylist (PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null)
		{
			// Query statement
			string sql = "DELETE FROM playlist WHERE id = '" + playlist.ID + "'";
			SqliteCommand cmd = new SqliteCommand (sql, db);

			// Result
			int result = cmd.ExecuteNonQuery ();

			// Close database connection
			cmd.Dispose ();
			DbClose ();

			return result > 0;
		}

		// Close database connection
		DbClose ();

		return false;
	}

	public bool DeleteFile (PlaylistObj playlist, FileObj file)
	{
		if (DbConnect () && playlist != null && file != null && playlist.Files.Contains (file))
		{
			// Select files of playlist
			string sql = "SELECT files FROM playlist WHERE id = '" + playlist.ID + "'";
			SqliteCommand cmd = new SqliteCommand (sql, db);

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();

			// Read file IDs
			List<String> fileIDs = null;
			while (reader.Read ()) {
				if (!reader.IsDBNull (0)) {
					fileIDs = new List<String> (reader.GetString (0).Split (new Char[] { ',', ' ' }));
				}
			}

			// Close reader
			reader.Close();
			cmd.Dispose ();

			if (fileIDs != null && fileIDs.Contains (file.ID.ToString ()))
			{
				// Remove file
				fileIDs.Remove (file.ID.ToString ());

				// Update file IDs
				string files = FormatFileIDs (fileIDs);

				// Query statement
				sql = "UPDATE playlist SET " +
					"files = " + files + " " +
					"WHERE id = '" + playlist.ID + "'";
				cmd = new SqliteCommand (sql, db);

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				DbClose ();

				return result > 0;
			}

			// TODO remove file if no other playlist contains it
		}

		// Close database connection
		DbClose ();

		return false;
	}



	//-- DATABASE CONNECTION

	public bool DbConnect ()
	{
		if (db == null) {
			db = Database.GetConnection ();
		}

		return db != null;
	}

	public void DbClose ()
	{
		// Close database
		Database.Close ();

		// Reset database instance
		db = null;
	}



	//-- HELPER METHODS

	public string FormatFileIDs (List<String> fileIDs)
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
		string output = "NULL";

		// Number of IDs
		int IDcount = 0;

		for (int i=0; i < files.Count; i++)
		{
			if (files [i].ID != 0)
			{
				// Insert starting apostrophe for sql query
				if (IDcount == 0) {
					output = "'";
				}

				// Insert ID
				output += files [i].ID.ToString ();

				// Insert comma
				if (i != files.Count-1) {
					output += ",";
				}

				IDcount++;
			}
		}

		// Insert final apostrophe for sql query
		if (IDcount > 0) {
			output += "'";
		}

		return output;
	}
}