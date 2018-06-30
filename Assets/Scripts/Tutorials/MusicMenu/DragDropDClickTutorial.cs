/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class DragDropDClickTutorial : Tutorial
{
    public override void CheckIfHappening()
    {
        if (GameObject.Find("Contents") != null) TutorialManager.Instace.CompletedTutorial();
    }
}