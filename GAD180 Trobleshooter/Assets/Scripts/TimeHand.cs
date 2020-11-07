﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHand : MonoBehaviour
{
    public int defultOrb = 0;
    public int selectedOrb = 0;
    public int numberOfOrbs = 2;

    public float timeMultiplier = 2;
    public float maxOrbRange = 100;

    public GameObject handOrb;
    public GameObject projectileOrb;
    public float projectileOrbTime = 0.5f;

    public Transform orbLaunchPoint;

    private GameObject playerCam;
    private GameObject player;

    public Color[] orbColors;

    private RaycastHit hit;

    void Start()
    {
        selectedOrb = defultOrb;

        UpdateOrb();

        playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectOrb();
        }

        if (Input.GetMouseButtonDown(0))
        {
            LaunchOrb();
        }

        Debug.DrawLine(playerCam.transform.position, hit.point, Color.green);
    }

    private void LaunchOrb()
    {
        if (selectedOrb == 0 || selectedOrb == 1) 
        {
            Physics.SphereCast(playerCam.transform.position, 1, playerCam.transform.forward, out hit, maxOrbRange);
            Debug.DrawLine(playerCam.transform.position, hit.point, Color.green);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponentInParent<TimeInterractableAnimation>())
                {
                    hit.collider.GetComponentInParent<TimeInterractableAnimation>().OrbHit(selectedOrb, timeMultiplier);

                    float distance = Vector3.Distance(player.transform.position, hit.point);
                    float speed = distance / projectileOrbTime;

                    GameObject orb = Instantiate(projectileOrb, orbLaunchPoint.position, orbLaunchPoint.rotation);
                    orb.GetComponent<TimeOrbProjectile>().SetStats(speed, hit.point, orbColors[selectedOrb]);
                }
                else if(hit.collider.GetComponentInParent<MovingPlatform>())
                {
                    hit.collider.GetComponentInParent<MovingPlatform>().OrbHit(selectedOrb, timeMultiplier);

                    CreateOrb();
                }
            }
        }
    }

    private void CreateOrb()
    {
        float distance = Vector3.Distance(hit.point, player.transform.position);
        float speed = distance / projectileOrbTime;

        GameObject orb = Instantiate(projectileOrb, orbLaunchPoint.position, orbLaunchPoint.rotation);
        orb.GetComponent<TimeOrbProjectile>().SetStats(speed, hit.point, orbColors[selectedOrb]);
    }

    private void SelectOrb()
    {
        if (selectedOrb < numberOfOrbs - 1)
        {
            selectedOrb++;
        }
        else
        {
            selectedOrb = 0;
        }

        UpdateOrb();
    }

    private void UpdateOrb()
    {
        handOrb.GetComponent<MeshRenderer>().material.color = orbColors[selectedOrb];
    }
}