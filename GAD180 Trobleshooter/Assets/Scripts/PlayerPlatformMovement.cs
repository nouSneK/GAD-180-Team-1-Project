using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformMovement : MonoBehaviour
{
    public bool control;

    public float movementSpeed = 0.1f;

    private Transform cam;

    private void Start()
    {
        cam = gameObject.GetComponent<PlayerMovement>().playerCam;
    }

    private void FixedUpdate()
    {
        if (control)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(cam.forward * movementSpeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-cam.forward * movementSpeed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-cam.right * movementSpeed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(cam.right * movementSpeed);
            }
        }
    }
}
