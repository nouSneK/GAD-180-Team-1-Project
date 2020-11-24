using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStickToPlatform : MonoBehaviour
{
    public bool canStick = true;

    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player && canStick)
        {
            player.transform.parent = transform;

            player.GetComponent<PlayerPlatformMovement>().control = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject == player && player.transform.parent == transform)
        {
            player.transform.parent = null;

            player.GetComponent<PlayerPlatformMovement>().control = false;
        }
    }
}
