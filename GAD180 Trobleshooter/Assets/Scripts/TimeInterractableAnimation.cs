using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeInterractableAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void OrbHit(int selectedOrb, float multiplier)
    {
        if(selectedOrb == 0)
        {
            animator.speed = 1 * multiplier;

            Debug.Log("Orb Hit animation * " + multiplier);
        }
        else if(selectedOrb == 1)
        {
            animator.speed = 1 / multiplier;

            Debug.Log("Orb Hit animation / " + multiplier);
        }
    }
}
