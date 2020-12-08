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
            if (robot.GetComponent<RobotAI>())
            {
                robot.GetComponent<RobotAI>().LookForPlayer();
            }
            else if (robot.GetComponent<Boss1Robot>())
            {
                robot.GetComponent<Boss1Robot>().LookForPlayer();
            }
        }
    }
}
