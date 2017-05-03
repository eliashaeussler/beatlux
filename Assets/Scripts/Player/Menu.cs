using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public PlayerCanvas canvas;

	void Update ()
	{
		if (Input.mousePosition.y >= transform.position.y && (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0))
		{
			canvas.KeepPlayer ();
		}
	}
}
