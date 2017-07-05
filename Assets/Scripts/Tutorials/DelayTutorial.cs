using UnityEngine;
using System.Collections;

public class DelayTutorial : MonoBehaviour {

    private bool Temp;
    

    void Awake()
    {
        if (Settings.Player.TutorialTog == true)
        {
            GameObject.Find("FocusShape").SetActive(true);
            GameObject.Find("TutorialManager").SetActive(true);
            Temp = true;
        }
        else
        {
            GameObject.Find("FocusShape").SetActive(false);
            GameObject.Find("TutorialManager").SetActive(false);
            Temp = false;
        }
    }

    void Update()
    {
        if (Settings.Player.TutorialTog != Temp)
        {
            GameObject.Find("FocusShape").SetActive(Temp);
            GameObject.Find("TutorialManager").SetActive(Temp);
            Temp = Settings.Player.TutorialTog;
        }
        
    }
}
