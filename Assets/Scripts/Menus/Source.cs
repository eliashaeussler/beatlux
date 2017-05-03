using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Source : MonoBehaviour {

    public GameObject source;
	Text text;

    void Start ()
	{
		text = source.GetComponent<Text> ();
		text.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
		text.fontSize = 20;
    }

	void Update ()
	{
        text.text = Settings.Source.Current;
	}

}
