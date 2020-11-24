using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotVisionCollision : MonoBehaviour
{
    public GameObject robot;

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerHealth>())
        {
            robot.GetComponent<RobotAI>().LookForPlayer();
        }
    }
}
