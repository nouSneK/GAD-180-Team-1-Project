using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMeleeCollisionBox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerHealth>())
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
