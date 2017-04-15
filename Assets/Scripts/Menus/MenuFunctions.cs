using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class MenuFunctions : MonoBehaviour {
    
	public void startLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    /**
    If started through Unity Editor, end play
        If Started through Build Application, Quit Application
    **/
    public void end()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Quitting Editor");
#else
        Debug.Log("Quitting App");
        Application.Quit();
#endif
        
    }

	public static List<String> searchDirs;
	public static List<String> searchFiles;
    public static string pathF;

    public void GetInput(string s)
    {
        if (s == "")
        {
            pathF = @SourceFolder.mainPath;
        }
        else
        {
			// Reset results
			searchDirs = new List<String> ();
			searchFiles = new List<String> ();

			// Get results
			string path = SourceFolder.currentPath;
			string pattern = "*" + s + "*";
			GetResults (path, pattern);

			// Remove hidden files and folders
			searchDirs = SourceFolder.RemoveHidden (searchDirs);
			searchFiles = SourceFolder.RemoveHidden (searchFiles);
        }
    }

	private void GetResults (string folder, string pattern)
	{
		foreach (string file in Directory.GetFiles (folder, pattern))
		{
			searchFiles.Add (file);
		}
		foreach (string subDir in Directory.GetDirectories (folder))
		{
			try {
				//searchDirs.Add (subDir);
				GetResults (subDir, pattern);
			} catch {
				print (subDir);
			}
		}
	}
}