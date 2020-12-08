using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;
    private int startingHealth;

    public float playerHitCooldown = 1;

    private bool canTakeDamage = true;

    public PostProcessVolume ppVolume;

    public PostProcessProfile defultppfx;
    public PostProcessProfile deathppfx;

    public GameObject playerCam;
    public GameObject levelManager;

    public Animator hUDColorAnimator;

    private AudioSource audioSource;

    public TMP_Text healthText;

    private void Start()
    {
        startingHealth = health;

        DisplayHealth();

        if (gameObject.GetComponent<AudioSource>())
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            health -= damage;

            DisplayHealth();

            canTakeDamage = false;

            Invoke("HitCooldown", playerHitCooldown);

            CheckForDeath();

            playerCam.GetComponent<Animator>().Play("PlayerTakeDamageFx");

            if (hUDColorAnimator)
            {
                hUDColorAnimator.Play("PlayerHitRedFx");
            }

            if (audioSource)
            {
                audioSource.Play();
            }
        }
    }

    void HitCooldown()
    {
        canTakeDamage = true;
    }

    public void AddHealth(int healthAmount)
    {
        health += healthAmount;

        if(health > startingHealth)
        {
            health = startingHealth;
        }

        DisplayHealth();

        if (health > 1)
        {
            ppVolume.profile = defultppfx;
        }
    }

    void DisplayHealth()
    {
        if (healthText)
        {
            healthText.text = "Health: " + health.ToString() + " / " + startingHealth.ToString();
        }
    }

    void CheckForDeath()
    {
        if(health == 1)
        {
            ppVolume.profile = deathppfx;
        }
        else if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (levelManager)
        {
            levelManager.GetComponent<LevelManager>().PlayerDeath();
        }
        else
        {
            Debug.Log("Need to assign Level Manager!!!");
        }
    }
}
