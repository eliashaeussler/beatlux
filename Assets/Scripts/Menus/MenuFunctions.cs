using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

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

    string sourcePath;
    public static string[] searchResults;
    public static string pathF;

    public void getInput(string s)
    {
        if (s == "")
        {
            pathF = @SourceFolder.sPath.pathFinal;
        }
        else
        {
            searchResults = Directory.GetFiles(@SourceFolder.sPath.pathFinal, "*" + s + "*", SearchOption.AllDirectories);
        }


    } 
}
