/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections.Generic;
using UnityEngine;

public class KeyTutorial : Tutorial3
{
    public List<string> Keys = new List<string>();

    public override void CheckIfHappening()
    {
        for (var i = 0; i < Keys.Count; i++)
        {
            if (!Input.inputString.Contains(Keys[i])) continue;

            Keys.RemoveAt(i);
            break;
        }

        if (Keys.Count == 0) TutorialManager3.Instace.CompletedTutorial();
    }
}