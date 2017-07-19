using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayTutorial : MonoBehaviour {

    private bool Temp;
    private List<GameObject> list = new List<GameObject>();

    void Awake()
    {
        list.Add(GameObject.Find("FocusShape2"));
        list.Add(GameObject.Find("TutorialManager"));
        if (Settings.Player.TutorialTog == true)
        {
            GameObject.Find("FocusShape2").SetActive(true);
            GameObject.Find("TutorialManager").SetActive(true);
            Temp = true;
        }
        else
        {
            if (GameObject.Find("FocusShape2")||GameObject.Find("TutorialManager"))
            {
                GameObject.Find("FocusShape2").SetActive(false);
                GameObject.Find("TutorialManager").SetActive(false);
                Temp = false;

            }
            
        }
    }

    void Update()
    {
        if (Settings.Player.TutorialTog != Temp)
        {
            list[0].SetActive(Settings.Player.TutorialTog);
            list[1].SetActive(Settings.Player.TutorialTog);
            if (list[1].GetComponent<TutorialManager3>()&&Temp==true)
            {
                list[1].GetComponent<TutorialManager3>().Init();
            }
            Temp = Settings.Player.TutorialTog;
        }
        
    }
}
