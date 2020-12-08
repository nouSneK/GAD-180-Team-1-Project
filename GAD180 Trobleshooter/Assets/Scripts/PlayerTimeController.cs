using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerTimeController : MonoBehaviour
{
    public float maxTimeJuice = 5;
    public float coolDownTime = 30;
    private float currentTimeJuice;

    private AudioSource audioSource;

    public TMP_Text timeSlowText;

    private bool timeIsSlow = false;

    void Start()
    {
        currentTimeJuice = maxTimeJuice;

        if (gameObject.GetComponent<AudioSource>())
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SlowDownTime();
        }

        if(!timeIsSlow && currentTimeJuice < maxTimeJuice)
        {
            currentTimeJuice += (maxTimeJuice / coolDownTime) * Time.deltaTime;
        }
        else if (!timeIsSlow && currentTimeJuice > maxTimeJuice)
        {
            currentTimeJuice = maxTimeJuice;

            UpdateSlowText();
        }

        if (timeIsSlow && currentTimeJuice > 0)
        {
            currentTimeJuice -= Time.deltaTime;
        }
        else if (currentTimeJuice < 0 || (currentTimeJuice == 0 & Time.timeScale == 0.5f))
        {
            currentTimeJuice = 0;

            Time.timeScale = 1f;

            timeIsSlow = false;
        }

        if(currentTimeJuice < maxTimeJuice)
        {
            UpdateSlowText();
        }
    }

    void UpdateSlowText()
    {
        float roundedNumber;
        roundedNumber = Mathf.Round(currentTimeJuice * 100) / 100;

        timeSlowText.text = "Time Slow = " + roundedNumber + " / " + maxTimeJuice;
    }

    void SlowDownTime()
    {
        if(currentTimeJuice == maxTimeJuice)
        {
            Time.timeScale = 0.5f;
            timeIsSlow = true;

            if(audioSource)
            {
                audioSource.Play();
            }
        }
        else if (currentTimeJuice < maxTimeJuice)
        {
            if (!timeIsSlow)
            {
                Time.timeScale = 0.5f;
                timeIsSlow = true;

                if (audioSource)
                {
                    audioSource.Play();
                }
            }
            else
            {
                Time.timeScale = 1f;
                timeIsSlow = false;
            }
        }
    }

    public void AddTimeJuice()
    {
        currentTimeJuice += 2;

        if(currentTimeJuice > maxTimeJuice)
        {
            currentTimeJuice = maxTimeJuice;
        }

        UpdateSlowText();
    }
}
