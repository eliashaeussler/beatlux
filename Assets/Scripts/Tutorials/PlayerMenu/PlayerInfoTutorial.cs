/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInfoTutorial : Tutorial3
{
    public override void CheckIfHappening()
    {
        if (EventSystem.current.currentSelectedGameObject ==
            GameObject.Find("Controls_Left").transform.Find("VolumeSlider").gameObject)
            TutorialManager3.Instace.CompletedTutorial();
    }
}