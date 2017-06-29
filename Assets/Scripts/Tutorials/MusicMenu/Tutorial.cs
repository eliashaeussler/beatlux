using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour {
    public int Order;
    [TextArea(6, 15)]
    public string Explanation;

	void Awake () {
        TutorialManager.Instace.Tutorials.Add(this);
	}
    
    public virtual void CheckIfHappening() { }
}
