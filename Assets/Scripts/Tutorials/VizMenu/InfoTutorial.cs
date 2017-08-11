using UnityEngine;
using System.Collections;

public class InfoTutorial : Tutorial2 {

    public override void CheckIfHappening()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TutorialManager2.Instace.CompletedTutorial();
        }
    }
}
