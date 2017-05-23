using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	// Player canvas
	public PlayerCanvas canvas;

	void Start ()
	{
		canvas.KeepPlayer ();
	}

	void Update ()
	{
		// Show player if mouse moves
		if (Input.mousePosition.y <= transform.position.y && (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0)) {

			canvas.KeepPlayer ();
		}
	}
}
