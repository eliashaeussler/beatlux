using UnityEngine;
using System.Collections;
using System.Timers;

public class PlayerCanvas : MonoBehaviour {

	public GameObject wrapper;
	public int displayLength = 4;

	private Coroutine timer;
	private bool canShow;



	void Start ()
	{
		// Show player at the beginning
		ShowPlayer ();
	}

	void Update ()
	{
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

		// Hide player
		wrapper.SetActive (false);

		// Do not show player again if mouse does not move
		canShow = false;

		// Hide cursor
		Cursor.visible = false;
	}
}
