/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class VizPlayerTutorial : Tutorial3
{
    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Logo"))
            TutorialManager3.Instace.CompletedTutorial();
    }
}