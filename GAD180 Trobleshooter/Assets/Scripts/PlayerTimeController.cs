using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTimeController : MonoBehaviour
{
    private bool timeIsSlow = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!timeIsSlow)
            {
                Time.timeScale = 0.5f;
                timeIsSlow = true;
            }
            else
            {
                Time.timeScale = 1f;
                timeIsSlow = false;
            }
        }
    }
}
