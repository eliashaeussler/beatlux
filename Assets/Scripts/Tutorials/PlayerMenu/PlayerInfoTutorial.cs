using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerInfoTutorial : Tutorial3 {

	public override void CheckIfHappening() {

        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Controls_Left").transform.FindChild("VolumeSlider").gameObject)
        {
            TutorialManager3.Instace.CompletedTutorial();
        }
    }
}
