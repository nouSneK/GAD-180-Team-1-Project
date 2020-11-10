using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentOpening : MonoBehaviour
{
    public bool isOpened;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<Rigidbody>() && collision.collider.GetComponent<Rigidbody>().velocity.magnitude > 3)
        {
            Open();
        }
    }

    public void Open()
    {
        if (gameObject.GetComponent<Rigidbody>())
        {
            isOpened = true;

            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
