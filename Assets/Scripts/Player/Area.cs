﻿using UnityEngine;
using System.Collections;

public class Area : MonoBehaviour {

	public PlayerCanvas canvas;
	public bool onLeft = true;
	public bool onBottom = true;

	private RectTransform trans;
	private Vector2 size;
	private Vector2 rangeX;
	private Vector2 rangeY;



	void Start ()
	{
		// Get transformation
		trans = GetComponent<RectTransform> ();

		// Get size
		size = trans.sizeDelta;
		if (size.x == 0) size.x = Camera.main.pixelWidth;
		if (size.y == 0) size.y = Camera.main.pixelHeight;

		// Set x and y range
		// IMPORTANT: mouse (0,0) is bottom-left
		rangeX = new Vector2 ();
		rangeY = new Vector2 ();

		if (onLeft) {
			rangeX.x = 0;
			rangeX.y = size.x;
		} else {
			rangeX.x = Camera.main.pixelWidth - size.x;
			rangeX.y = Camera.main.pixelWidth;
		}

		if (onBottom) {
			rangeY.x = 0;
			rangeY.y = size.y;
		} else {
			rangeY.x = Camera.main.pixelHeight - size.y;
			rangeY.y = Camera.main.pixelHeight;
		}
	}

	void Update ()
	{
		if ((Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0)
			&& Input.mousePosition.x >= rangeX.x && Input.mousePosition.x <= rangeX.y
			&& Input.mousePosition.y >= rangeY.x && Input.mousePosition.y <= rangeY.y)
		{
			canvas.ShowPlayer ();
		}
	}
}