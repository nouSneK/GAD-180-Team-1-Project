using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject deathMenu;
    public GameObject pauseMenu;

    public bool isPaused;
    public bool playerIsDead;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !playerIsDead)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isPaused = false;
        }
    }

    public void PlayerDeath()
    {
        Time.timeScale = 0;
        deathMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerIsDead = true;
    }
}
