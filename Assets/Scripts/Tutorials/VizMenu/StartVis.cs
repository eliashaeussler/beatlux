/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class StartVis : Tutorial2
{
    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Start"))
            TutorialManager2.Instace.CompletedTutorial();
    }
}