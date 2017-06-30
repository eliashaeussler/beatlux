using UnityEngine;
using System.Collections;

public class DelayTutorial : MonoBehaviour {

    void Start()
    {
        GameObject.Find("TutorialManager3").SetActive(false);
        GameObject.Find("TutorialManager3").SetActive(true);
    }
}
