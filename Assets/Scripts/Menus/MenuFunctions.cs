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

	public static List<String> searchResults;
    public static string pathF;

    public void getInput(string s)
    {
        if (s == "")
        {
            pathF = @SourceFolder.mainPath;
        }
        else
        {
			// Reset results
			searchResults = new List<String> ();

			// Get results
			searchResults.AddRange ( Directory.GetFiles (@SourceFolder.currentPath, "*" + s + "*", SearchOption.AllDirectories) );
			searchResults.AddRange ( Directory.GetDirectories (@SourceFolder.currentPath, "*" + s + "*", SearchOption.AllDirectories) );
			//			searchResults.Sort ();
        }
    } 
}