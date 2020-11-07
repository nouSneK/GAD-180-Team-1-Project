using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlatform : MonoBehaviour
{
    public float launchForce = 500;
    public float coolDown = 1;
    private float currentLaunchForce;

    private bool canLaunch = true;

    void Start()
    {
        currentLaunchForce = launchForce;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() && canLaunch)
        {
            //collision.gameObject.transform.position = new Vector3(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y + 10, collision.gameObject.transform.position.z);

            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * currentLaunchForce);

            canLaunch = false;

            Invoke("CoolDown", coolDown);
        }
    }

    void CoolDown()
    {
        canLaunch = true;
    }
}
