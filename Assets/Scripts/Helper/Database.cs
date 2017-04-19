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

	// Class instance
	private static Database instance;

	// Database name
	private static string name = "beatlux";

	// Database extension
	private static string ext = "db";

	// Database connection
	static SqliteConnection con;

	// Enums for error handling
	public enum Constants : long
	{
		QueryFailed = -1,
		DuplicateFound = -2
	}



	// Object initialization
	private Database ()
	{
		// Path to database
		string uri = "Data Source=" + Application.dataPath + "/" + name + "." + ext;

		// Connect to database
		con = Connect (uri);
	}

	// Connect to dataabse
	private SqliteConnection Connect (string uri)
	{
		// Connect to database
		con = new SqliteConnection (uri);

		// Open database connection
		con.Open();

		return con;
	}

	// Close database connection
	public static void Close ()
	{
		// Close connection to database
		if (con != null) {
			con.Close ();
		}

		// Reset instance
		instance = null;
	}

	public static void CreateTables ()
	{
		// SQL statements
		string[] stm = {
			"CREATE TABLE IF NOT EXISTS \"file\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `path` TEXT NOT NULL UNIQUE )",
			"CREATE TABLE IF NOT EXISTS \"playlist\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `files` TEXT DEFAULT NULL )",
			"CREATE TABLE IF NOT EXISTS \"visualization\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL UNIQUE, `buildNumber` INTEGER UNIQUE )",
			"CREATE TABLE IF NOT EXISTS \"color_scheme\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `viz_id` INTEGER, `colors` TEXT, FOREIGN KEY(`viz_id`) REFERENCES `visualization`(`id`) )",
		};

		// Create tables
		if (GetConnection () != null)
		{
			foreach (string sql in stm)
			{
				SqliteCommand cmd = new SqliteCommand (sql, con);
				cmd.ExecuteNonQuery ();
				cmd.Dispose ();
			}
		}
	}



	// Get instance (Singleton)
	public static SqliteConnection GetConnection ()
	{
		// Create instance if not exists
		if (instance == null) {
			instance = new Database ();
			CreateTables ();
		}

		// Return database connection
		return con;
	}
}
