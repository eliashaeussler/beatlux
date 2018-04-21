/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class NameSchemeTutorial : Tutorial2 {

    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("InputField Input Caret"))
        {
            TutorialManager2.Instace.CompletedTutorial();
        }
    }
}
