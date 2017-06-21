using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraMover))]
public class CameraController : MonoBehaviour {
    public float speed = 10f;
    public float mouseSensitivity = 3f;
    public bool permitmove = false;

    private CameraMover move;
	private PlayerCanvas player;

    void Start()
    {
        
        move = GetComponent<CameraMover>();
		player = GameObject.Find ("PlayerCanvas").GetComponent<PlayerCanvas> ();
    }

    void Update()
	{
        float xMov = Input.GetAxisRaw("Horizontal");


        float zMov = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (speed < 50)
            {
                speed += 5f;
            }
        }

        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            if (speed > 0)
            {
                speed -= 5f;
            }
        }

        if (Input.GetKeyDown("space"))
        {
            permitmove = !permitmove;

			if (permitmove) {
				player.HidePlayerImmediate ();
			} else {
				player.ShowPlayer ();
			}
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            move.resPosition();
        }

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
