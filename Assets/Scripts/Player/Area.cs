/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using UnityEngine;

public class Area : MonoBehaviour
{
    private Vector2 _rangeX;
    private Vector2 _rangeY;
    private Vector2 _size;

    private RectTransform _trans;

    public PlayerCanvas Canvas;
    public bool OnBottom = true;
    public bool OnLeft = true;


    private void Start()
    {
        // Get transformation
        _trans = GetComponent<RectTransform>();

        // Get size
        GetRealSize();

        // Set x and y range
        GetRange();
    }

    private void Update()
    {
        // Get x and y range
        GetRange();

        // Check if mouse is in range
        if ((Math.Abs(Input.GetAxis("Mouse X")) > 0 || Math.Abs(Input.GetAxis("Mouse Y")) > 0)
            && Input.mousePosition.x >= _rangeX.x && Input.mousePosition.x <= _rangeX.y
            && Input.mousePosition.y >= _rangeY.x && Input.mousePosition.y <= _rangeY.y)
        {
//			canvas.ShowPlayer ();
        }
    }

    private void GetRealSize()
    {
        // Get scale
        _size = _trans.sizeDelta;

        // Change with current scale
        _size.x *= Canvas.transform.localScale.x;
        _size.y *= Canvas.transform.localScale.y;

        // Change if area is completely stretched
        if (Math.Abs(_size.x) <= 0) _size.x = Camera.main.pixelWidth;
        if (Math.Abs(_size.y) <= 0) _size.y = Camera.main.pixelHeight;
    }

    private void GetRange()
    {
        // IMPORTANT: mouse (0,0) is bottom-left
        _rangeX = new Vector2();
        _rangeY = new Vector2();

        if (OnLeft)
        {
            _rangeX.x = 0;
            _rangeX.y = _size.x;
        }
        else
        {
            _rangeX.x = Camera.main.pixelWidth - _size.x;
            _rangeX.y = Camera.main.pixelWidth;
        }

        if (OnBottom)
        {
            _rangeY.x = 0;
            _rangeY.y = _size.y;
        }
        else
        {
            _rangeY.x = Camera.main.pixelHeight - _size.y;
            _rangeY.y = Camera.main.pixelHeight;
        }
    }
}