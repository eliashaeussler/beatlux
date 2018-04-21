/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;


public class ShowTextTutorial : Tutorial
{
    public override void CheckIfHappening() {
        if (Input.GetMouseButtonDown(1))
        {
            TutorialManager.Instace.CompletedTutorial();
        }
    }
}

