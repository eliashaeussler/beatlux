/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

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
