using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public Transform teleportPoint;

    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == player.name)
        {
            player.GetComponent<Rigidbody>().MovePosition(teleportPoint.position);
        }
    }
}
