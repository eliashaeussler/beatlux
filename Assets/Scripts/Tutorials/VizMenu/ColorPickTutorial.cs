/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class ColorPickTutorial : Tutorial2
{
    public override void CheckIfHappening()
    {
        if (GameObject.Find("ColorPicker").activeSelf) TutorialManager2.Instace.CompletedTutorial();
    }
}