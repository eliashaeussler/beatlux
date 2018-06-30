/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public PlayerCanvas Canvas;

    private void Update()
    {
        if (Input.mousePosition.y >= transform.position.y &&
            (Math.Abs(Input.GetAxis("Mouse X")) > 0 || Math.Abs(Input.GetAxis("Mouse Y")) > 0))
        {
//			Canvas.KeepPlayer ();
        }
    }
}