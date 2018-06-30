/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class NamePlaylistTutorial : Tutorial
{
    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("InputField Input Caret"))
            TutorialManager.Instace.CompletedTutorial();
    }
}