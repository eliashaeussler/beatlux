using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraMover))]
public class CameraController : MonoBehaviour {
    public float speed = 10f;
    public float mouseSensitivity = 3f;
    public bool permitmove = false;

    private CameraMover move;
	private PlayerCanvas player;

    /**
     * Initialize the mover
     **/
    void Start()
    {
        
        move = GetComponent<CameraMover>();
		player = GameObject.Find ("PlayerCanvas").GetComponent<PlayerCanvas> ();
    }

    /**
     * Get inputs from the player, tells the CameraMover what to do.
     **/
    void Update()
	{
        float xMov = Input.GetAxisRaw("Horizontal");


        float zMov = Input.GetAxisRaw("Vertical");

        //Increase speed when pressing left shift
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (speed < 50)
            {
                speed += 5f;
            }
        }

        //Decrease speed when pressing left shift
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (speed > 0)
            {
                speed -= 5f;
            }
        }

        //Enable/disable movement when pressing shift
        if (Input.GetKeyDown("space"))
        {
            permitmove = !permitmove;

			if (permitmove) {
				player.HidePlayerImmediate ();
			} else {
				player.ShowPlayer ();
			}
        }

        //Resets player position to standard when pressing R
        if (Input.GetKeyDown(KeyCode.R))
        {
            move.resPosition();
        }

        //Get basic movement inputs from WASD and Mouse
        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;
        move.Move(velocity);

        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");


        Vector3 rotation = new Vector3(-xRot, yRot, 0f) * mouseSensitivity;

        move.Permission(permitmove);

        move.Rotate(rotation);
    }
}
