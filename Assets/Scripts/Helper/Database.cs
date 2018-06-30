/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using Mono.Data.Sqlite;
using UnityEngine;

/// <summary>
///     Offers connection to the local database. It should be only one connection opened
///     which is realized through the use of the Singleton pattern.
/// </summary>
/// <remarks>
///     == Usage ==
///     SqliteConnection db = Database.getConnection ();
///     SqliteCommand cmd = new SqliteCommand (db);
///     // and so on ...
/// </remarks>
public class Database
{
	/// <summary>
	///     Database query result constants defining the result state of a database query.
	/// </summary>
	public enum Constants : long
    {
        Successful = -1,

        QueryFailed = -10,
        DuplicateFound = -11,
        EmptyInputValue = -12
    }

	/// <summary>
	///     The database filename.
	/// </summary>
	private const string Name = "beatlux.db";

	/// <summary>
	///     The instance of this class (Singleton).
	/// </summary>
	private static Database _instance;

	/// <summary>
	///     The database connection.
	/// </summary>
	public static SqliteConnection Connection;


	/// <summary>
	///     Initializes a new instance of the <see cref="Database" /> class.
	/// </summary>
	private Database()
    {
        // Set path to database.
        var uri = "Data Source=" + Application.dataPath + "/" + Name;

        // Try to connect to database.
        Connection = Connect(uri);
    }

	/// <summary>
	///     Try to open a connection to the database.
	/// </summary>
	/// <param name="uri">The database uri.</param>
	/// <returns>
	///     <see cref="Mono.Data.Sqlite.SqliteConnection" /> object which holds the database connection
	/// </returns>
	private static SqliteConnection Connect(string uri)
    {
        // Connect to database
        Connection = new SqliteConnection(uri);

        // Open database connection
        Connection.Open();

        return Connection;
    }

	/// <summary>
	///     Close connection to the database.
	/// </summary>
	public static void Close()
    {
        // Close connection to database.
        if (Connection != null) Connection.Close();

        // Reset class instance and connection.
        _instance = null;
        Connection = null;
    }

	/// <summary>
	///     Creates the database tables.
	/// </summary>
	private static void CreateTables()
    {
        // Sql statements for table creation.
        string[] stm =
        {
            "CREATE TABLE IF NOT EXISTS `file` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `path` TEXT NOT NULL UNIQUE )",
            "CREATE TABLE IF NOT EXISTS `playlist` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `files` TEXT DEFAULT NULL )",
            "CREATE TABLE IF NOT EXISTS `visualization` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `colors` INTEGER NOT NULL DEFAULT 1, `buildNumber` INTEGER UNIQUE, `skybox` TEXT DEFAULT NULL )",
            "CREATE TABLE IF NOT EXISTS `color_scheme` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `viz_id` INTEGER, `colors` TEXT NOT NULL, FOREIGN KEY(`viz_id`) REFERENCES `visualization`(`id`) )"
        };

        // Create database tables.
        if (GetConnection() == null) return;
        foreach (var sql in stm)
        {
            var cmd = new SqliteCommand(sql, Connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }

	/// <summary>
	///     Inserts all available visualizations into the database.
	/// </summary>
	private static void InsertVisualizations()
    {
        if (GetConnection() == null) return;

        foreach (var viz in Settings.Visualizations)
        {
            if (!Application.CanStreamedLevelBeLoaded(viz.BuildNumber)) continue;

            // Query statement
            var sql = "INSERT INTO visualization (name, colors, buildNumber, skybox) " +
                      "VALUES (@Name, @Colors, @BuildNumber, @Skybox); " +
                      "SELECT last_insert_rowid()";
            var cmd = new SqliteCommand(sql, Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("Name", viz.Name));
            cmd.Parameters.Add(new SqliteParameter("Colors", viz.Colors));
            cmd.Parameters.Add(new SqliteParameter("BuildNumber", viz.BuildNumber));
            cmd.Parameters.Add(new SqliteParameter("Skybox", viz.Skybox));

            try
            {
                // Execute insert statement
                var id = (long) cmd.ExecuteScalar();

                // Update vz id.
                viz.Id = id;

                // Dispose command
                cmd.Dispose();
            }
            catch
            {
                // Dispose command
                cmd.Dispose();

                // Select ID from database
                sql = "SELECT id FROM visualization WHERE name = @Name AND buildNumber = @BuildNumber";
                cmd = new SqliteCommand(sql, Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Name", viz.Name));
                cmd.Parameters.Add(new SqliteParameter("BuildNumber", viz.BuildNumber));

                // Get sql results
                var reader = cmd.ExecuteReader();

                // Read id
                while (reader.Read()) viz.Id = reader.GetInt64(0);

                // Close reader
                reader.Close();
                cmd.Dispose();
            }
        }
    }

	/// <summary>
	///     Inserts the default color scheme of each available visualization into the database.
	/// </summary>
	private static void InsertDefaultColorScheme()
    {
        if (GetConnection() == null || Settings.Visualizations == null || Settings.Visualizations.Length <= 0) return;
        foreach (var viz in Settings.Visualizations)
        {
            if (ColorScheme.Exists(new ColorSchemeObj(viz.Name, viz))) continue;

            // Insert default color scheme.
            var sql = "INSERT INTO color_scheme (name, viz_id, colors) VALUES (@Name, @Viz_ID, @Colors)";
            var cmd = new SqliteCommand(sql, Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("Name", viz.Name));
            cmd.Parameters.Add(new SqliteParameter("Viz_ID", viz.Id));

            if (!Settings.Defaults.Colors.ContainsKey(viz.Name)) continue;

            // Set colors
            var colors = Settings.Defaults.Colors[viz.Name];
            cmd.Parameters.Add(new SqliteParameter("Colors", ColorScheme.FormatColors(colors)));

            // Execute insert statement
            cmd.ExecuteNonQuery();

            // Dispose command
            cmd.Dispose();
        }
    }

	/// <summary>
	///     Inserts the default playlist into the database.
	/// </summary>
	private static void InsertDefaultPlaylist()
    {
        if (GetConnection() == null) return;

        // Database command
        var cmd = new SqliteCommand(Connection);

        // Query statement
        var sql = "SELECT id FROM playlist LIMIT 1";
        cmd.CommandText = sql;

        // Get sql results
        var reader = cmd.ExecuteReader();


        if (!reader.HasRows)
        {
            // Dispose command
            cmd.Dispose();

            // Create default playlist
            sql = "INSERT INTO playlist (name) VALUES(@Name)";
            cmd = new SqliteCommand(sql, Connection);

            // Add parameters
            cmd.Parameters.Add(new SqliteParameter("Name", "Playlist"));

            // Send query
            cmd.ExecuteNonQuery();
        }


        // Close reader
        reader.Close();
        cmd.Dispose();
    }

	/// <summary>
	///     Gets the connection if class has already been instantiated. If not, tries to
	///     connect to database and inserts all tables and default datasets.
	/// </summary>
	/// <returns>The database connection.</returns>
	private static SqliteConnection GetConnection()
    {
        if (_instance != null) return Connection;

        // Create instance if not exists.
        _instance = new Database();

        CreateTables();
        InsertVisualizations();
        InsertDefaultColorScheme();
        InsertDefaultPlaylist();

        return Connection;
    }

	/// <summary>
	///     Connect to database. Needed to instantiate <see cref="Database" /> class.
	/// </summary>
	/// <returns>
	///     <c>true</c> if the connection was established; otherwise, <c>false</c>.
	/// </returns>
	public static bool Connect()
    {
        if (Connection == null) Connection = GetConnection();

        return Connection != null;
    }
}