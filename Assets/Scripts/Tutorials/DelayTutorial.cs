using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayTutorial : MonoBehaviour {

    private bool Temp;
    private List<GameObject> list = new List<GameObject>();

    void Awake()
    {
        list.Add(GameObject.Find("FocusShape"));
        list.Add(GameObject.Find("TutorialManager"));
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
            list[0].SetActive(Settings.Player.TutorialTog);
            list[1].SetActive(Settings.Player.TutorialTog);
            Temp = Settings.Player.TutorialTog;
        }
        
    }
}
