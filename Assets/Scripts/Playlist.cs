using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;

public class Playlist : MonoBehaviour {

	// Database connection
	SqliteConnection db;

	// Playlists
	List<PlaylistObj> playlists = new List<PlaylistObj>();



	void Start ()
	{
		// Connect to database
		DbConnect ();

		// Select playlists from database
		LoadPlaylists ();

		// Close database connection
		DbClose ();
	}



	void LoadPlaylists ()
	{
		if (DbConnect ())
		{
			// Database command
			SqliteCommand cmd = new SqliteCommand (db);

			// Query statement
			string sql = "SELECT id,name,files FROM playlist";
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
				List<String> files = new List<String> ();
				foreach (string id in fileIDs)
				{
					// Send database query
					SqliteCommand cmd2 = new SqliteCommand (db);
					cmd2.CommandText = "SELECT path FROM file WHERE id = '" + id + "'";
					SqliteDataReader fileReader = cmd2.ExecuteReader ();

					// Read and add file paths
					while (fileReader.Read ()) {
						files.Add (fileReader.GetString (0));
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

	int CreatePlaylist (PlaylistObj playlist)
	{
		if (DbConnect () && playlist != null)
		{
			// SQL settings
			SqliteCommand cmd = null;
			SqliteDataReader reader = null;
			string sql = "";

			// Check if playlist files are already in database
			List<Int32> fileIDs = new List<Int32> ();
			foreach (string file in playlist.Files)
			{
				// Query statement
				sql = "SELECT id FROM file WHERE path = '" + file + "'";
				cmd = new SqliteCommand (sql, db);

				// Get sql results
				reader = cmd.ExecuteReader ();

				// Read and add file IDs
				int count = 0;
				while (reader.Read ()) {
					fileIDs.Add (reader.GetInt32 (0));
					count++;
				}

				// Close reader
				reader.Close ();
				cmd.Dispose ();

				// Add file to database if not already exists
				if (count == 0)
				{
					// Get file name
					string[] name = file.Split(new Char[] {'/', '\\'});

					// Query statement
					sql = "INSERT INTO file (name,path) VALUES(" +
						"'" + name[name.Length-1] + "'," +
						"'" + file + "')";
					cmd = new SqliteCommand (sql, db);

					// Execute statement
					cmd.ExecuteNonQuery ();
					cmd.Dispose ();

					// Read id: Query statement
					sql = "SELECT id FROM file WHERE path = '" + file + "'";
					cmd = new SqliteCommand (sql, db);

					// Read id: Get sql results
					reader = cmd.ExecuteReader ();

					// Read id
					while (reader.Read ()) {
						fileIDs.Add (reader.GetInt32 (0));
					}

					// Close reader
					reader.Close ();
					cmd.Dispose ();
				}
			}

			// Format file IDs
			string files = "NULL";
			if (fileIDs.Count > 0)
			{
				files = "'" + fileIDs [0].ToString ();

				for(int i=1; i < fileIDs.Count; i++) {
					files += "," + fileIDs [i].ToString ();
				}

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
