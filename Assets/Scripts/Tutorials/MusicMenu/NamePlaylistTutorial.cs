using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NamePlaylistTutorial : Tutorial {

    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("InputField Input Caret"))
        {
            TutorialManager.Instace.CompletedTutorial();
        }
    }
}
