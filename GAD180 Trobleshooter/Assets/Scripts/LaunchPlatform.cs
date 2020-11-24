using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlatform : MonoBehaviour
{
    public float normalLaunchForce = 500;
    public float speedUpLaunchForce = 500;
    public float slowDownLaunchForce = 500;
    public float coolDown = 1;
    private float currentLaunchForce;

    private bool canLaunch = true;

    void Start()
    {
        currentLaunchForce = normalLaunchForce;
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

    public void ChangeLaunchForce(int orbType)
    {
        if(orbType == 0)
        {
            currentLaunchForce = speedUpLaunchForce;
        }
        else if(orbType == 1)
        {
            currentLaunchForce = slowDownLaunchForce;
        }
    }

    void CoolDown()
    {
        canLaunch = true;
    }
}
