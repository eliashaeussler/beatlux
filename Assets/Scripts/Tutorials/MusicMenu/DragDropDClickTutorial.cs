using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragDropDClickTutorial : Tutorial {

    public override void CheckIfHappening()
    {
        if (GameObject.Find("Contents")!= null)
        {
            TutorialManager.Instace.CompletedTutorial();
        }
    }
}
