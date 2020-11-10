using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject spawnObject;
    public GameObject[] weapons;

    public bool robotrandomWeapon;

    public string keyboardInput = "g";

    void Update()
    {
        if (Input.GetKeyDown(keyboardInput))
        {
            GameObject sObject = Instantiate(spawnObject, transform.position, transform.rotation);
            
            if(robotrandomWeapon && sObject.GetComponent<RobotAI>())
            {
                int rand = Random.Range(0, weapons.Length);

                sObject.GetComponent<RobotAI>().startingWeapon = weapons[rand];
                //sObject.GetComponent<RobotAI>().SetStartingWeapon();
            }
        }
    }
}
