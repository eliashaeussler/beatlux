/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

[RequireComponent(typeof(CameraMover))]
public class CameraController : MonoBehaviour
{
    private CameraMover _move;
    private PlayerCanvas _player;
    public float MouseSensitivity = 3f;
    public bool Permitmove;
    public float Speed = 10f;

    /**
     * Initialize the mover
     **/
    private void Start()
    {
        _move = GetComponent<CameraMover>();
        GameObject canvas;
        if ((canvas = GameObject.Find("PlayerCanvas")) != null) _player = canvas.GetComponent<PlayerCanvas>();
    }

    /**
     * Get inputs from the player, tells the CameraMover what to do.
     **/
    private void Update()
    {
        var xMov = Input.GetAxisRaw("Horizontal");


        var zMov = Input.GetAxisRaw("Vertical");

        //Increase speed when pressing left shift
        if (Input.GetKeyDown(KeyCode.LeftShift))
            if (Speed < 50)
                Speed += 5f;

        //Decrease speed when pressing left shift
        if (Input.GetKeyDown(KeyCode.RightShift))
            if (Speed > 0)
                Speed -= 5f;

        //Enable/disable movement when pressing shift
        if (Input.GetKeyDown("space"))
        {
            Permitmove = !Permitmove;

            if (Permitmove)
                _player.HidePlayerImmediate();
            else
                _player.ShowPlayer();
        }

        //Resets player position to standard when pressing R
        if (Input.GetKeyDown(KeyCode.R)) _move.resPosition();

        //Get basic movement inputs from WASD and Mouse
        var movHorizontal = transform.right * xMov;
        var movVertical = transform.forward * zMov;

        var velocity = (movHorizontal + movVertical).normalized * Speed;
        _move.Move(velocity);

        var yRot = Input.GetAxisRaw("Mouse X");
        var xRot = Input.GetAxisRaw("Mouse Y");


        var rotation = new Vector3(-xRot, yRot, 0f) * MouseSensitivity;

        _move.Permission(Permitmove);

        _move.Rotate(rotation);
    }
}