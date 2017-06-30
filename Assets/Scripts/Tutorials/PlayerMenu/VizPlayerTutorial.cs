using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class VizPlayerTutorial : Tutorial3 {

    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Logo"))
        {
            TutorialManager3.Instace.CompletedTutorial();
        }
    }
}
