using UnityEngine;
using UnityEngine.UI;

using Mono.Data.Sqlite;

using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Offers connection to the local database. It should be only one connection opened
/// which is realized through the use of the Singleton pattern.
/// </summary>
/// <remarks>
/// == Usage ==
/// SqliteConnection db = Database.getConnection ();
/// SqliteCommand cmd = new SqliteCommand (db);
/// // and so on ...
/// </remarks>
public class Database {

	/// <summary>
	/// The database filename.
	/// </summary>
	private static string NAME = "beatlux.db";

	/// <summary>
	/// The instance of this class (Singleton).
	/// </summary>
	private static Database Instance;

	/// <summary>
	/// The database connection.
	/// </summary>
	public static SqliteConnection Connection;


	/// <summary>
	/// Database query result constants defining the result state of a database query.
	/// </summary>
	public enum Constants : long
	{
		Successful = -1,

		QueryFailed = -10,
		DuplicateFound = -11,
		EmptyInputValue = -12,
	}


	/// <summary>
	/// Initializes a new instance of the <see cref="Database" /> class.
	/// </summary>
	private Database ()
	{
		// Set path to database.
		string uri = "Data Source=" + Application.dataPath + "/" + NAME;

		// Try to connect to database.
		Connection = Connect (uri);
	}

	/// <summary>
	/// Try to open a connection to the database.
	/// </summary>
	/// <param name="uri">The database uri.</param>
	/// <returns>
	/// <see cref="Mono.Data.Sqlite.SqliteConnection" /> object which holds the database connection
	/// </returns>
	private SqliteConnection Connect (string uri)
	{
		// Connect to database
		Connection = new SqliteConnection (uri);

		// Open database connection
		Connection.Open ();

		return Connection;
	}

	/// <summary>
	/// Close connection to the database.
	/// </summary>
	public static void Close ()
	{
		// Close connection to database.
		if (Connection != null) Connection.Close ();

		// Reset class instance and connection.
		Instance = null;
		Connection = null;
	}

	/// <summary>
	/// Creates the database tables.
	/// </summary>
	public static void CreateTables ()
	{
		// Sql statements for table creation.
		string[] stm = {
			"CREATE TABLE IF NOT EXISTS `file` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `path` TEXT NOT NULL UNIQUE )",
			"CREATE TABLE IF NOT EXISTS `playlist` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `files` TEXT DEFAULT NULL )",
			"CREATE TABLE IF NOT EXISTS `visualization` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `colors` INTEGER NOT NULL DEFAULT 1, `buildNumber` INTEGER UNIQUE, `skybox` TEXT DEFAULT NULL )",
			"CREATE TABLE IF NOT EXISTS `color_scheme` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `viz_id` INTEGER, `colors` TEXT NOT NULL, FOREIGN KEY(`viz_id`) REFERENCES `visualization`(`id`) )",
		};

		// Create database tables.
		if (GetConnection () != null)
		{
			foreach (string sql in stm)
			{
				SqliteCommand cmd = new SqliteCommand (sql, Connection);
				cmd.ExecuteNonQuery ();
				cmd.Dispose ();
			}
		}
	}

	/// <summary>
	/// Inserts all available visualizations into the database.
	/// </summary>
	public static void InsertVisualizations ()
	{
		if (GetConnection () != null)
		{
			for (int i=0; i < Settings.Visualizations.Length; i++)
			{
				// Get all available visualizations.
				VisualizationObj viz = Settings.Visualizations [i];

				if (Application.CanStreamedLevelBeLoaded (viz.BuildNumber))
				{
					// Query statement
					string sql = "INSERT INTO visualization (name, colors, buildNumber, skybox) " +
						"VALUES (@Name, @Colors, @BuildNumber, @Skybox); " +
						"SELECT last_insert_rowid()";
					SqliteCommand cmd = new SqliteCommand (sql, Connection);

					// Add Parameters to statement
					cmd.Parameters.Add (new SqliteParameter ("Name", viz.Name));
					cmd.Parameters.Add (new SqliteParameter ("Colors", viz.Colors));
					cmd.Parameters.Add (new SqliteParameter ("BuildNumber", viz.BuildNumber));
					cmd.Parameters.Add (new SqliteParameter ("Skybox", viz.Skybox));

					try
					{
						// Execute insert statement
						long id = (long) cmd.ExecuteScalar ();

						// Update vz id.
						Settings.Visualizations [i].ID = id;

						// Dispose command
						cmd.Dispose ();
					}
					catch
					{
						// Dispose command
						cmd.Dispose ();

						// Select ID from database
						sql = "SELECT id FROM visualization WHERE name = @Name AND buildNumber = @BuildNumber";
						cmd = new SqliteCommand (sql, Connection);

						// Add Parameters to statement
						cmd.Parameters.Add (new SqliteParameter ("Name", viz.Name));
						cmd.Parameters.Add (new SqliteParameter ("BuildNumber", viz.BuildNumber));

						// Get sql results
						SqliteDataReader reader = cmd.ExecuteReader ();

						// Read id
						while (reader.Read ()) {
							Settings.Visualizations [i].ID = reader.GetInt64 (0);
						}

						// Close reader
						reader.Close();
						cmd.Dispose ();
					}
				}
			}
		}
	}

	/// <summary>
	/// Inserts the default color scheme of each available visualization into the database.
	/// </summary>
	public static void InsertDefaultCS ()
	{
		if (GetConnection () != null && Settings.Visualizations != null && Settings.Visualizations.Length > 0)
		{
			foreach (VisualizationObj viz in Settings.Visualizations)
			{
				if (!ColorScheme.Exists (new ColorSchemeObj (viz.Name, viz)))
				{
					// Insert default color scheme.
					string sql = "INSERT INTO color_scheme (name, viz_id, colors) VALUES (@Name, @Viz_ID, @Colors)";
					SqliteCommand cmd = new SqliteCommand (sql, Connection);

					// Add Parameters to statement
					cmd.Parameters.Add (new SqliteParameter ("Name", viz.Name));
					cmd.Parameters.Add (new SqliteParameter ("Viz_ID", viz.ID));

					if (Settings.Defaults.Colors.ContainsKey (viz.Name))
					{
						// Set colors
						Color[] colors = Settings.Defaults.Colors [viz.Name];
						cmd.Parameters.Add (new SqliteParameter ("Colors", ColorScheme.FormatColors (colors)));

						// Execute insert statement
						cmd.ExecuteNonQuery ();

						// Dispose command
						cmd.Dispose ();
					}
				}
			}
		}
	}

	/// <summary>
	/// Inserts the default playlist into the database.
	/// </summary>
	public static void InsertDefaultPlaylist ()
	{
		if (GetConnection () != null)
		{
			// Database command
			SqliteCommand cmd = new SqliteCommand (Connection);

			// Query statement
			string sql = "SELECT id FROM playlist LIMIT 1";
			cmd.CommandText = sql;

			// Get sql results
			SqliteDataReader reader = cmd.ExecuteReader ();


			if (!reader.HasRows)
			{
				// Dispose command
				cmd.Dispose ();

				// Create default playlist
				sql = "INSERT INTO playlist (name) VALUES(@Name)";
				cmd = new SqliteCommand (sql, Connection);

				// Add parameters
				cmd.Parameters.Add (new SqliteParameter ("Name", "Playlist"));

				// Send query
				cmd.ExecuteNonQuery ();
			}


			// Close reader
			reader.Close ();
			cmd.Dispose ();
		}
	}
		
	/// <summary>
	/// Gets the connection if class has already been instantiated. If not, tries to
	/// connect to database and inserts all tables and default datasets.
	/// </summary>
	/// <returns>The database connection.</returns>
	public static SqliteConnection GetConnection ()
	{
		// Create instance if not exists.
		if (Instance == null)
		{
			Instance = new Database ();

			CreateTables ();
			InsertVisualizations ();
			InsertDefaultCS ();
			InsertDefaultPlaylist ();
		}

		return Connection;
	}

	/// <summary>
	/// Connect to database. Needed to instantiate <see cref="Database" /> class.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the connection was established; otherwise, <c>false</c>.
	/// </returns>
	public static bool Connect ()
	{
		if (Connection == null) {
			Connection = GetConnection ();
		}

		return Connection != null;
	}
}
