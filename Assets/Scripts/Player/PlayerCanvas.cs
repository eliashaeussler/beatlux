using UnityEngine;
using System.Collections;
using System.Timers;

public class PlayerCanvas : MonoBehaviour {

	public GameObject wrapper;
	public float displayLength = 3;

	private CameraController ctrl;
	private Coroutine timer;
	private bool canShow;



	void Start ()
	{
		// Get Camera Controller reference
		ctrl = Camera.main.transform.parent.GetComponent<CameraController> ();

		// Show player at the beginning
		canShow = true;
		ShowPlayer ();
	}

	void Update ()
	{
		// Update Camera Controller reference
		ctrl = Camera.main.transform.parent.GetComponent<CameraController> ();

		if (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0)
		{
			canShow = true;
			Cursor.visible = true;
		}
	}



	public void ShowPlayer ()
	{
		if (canShow)
		{
			// TODO fade in wrapper

			// Show player wrapper
			wrapper.SetActive (true);

			// Disable camera moving
			if (ctrl != null) ctrl.permitmove = false;

			// Fade out after x seconds
			KeepPlayer ();
		}
	}

	public void KeepPlayer ()
	{
		if (wrapper.activeSelf)
		{
			if (timer != null) StopCoroutine (timer);
			timer = StartCoroutine (HidePlayer ());
		}
	}

	public IEnumerator HidePlayer ()
	{
		// Wait for x seconds
		yield return new WaitForSeconds (displayLength);

		// TODO fade out wrapper

		HidePlayerImmediate ();
	}

	public void HidePlayerImmediate ()
	{
		// Hide player
		wrapper.SetActive (false);

		// Disable camera moving
		if (ctrl != null) ctrl.permitmove = true;

		// Do not show player again if mouse does not move
		canShow = false;

		// Hide cursor
		Cursor.visible = false;
	}
}
