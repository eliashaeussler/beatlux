using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NewTapTutorial : Tutorial {

    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Visualisierung"))
        {
            TutorialManager.Instace.CompletedTutorial();
        }
    }
}
