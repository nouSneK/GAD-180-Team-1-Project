using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] weapons;

    public Transform[] spawnPoints;

    public bool spawnTriggered;
    private bool hasSpawned;

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
            }
        }

        hasSpawned = true;
    }
}
