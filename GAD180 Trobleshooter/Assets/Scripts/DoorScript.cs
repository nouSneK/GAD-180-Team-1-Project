using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private bool opened;
    public bool searchForRobts;

    public float openDistance = 2.5f;

    public GameObject door;

    public List<GameObject> closeObjects;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerHealth>())
        {
            Open();
        }
    }

    private void Update()
    {
        if(!opened && searchForRobts)
        {
            foreach (GameObject robot in closeObjects)
            {
                if(Vector3.Distance(transform.position, robot.transform.position) < openDistance)
                {
                    Open();
                }
            }
        }
    }

    void Open()
    {
        if (!opened)
        {
            opened = true;

            door.GetComponent<Animator>().SetBool("DoorOpen", true);
        }
    }
}
