/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraMover : MonoBehaviour
{
    private bool _permitmove;

    private Rigidbody _rb;
    private bool _resetPosition;
    private Vector3 _rotation = Vector3.zero;
    private Vector3 _spawnPosition = Vector3.zero;
    private Quaternion _spawnRotation;


    private Vector3 _velocity = Vector3.zero;


    /**
     * Set camera in the right position
     **/
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _spawnPosition = _rb.position;
        _spawnRotation = _rb.rotation;
    }

    /**
     * Actual movement of the camera
     **/
    private void FixedUpdate()
    {
        if (_resetPosition)
        {
            _resetPosition = false;
            _rb.position = _spawnPosition;
            _rb.rotation = _spawnRotation;
        }

        if (!_permitmove) return;

        PerformMovement();
        PerformRotation();
    }

    /**
     * Prevent z-movement of camera
     **/
    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
    }


    /**
     * Handover of inputs from controller
     **/
    public void Move(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void Rotate(Vector3 rotation)
    {
        _rotation = rotation;
    }

    public void resPosition()
    {
        _resetPosition = true;
    }

    public void Permission(bool permitmove)
    {
        _permitmove = permitmove;
    }


    /**
     * Camera movement
     **/
    private void PerformMovement()
    {
        if (_velocity != Vector3.zero) _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    /**
     * Camera Rotation
     **/
    private void PerformRotation()
    {
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(_rotation));
    }
}