using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Playlist : MonoBehaviour {

	// Database connection
	private SqliteConnection db;

	// Playlists
	public List<PlaylistObj> playlists = new List<PlaylistObj>();

	// Playlist List GameObject
	public GameObject playlist;



	void Start ()
	{
		// Get reference to GameObjects
		playlist = GameObject.Find ("PlaylistGrid");

		// Connect to database
		DbConnect ();

		// Select playlists from database
		Load ();

		// Display playlists
		Display ();

		// Close database connection
		DbClose ();
	}



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
				obj.Files = files.ToArray();

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

	void Display ()
	{
		foreach (PlaylistObj p in playlists)
		{
			// Create GameOject
			GameObject playlist = DisplayPlaylist ("#" + p.ID);
			Text textPlaylist = playlist.GetComponent<Text> ();

			// Text settings
			textPlaylist.color = Color.white;
			textPlaylist.text = p.Name;

			// Create Event Trigger
			EventTrigger events = playlist.AddComponent<EventTrigger> ();

			// Add Click Event
			EventTrigger.Entry evtClick = new EventTrigger.Entry ();
			evtClick.eventID = EventTriggerType.PointerClick;
			events.triggers.Add (evtClick);

			evtClick.callback.AddListener ((eventData) => {
				ToggleFiles(p);
			});

			// Add Hover Enter Event
			EventTrigger.Entry evtHover = new EventTrigger.Entry ();
			evtHover.eventID = EventTriggerType.PointerEnter;
			events.triggers.Add (evtHover);

			evtHover.callback.AddListener ((eventData) => {
				// TODO
			});

			// Add Hover Exit Event
			EventTrigger.Entry evtExit = new EventTrigger.Entry ();
			evtExit.eventID = EventTriggerType.PointerExit;
			events.triggers.Add (evtExit);

			evtExit.callback.AddListener ((eventData) => {
				// TODO
			});

			// Add files
			foreach (FileObj f in p.Files)
			{
				// Create GameObject
				GameObject file = DisplayPlaylist ("#" + p.ID + "." + f.ID);
				Text textFile = file.GetComponent<Text> ();

				// Text settings
				textFile.color = Color.gray;
				textFile.text = f.Name;

				// Hide GameObject
				file.SetActive (false);
			}
		}
	}

	GameObject DisplayPlaylist (string name)
	{
		// Create GameOject
		GameObject gameObject = new GameObject (name);

		// Append GameObject to playlists GameObject
		gameObject.transform.SetParent (playlist.transform);

		// Add text
		Text text = gameObject.AddComponent<Text> ();

		// Set transformations
		text.rectTransform.sizeDelta = new Vector2 (398, 20);
		text.rectTransform.pivot = Vector2.up;

		// Font settings
		text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
		text.fontSize = 16;

		// Add Content Size Fitter
		// => now it's possible to get size of the text (used to append icons next to the text)
		ContentSizeFitter csf = gameObject.AddComponent<ContentSizeFitter> ();
		csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

		return gameObject;
	}

	void ToggleFiles (PlaylistObj playlist)
	{
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
					} else {
						file.SetActive (false);
					}
				}
			}
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

			for (int i=0; i < playlist.Files.Length; i++)
			{
				if (playlist.Files [i].ID != 0)
				{
					if (IDcount == 0) {
						files = "'";
					}

					files += playlist.Files [i].ID.ToString ();

					if (i != playlist.Files.Length-1) {
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
