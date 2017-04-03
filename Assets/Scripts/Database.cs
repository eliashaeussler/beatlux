using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

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
	public enum Constants : int
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



	// Get instance (Singleton)
	public static SqliteConnection GetConnection ()
	{
		// Create instance if not exists
		if (instance == null) {
			instance = new Database ();
		}

		// Return database connection
		return con;
	}
}
