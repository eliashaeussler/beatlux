﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class MenuTutorial : Tutorial3 {

    public GameObject TextIcons;

    public override void CheckIfHappening()
    {
        TextIcons.SetActive(true);
        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Settings"))
        {
            TextIcons.SetActive(false);
            TutorialManager3.Instace.CompletedTutorial();
        }
    }
}
