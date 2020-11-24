using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeCollision : MonoBehaviour
{
    public int meleeDamage = 1;
    public int numberOfMeleeHits = 1;
    public int currentNumberOfMeleeHits;

    private void OnTriggerEnter(Collider other)
    {
        if (currentNumberOfMeleeHits < numberOfMeleeHits)
        {
            if (other.GetComponent<RobotCollisionBox>())
            {
                other.GetComponent<RobotCollisionBox>().robotParent.GetComponent<RobotAI>().TakeDamage(meleeDamage);
                other.GetComponent<RobotCollisionBox>().BreakOff();

                currentNumberOfMeleeHits++;
            }
        }

        if (other.gameObject.GetComponent<VentOpening>())
        {
            other.gameObject.GetComponent<VentOpening>().Open(gameObject);
        }
    }
}
