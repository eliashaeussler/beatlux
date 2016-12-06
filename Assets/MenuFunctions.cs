using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
}
