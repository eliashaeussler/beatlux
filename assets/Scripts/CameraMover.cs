using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class CameraMover : MonoBehaviour {

    private Rigidbody rb;
    private Vector3 spawnPosition = Vector3.zero;
    private Quaternion spawnRotation; 
    

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private bool permitmove = true;
    private bool resetPosition = false;
   

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawnPosition = rb.position;
        spawnRotation = rb.rotation;
    }

    void FixedUpdate()
    {
        if (resetPosition)
        {
            resetPosition = false;
            rb.position = spawnPosition;
            rb.rotation = spawnRotation;
        }

        if (permitmove)
        {
            PerformMovement();
            PerformRotation();
        }
       
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void resPosition()
    {
        resetPosition = true;
    }

    public void Permission(bool _permitmove)
    {
        permitmove = _permitmove;

    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        
    }
}

