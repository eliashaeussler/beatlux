/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tutorial3 : MonoBehaviour {

    public int Order;
    [TextArea(6, 15)]
    public string Explanation;

    // called on awake to add tutorials to tutorialmanager
    void Awake()
    {
        TutorialManager3.Instace.Tutorials.Add(this);
    }

    public virtual void CheckIfHappening() { }
}
