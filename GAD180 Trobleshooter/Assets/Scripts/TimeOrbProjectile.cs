using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOrbProjectile : MonoBehaviour
{
    public ParticleSystem[] particles;
    public GameObject destroyFx;

    private float speed = 1;
    private Vector3 destination;

    private Color color;

    private Rigidbody rb;

    void Start()
    {
        if (gameObject.GetComponent<Rigidbody>())
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        /*
        transform.LookAt(destination);
        transform.Translate(destination * speed * Time.deltaTime);
        */

        Vector3.Lerp(transform.position, destination, speed * 10000);

        if(Vector3.Distance(transform.position, destination) < 1)
        {
            GameObject fx = Instantiate(destroyFx, transform.position, transform.rotation);
            fx.GetComponent<ParticleSystem>().startColor = color;
            fx.GetComponentInChildren<ParticleSystem>().startColor = color;

            Destroy(fx, 1);
            Destroy(gameObject);
        }
    }

    public void SetStats(float s, Vector3 d, Color c)
    {
        speed = s;
        destination = d;
        color = c;

        Debug.Log("Orb Speed: " + speed);
        Debug.Log("Orb Destination: " + destination);

        gameObject.GetComponent<MeshRenderer>().material.color = c;
        gameObject.GetComponent<TrailRenderer>().startColor = c;
        gameObject.GetComponent<TrailRenderer>().endColor = c;

        foreach(ParticleSystem ps in particles)
        {
            ps.startColor = c;
        }
    }
}
