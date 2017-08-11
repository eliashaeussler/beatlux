using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class ColorPickTutorial : Tutorial2 {

    public override void CheckIfHappening()
    {
        if (GameObject.Find("ColorPicker").activeSelf == true)
        {
            TutorialManager2.Instace.CompletedTutorial();
        }
    }
}
