using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;

/// <summary>
/// Database class offers connection to the local database. It should be
/// only one connection opened which is realized through the use of the
/// Singleton pattern.
/// 
/// == Usage ==
/// SqliteConnection db = Database.getConnection ();
/// SqliteCommand cmd = new SqliteCommand (db);
/// // and so on ...
/// </summary>
public class Database {

	// Database name
	private static string NAME = "beatlux.db";

	// Class instance
	private static Database Instance;

	// Database connection
	public static SqliteConnection Connection;



	// Enums for error handling
	public enum Constants : long
	{
		Successful = -1,

		QueryFailed = -10,
		DuplicateFound = -11,
		EmptyInputValue = -12
	}



	// Object initialization
	private Database ()
	{
		// Path to database
		string uri = "Data Source=" + Application.dataPath + "/" + NAME;

		// Connect to database
		Connection = Connect (uri);
	}

	// Connect to dataabse
	private SqliteConnection Connect (string uri)
	{
		// Connect to database
		Connection = new SqliteConnection (uri);

		// Open database connection
		Connection.Open ();

		return Connection;
	}

	// Close database connection
	public static void Close ()
	{
		// Close connection to database
		if (Connection != null) {
			Connection.Close ();
		}

		// Reset instance
		Instance = null;

		// Reset database instance
		Connection = null;
	}

	public static void CreateTables ()
	{
		// SQL statements
		string[] stm = {
			"CREATE TABLE IF NOT EXISTS `file` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `path` TEXT NOT NULL UNIQUE )",
			"CREATE TABLE IF NOT EXISTS `playlist` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `files` TEXT DEFAULT NULL )",
			"CREATE TABLE IF NOT EXISTS `visualization` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `colors` INTEGER NOT NULL DEFAULT 1, `buildNumber` INTEGER UNIQUE )",
			"CREATE TABLE IF NOT EXISTS `color_scheme` ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `viz_id` INTEGER, `colors` TEXT NOT NULL, FOREIGN KEY(`viz_id`) REFERENCES `visualization`(`id`) )",
		};

		// Create tables
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



	// Get instance (Singleton)
	public static SqliteConnection GetConnection ()
	{
		// Create instance if not exists
		if (Instance == null) {
			Instance = new Database ();
			CreateTables ();
		}

		// Return database connection
		return Connection;
	}



	public static bool Connect ()
	{
		if (Connection == null) {
			Connection = Database.GetConnection ();
		}

		return Connection != null;
	}
}
