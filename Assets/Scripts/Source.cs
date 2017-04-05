using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Source : MonoBehaviour {

    // Use this for initialization
    Text sourceText;

    void Start () {

      
    }
	
	// Update is called once per frame
	void Update () {
        sourceText = GetComponent<Text>();
        sourceText.text = SourceFolder.sPath.pathAll;

    }

}
