/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    private bool _canShow;

    private CameraController _ctrl;
    private Coroutine _timer;
    public float DisplayLength = 3;

    public GameObject Wrapper;


    private void Start()
    {
        // Get Camera Controller reference
        _ctrl = Camera.main.GetComponent<CameraController>();

        // Show player at the beginning
        _canShow = true;
        ShowPlayer();
    }

    private void Update()
    {
        // Update Camera Controller reference
        _ctrl = Camera.main.GetComponent<CameraController>();

        if (Math.Abs(Input.GetAxis("Mouse X")) < 1 && Math.Abs(Input.GetAxis("Mouse Y")) < 1) return;

        _canShow = true;
        Cursor.visible = true;
    }


    public void ShowPlayer()
    {
//		if (canShow)
//		{
        // Show player wrapper
        Wrapper.SetActive(true);

        // Disable camera moving
        if (_ctrl != null) _ctrl.Permitmove = false;

//			// Fade out after x seconds
//			KeepPlayer ();
//		}
    }

    /*public void KeepPlayer ()
    {
        if (wrapper.activeSelf)
        {
            if (timer != null) StopCoroutine (timer);
            timer = StartCoroutine (HidePlayer ());
        }
    }*/

    /*public IEnumerator HidePlayer ()
    {
        // Wait for x seconds
        yield return new WaitForSeconds (displayLength);
        HidePlayerImmediate ();
    }*/

    public void HidePlayerImmediate()
    {
        // Hide player
        Wrapper.SetActive(false);

        // Disable camera moving
        if (_ctrl != null) _ctrl.Permitmove = true;

        // Do not show player again if mouse does not move
        _canShow = false;

        // Hide cursor
        Cursor.visible = false;
    }
}