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

    private List<GameObject> spawnedRobots;

    public bool spawnTriggered;
    private bool hasSpawned;

    private GameObject[] openDoor;

    private void Start()
    {
        openDoor = GameObject.FindGameObjectsWithTag("Door");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealth>() && !hasSpawned)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        spawnedRobots = new List<GameObject>();

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

                    foreach (GameObject door in openDoor)
                    {
                        door.GetComponent<DoorScript>().closeObjects.Add(enemy);
                    }

                    spawnedRobots.Add(enemy);
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

                foreach(GameObject door in openDoor)
                {
                    door.GetComponent<DoorScript>().closeObjects.Add(enemy);
                }

                spawnedRobots.Add(enemy);
            }
        }

        if(destroyBounds.Length > 0)
        {
            foreach(GameObject bounds in destroyBounds)
            {
                bounds.SetActive(false);
            }
        }

        foreach (GameObject door in openDoor)
        {
            door.GetComponent<DoorScript>().searchForRobts = true;
        }

        CheckForReviveRobot();

        hasSpawned = true;
    }

    void CheckForReviveRobot()
    {
        foreach (GameObject robot in spawnedRobots)
        {
            if (robot.GetComponent<RobotAI>() && robot.GetComponent<RobotAI>().enemyType == 2)
            {
                foreach(GameObject otherRobot in spawnedRobots)
                {
                    if(otherRobot.GetComponent<RobotAI>() && otherRobot.GetComponent<RobotAI>().enemyType != 2)
                    {
                        robot.GetComponent<RobotAI>().robots.Add(otherRobot);
                    }
                }
            }
        }
    }
}
