using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class Playlist : MonoBehaviour {

	// Database connection
	private SqliteConnection db;

	// Playlists
	public List<PlaylistObj> playlists = new List<PlaylistObj>();

	// Playlist List GameObject
	public GameObject playlist;

	// Active playlist
	public static PlaylistObj active;



	void Start ()
	{
		// Connect to database
		DbConnect ();

		// Select playlists from database
		Load ();

		// Display playlists
		Display ();

		// Close database connection
		DbClose ();
	}



	void Display ()
	{
		foreach (PlaylistObj p in playlists)
		{
			// Create GameOject
			GameObject playlist = DisplayPlaylist (p);
			GameObject goText = playlist.transform.Find ("Text").gameObject;
			Text textPlaylist = goText.GetComponent<Text> ();

			// Text settings
			textPlaylist.color = Color.white;
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
				textFile.color = Color.gray;
				textFile.text = f.Name;

				// Hide GameObject
				file.SetActive (false);
			}
		}
	}

	private GameObject DisplayPlaylist (PlaylistObj playlist)
	{
		return DisplayPlaylist (playlist, null);
	}

	private GameObject DisplayPlaylist (PlaylistObj playlist, FileObj file)
	{
		if (playlist != null)
		{
			// Construct name
			string name = "#" + playlist.ID;
			if (file != null && playlist.Files.Contains (file))
				name += "." + file.ID;

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


			// Create arrow image GameObject
			GameObject goArrow = new GameObject ("Arrow");
			goArrow.transform.SetParent (gameObject.transform);

			// Add arrow image
			Image imgArrow = goArrow.AddComponent<Image> ();
			imgArrow.sprite = Resources.Load<Sprite> ("Images/" + (playlist == active && file == null ? "arrow-right" : "empty"));

			// Set arrow image transformations
			RectTransform imgArrowTrans = goArrow.GetComponent<RectTransform> ();
			imgArrowTrans.pivot = Vector2.up;
			imgArrowTrans.localPosition = Vector2.zero;
			imgArrowTrans.sizeDelta = new Vector2 (30, 30);
			imgArrowTrans.anchorMin = Vector2.up;
			imgArrowTrans.anchorMax = Vector2.up;


			// Create text GameObject
			GameObject goText = new GameObject ("Text");
			goText.transform.SetParent (gameObject.transform);

			// Add text
			Text text = goText.AddComponent<Text> ();

			// Set text alignment
			text.alignment = TextAnchor.MiddleLeft;

			// Set text transformations
			text.rectTransform.pivot = Vector2.up;
			text.rectTransform.localPosition = new Vector2(35, 0);
			text.rectTransform.sizeDelta = new Vector2(100, 30);
			text.rectTransform.anchorMin = Vector2.up;
			text.rectTransform.anchorMax = Vector2.up;

			// Font settings
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.fontSize = 16;

			// Add Content Size Fitter
			ContentSizeFitter csf = goText.AddComponent<ContentSizeFitter> ();
			csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;


			// Create images GameObject
			GameObject goImages = new GameObject ("Images");
			goImages.transform.SetParent (gameObject.transform);

			// Set images GameObject transformations
			RectTransform imgTrans = goImages.AddComponent<RectTransform> ();
			imgTrans.pivot = Vector2.one;
			imgTrans.localPosition = Vector2.zero;
			imgTrans.sizeDelta = new Vector2 (100, 30);
			imgTrans.anchorMin = Vector2.one;
			imgTrans.anchorMax = Vector2.one;

			// Add Layout Group to GameObject
			HorizontalLayoutGroup hlgImg = goImages.AddComponent<HorizontalLayoutGroup> ();
			hlgImg.childAlignment = TextAnchor.MiddleRight;
			hlgImg.spacing = 15;
			hlgImg.childForceExpandWidth = false;
			hlgImg.childForceExpandHeight = false;


			if (file == null)
			{
				// Create edit image GameObject
				GameObject goEdit = new GameObject ("Edit");
				goEdit.transform.SetParent (goImages.transform);

				// Add image
				Image editImg = goEdit.AddComponent<Image> ();
				editImg.sprite = Resources.Load<Sprite> ("Images/edit");

				// Set transformations
				editImg.rectTransform.sizeDelta = new Vector2 (20, 20);

				// Create images Event Triggers
				EventTrigger evtImgEdit = goEdit.AddComponent<EventTrigger> ();

				// Add Image Edit Click Event
				EventTrigger.Entry evtImgEditClick = new EventTrigger.Entry ();
				evtImgEditClick.eventID = EventTriggerType.PointerClick;
				evtImgEdit.triggers.Add (evtImgEditClick);

				evtImgEditClick.callback.AddListener ((eventData) => {
					// TODO
				});
			}


			// Create delete image GameObject
			GameObject goDelete = new GameObject ("Delete");
			goDelete.transform.SetParent (goImages.transform);

			// Add image
			Image deleteImg = goDelete.AddComponent<Image> ();
			deleteImg.sprite = Resources.Load<Sprite> ("Images/delete");

			// Disable images GameObject
			goImages.SetActive (false);


			// Create GameObject Event Triggers
			EventTrigger evtWrapper = gameObject.AddComponent<EventTrigger> ();

			// Add Hover Enter Event
			EventTrigger.Entry evtHover = new EventTrigger.Entry ();
			evtHover.eventID = EventTriggerType.PointerEnter;
			evtWrapper.triggers.Add (evtHover);

			evtHover.callback.AddListener ((eventData) => {
				goImages.SetActive (true);
			});

			// Add Hover Exit Event
			EventTrigger.Entry evtExit = new EventTrigger.Entry ();
			evtExit.eventID = EventTriggerType.PointerExit;
			evtWrapper.triggers.Add (evtExit);

			evtExit.callback.AddListener ((eventData) => {
				goImages.SetActive (false);
			});


			// Create images Event Triggers
			EventTrigger evtImgDel = goDelete.AddComponent<EventTrigger> ();

			// Add Image Delete Click Event
			EventTrigger.Entry evtImgDelClick = new EventTrigger.Entry ();
			evtImgDelClick.eventID = EventTriggerType.PointerClick;
			evtImgDel.triggers.Add (evtImgDelClick);

			evtImgDelClick.callback.AddListener ((eventData) =>
			{
				// Get playlist and file
				PlaylistObj p = FindPlaylist (gameObject);
				FileObj f = FindFile (gameObject);

				if (EditorUtility.DisplayDialog("Playlist löschen", "Soll die Playlist \"" + p.Name + "\" wirklich gelöscht werden?", "Ja", "Nein"))
				{
					// Delete playlist or file
					bool deleted = Delete (gameObject);

					if (deleted) {
						// Remove from interface
						Destroy (gameObject);

						if (f == null) {
							// Remove from list
							playlists.Remove (p);

							// Unset active playlist
							if (p == active) active = null;
						}
					}
				}
			});


			return gameObject;
		}

		return null;
	}

	void ToggleFiles (GameObject gameObject)
	{
		// Get playlist
		PlaylistObj playlist = FindPlaylist (gameObject);

		// Set playlist as active playlist
		active = playlist;

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
			Image arr = this.playlist.transform.Find ("#" + p.ID).transform.Find ("Arrow").GetComponent<Image>();
			if (p != playlist) {
				arr.sprite = Resources.Load<Sprite> ("Images/empty");
			}
		}

		// Change arrow image
		Image arrow = gameObject.transform.Find ("Arrow").GetComponent<Image> ();
		if (arrow != null) {
			arrow.sprite = Resources.Load<Sprite> ("Images/arrow-" + (opened ? "down" : "right"));
		}
	}

	bool Delete (GameObject gameObject)
	{
		// Get file
		PlaylistObj playlist = FindPlaylist (gameObject);
		FileObj file = FindFile (gameObject);

		return playlist != null ? (file != null ? DeleteFile (playlist, file) : DeletePlaylist (playlist)) : false;
	}

	PlaylistObj FindPlaylist (GameObject gameObject)
	{
		// Get playlist id
		string[] name = gameObject.name.Split ('.');
		int playlistID = Int32.Parse (name [0].Split ('#') [1]);

		// Get playlist
		PlaylistObj playlist = playlists.Find(x => x.ID == playlistID);

		return playlist;
	}

	FileObj FindFile (GameObject gameObject)
	{
		// Get playlist and file id
		string[] name = gameObject.name.Split ('.');
		int playlistID = Int32.Parse (name [0].Split ('#') [1]);
		int fileID = name.Length > 1 ? Int32.Parse (name [1]) : 0;

		// Get playlist
		PlaylistObj playlist = FindPlaylist (gameObject);

		// Get file
		FileObj file = playlist.Files.Find(x => x.ID == fileID);

		return file;
	}



	//-- DATABASE METHODS

	void Load ()
	{
		if (DbConnect ())
		{
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
				PlaylistObj obj = new PlaylistObj ();

				// Set id and name
				obj.ID = reader.GetInt32 (0);
				obj.Name = reader.GetString (1);

				// Get file IDs
				string[] fileIDs = !reader.IsDBNull (2) ? reader.GetString (2).Split (new Char[] { ',', ' ' }) : new string[0];

				// Select files
				List<FileObj> files = new List<FileObj> ();
				foreach (string id in fileIDs)
				{
					// Send database query
					SqliteCommand cmd2 = new SqliteCommand (db);
					cmd2.CommandText = "SELECT id,name,path FROM file WHERE id = '" + id + "'";
					SqliteDataReader fileReader = cmd2.ExecuteReader ();

					// Read and add file
					while (fileReader.Read ())
					{
						FileObj file = new FileObj ();

						file.ID = fileReader.GetInt32 (0);
						file.Name = fileReader.GetString (1);
						file.Path = fileReader.GetString (2);

						files.Add (file);
					}

					// Close reader
					fileReader.Close ();
					cmd2.Dispose ();
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

	int Create (PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null)
		{
			// SQL settings
			SqliteCommand cmd = null;
			SqliteDataReader reader = null;
			string sql = "";

			// Check if playlist files are already in database
			foreach (FileObj file in playlist.Files)
			{
				// Query statement
				sql = "SELECT id FROM file WHERE path = '" + file.Path + "' AND name = '" + file.Name + "'";
				cmd = new SqliteCommand (sql, db);

				// Get sql results
				reader = cmd.ExecuteReader ();

				// Read and add file IDs
				int count = 0;
				while (reader.Read ()) {
					file.ID = reader.GetInt32 (0);
					count++;
				}

				// Close reader
				reader.Close ();
				cmd.Dispose ();

				// Add file to database if not already exists
				if (count == 0)
				{
					// Query statement
					sql = "INSERT INTO file (name,path) VALUES(" +
						"'" + file.Name + "'," +
						"'" + file.Path + "')";
					cmd = new SqliteCommand (sql, db);

					// Execute statement
					cmd.ExecuteNonQuery ();
					cmd.Dispose ();

					// Read id: Query statement
					sql = "SELECT id FROM file WHERE path = '" + file.Path + "' AND name = '" + file.Name + "'";
					cmd = new SqliteCommand (sql, db);

					// Read id: Get sql results
					reader = cmd.ExecuteReader ();

					// Read id
					while (reader.Read ()) {
						file.ID = reader.GetInt32 (0);
					}

					// Close reader
					reader.Close ();
					cmd.Dispose ();
				}
			}

			// Format file IDs
			string files = "NULL";
			int IDcount = 0;

			for (int i=0; i < playlist.Files.Count; i++)
			{
				if (playlist.Files [i].ID != 0)
				{
					if (IDcount == 0) {
						files = "'";
					}

					files += playlist.Files [i].ID.ToString ();

					if (i != playlist.Files.Count-1) {
						files += ",";
					}

					IDcount++;
				}
			}

			if (IDcount > 0) {
				files += "'";
			}

			// Insert playlist into database
			try
			{
				sql = "INSERT INTO playlist (name,files) VALUES(" +
					"'" + playlist.Name + "'," +
					files + ")";
				cmd = new SqliteCommand (sql, db);

				// Execute insert statement
				cmd.ExecuteNonQuery ();
				cmd.Dispose ();

				// Select id of inserted playlist
				sql = "SELECT id FROM playlist WHERE name = '" + playlist.Name + "'";
				cmd = new SqliteCommand (sql, db);

				// Get sql results
				reader = cmd.ExecuteReader ();

				// Read id
				int playlistId = (int) Database.Constants.QueryFailed;
				while (reader.Read ()) {
					playlistId = reader.GetInt32 (0);
				}

				// Close reader
				reader.Close();
				cmd.Dispose ();

				// Close database connection
				DbClose ();

				return playlistId;
			}
			catch (SqliteException)
			{
				return (int) Database.Constants.DuplicateFound;
			}
		}

		return (int) Database.Constants.QueryFailed;
	}

	bool DeletePlaylist (PlaylistObj playlist)
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

		return false;
	}

	bool DeleteFile (PlaylistObj playlist, FileObj file)
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
				string files = "NULL";
				int IDcount = 0;

				for (int i=0; i < fileIDs.Count; i++)
				{
					if (Int32.Parse (fileIDs [i]) != 0)
					{
						if (IDcount == 0) {
							files = "'";
						}

						files += fileIDs [i];

						if (i != fileIDs.Count-1) {
							files += ",";
						}

						IDcount++;
					}
				}

				if (IDcount > 0) {
					files += "'";
				}

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
		}

		return false;
	}



	//-- DATABASE CONNECTION

	bool DbConnect ()
	{
		if (db == null) {
			db = Database.GetConnection ();
		}

		return db != null;
	}

	void DbClose ()
	{
		// Close database
		Database.Close ();

		// Reset database instance
		db = null;
	}
}
