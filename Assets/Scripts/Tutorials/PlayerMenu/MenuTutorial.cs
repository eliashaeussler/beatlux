using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuTutorial : Tutorial3 {

    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Settings"))
        {
            TutorialManager3.Instace.CompletedTutorial();
        }
    }
}
