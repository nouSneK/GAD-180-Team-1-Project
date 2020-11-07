using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject spawnObject;

    public string keyboardInput = "g";

    void Update()
    {
        if (Input.GetKeyDown(keyboardInput))
        {
            Instantiate(spawnObject, transform.position, transform.rotation);
        }
    }
}
