using UnityEngine;
using System.Collections;
using System.Timers;

public class PlayerCanvas : MonoBehaviour {

	public GameObject wrapper;
	public int displayLength = 7;
	private Coroutine timer;



	// Use this for initialization
	void Start ()
	{
		ShowPlayer ();
	}

	void Update ()
	{
		// Show player if mouse moves
		if (Input.GetAxis ("Mouse X") != 0) {
			ShowPlayer ();
		}
	}

	public void ShowPlayer ()
	{
		// TODO fade in wrapper

		// Show player wrapper
		wrapper.SetActive (true);

		// Fade out after x seconds
		if (timer != null) StopCoroutine (timer);
		timer = StartCoroutine (HidePlayer ());
	}

	public IEnumerator HidePlayer ()
	{
		// Wait for x seconds
		yield return new WaitForSeconds (displayLength);

		// TODO fade out wrapper

		// Hide player
		wrapper.SetActive (false);
	}
}
