/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Player canvas
    public PlayerCanvas Canvas;

    private void Start()
    {
//		canvas.KeepPlayer ();
    }

    private void Update()
    {
        // Show player if mouse moves
        if (Input.mousePosition.y <= transform.position.y &&
            (Math.Abs(Input.GetAxis("Mouse X")) > 0 || Math.Abs(Input.GetAxis("Mouse Y")) > 0))
        {
//			canvas.KeepPlayer ();
        }
    }
}