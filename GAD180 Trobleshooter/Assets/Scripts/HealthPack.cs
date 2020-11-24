using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int healthAmount = 4;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<PlayerHealth>())
        {
            collision.collider.GetComponent<PlayerHealth>().AddHealth(healthAmount);

            Destroy(gameObject);
        }
    }
}
