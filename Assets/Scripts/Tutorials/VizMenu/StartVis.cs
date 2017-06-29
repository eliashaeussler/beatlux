using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StartVis : Tutorial2
{

    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Start"))
        {
            TutorialManager2.Instace.CompletedTutorial();
        }
    }
}
