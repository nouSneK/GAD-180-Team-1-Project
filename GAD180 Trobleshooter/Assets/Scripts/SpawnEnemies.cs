using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] weapons;

    public Transform[] spawnPoints;

    public GameObject[] destroyBounds;
    public GameObject[] wanderingPaths;

    public bool spawnTriggered;
    private bool hasSpawned;

    public GameObject openDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealth>() && !hasSpawned)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (enemies.Length > 1)
        {
            for(int i = 0; i < enemies.Length; i++)
            {
                GameObject enemy = Instantiate(enemies[i], spawnPoints[i].position, spawnPoints[i].rotation);

                if (enemy.GetComponent<RobotAI>())
                {
                    if (weapons[i] != null)
                    {
                        enemy.GetComponent<RobotAI>().startingWeapon = weapons[i];
                    }

                    if (spawnTriggered)
                    {
                        enemy.GetComponent<RobotAI>().PlayerTrigger();
                    }

                    if (wanderingPaths.Length > 0 && wanderingPaths[i] != null)
                    {
                        enemy.GetComponent<RobotAI>().hasWanderingPath = true;
                        enemy.GetComponent<RobotAI>().wanderPoints.Add(wanderingPaths[i]);

                        for(int child = 0; child < wanderingPaths[i].transform.childCount; child++)
                        {
                            enemy.GetComponent<RobotAI>().wanderPoints.Add(wanderingPaths[i].transform.GetChild(child).gameObject);
                        }
                    }

                    if (openDoor)
                    {
                        openDoor.GetComponent<DoorScript>().closeObjects.Add(enemy);
                    }
                }
            }
        }
        else
        {
            foreach (Transform location in spawnPoints)
            {
                GameObject enemy = Instantiate(enemies[0], location.position, location.rotation);

                if (enemy.GetComponent<RobotAI>())
                {
                    if (weapons[0] != null)
                    {
                        enemy.GetComponent<RobotAI>().startingWeapon = weapons[0];
                    }

                    if (spawnTriggered)
                    {
                        enemy.GetComponent<RobotAI>().PlayerTrigger();
                    }
                }

                if (openDoor)
                {
                    openDoor.GetComponent<DoorScript>().closeObjects.Add(enemy);
                }
            }
        }

        if(destroyBounds.Length > 0)
        {
            foreach(GameObject bounds in destroyBounds)
            {
                bounds.SetActive(false);
            }
        }

        if (openDoor)
        {
            openDoor.GetComponent<DoorScript>().searchForRobts = true;
        }

        hasSpawned = true;
    }
}
