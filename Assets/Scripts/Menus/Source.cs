/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class Source : MonoBehaviour {

	public Transform text;

	private string oldSource;



	void Update ()
	{
		if (oldSource != Settings.Source.Current)
		{
			// Remove all GameObjects
			for (int i=text.transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyImmediate (text.transform.GetChild (i).gameObject);
			}

			// Get current source
			string [] dirs = Settings.Source.Current
				.Replace ('\\', '/')
				.Trim (new char [] { '/' })
				.Split (new char [] { '/' });

			for (int i=0; i < dirs.Length; i++)
			{
				// Create GameObject
				GameObject go = new GameObject (dirs [i]);
				go.transform.SetParent (text.transform);

				// Add text
				Text goText = go.AddComponent<Text> ();
				goText.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
				goText.fontSize = 20;
				goText.color = new Color (1, 1, 1, 0.5f);
				goText.text = dirs [i];

				// Set RectTransform
				RectTransform trans = go.GetComponent<RectTransform> ();
				trans.localScale = Vector3.one;

				// Add arrow
				if (i < dirs.Length - 1)
				{
					GameObject arrow = new GameObject ("Arrow");
					arrow.transform.SetParent (text.transform);

					TextUnicode arrowText = arrow.AddComponent<TextUnicode> ();
					arrowText.font = IconFont.font;
					arrowText.fontSize = 15;
					arrowText.color = new Color (1, 1, 1, 0.5f);
					arrowText.alignment = TextAnchor.MiddleCenter;
					arrowText.text = IconFont.DROPDOWN_CLOSED;

					RectTransform arrowTrans = arrow.GetComponent<RectTransform> ();
					arrowTrans.localScale = Vector3.one;
				}
			}

			// Set old source
			oldSource = Settings.Source.Current;
		}
	}

}
