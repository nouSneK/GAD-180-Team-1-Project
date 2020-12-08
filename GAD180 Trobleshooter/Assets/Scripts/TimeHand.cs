using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHand : MonoBehaviour
{
    public int defultOrb = 0;
    public int selectedOrb = 0;
    public int numberOfOrbs = 2;
    public int numberOfMeleeHits = 1;
    public int meleeDamage = 1;

    public float timeMultiplier = 2;
    public float maxOrbRange = 100;
    public float meleeAttackTime = 1;

    public GameObject handOrb;
    public GameObject projectileOrb;
    public GameObject meleeFistsCollision;
    public float projectileOrbTime = 0.5f;

    public Transform orbLaunchPoint;

    private GameObject playerCam;
    private GameObject player;
    private GameObject speedOrbHitObject;
    private GameObject slowOrbHitObject;

    private Animator animator;

    public Color[] orbColors;

    public string meleeAnimation;

    private bool canMeleeAttack = true;

    private RaycastHit hit;

    public AudioClip slowDownTimeSound;
    public AudioClip speedUpTimeSound;
    private AudioSource audioSource;

    void Start()
    {
        selectedOrb = defultOrb;

        UpdateOrb();

        playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        player = GameObject.FindGameObjectWithTag("Player");

        animator = gameObject.GetComponent<Animator>();

        if (gameObject.GetComponent<AudioSource>())
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectOrb();
        }

        if (Input.GetMouseButtonDown(1))
        {
            LaunchOrb();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            MeleeAttack();
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
                if (hit.collider.GetComponentInParent<TimeInterractableAnimation>() && !hit.collider.GetComponentInParent<TimeInterractableAnimation>().timeOrbHit)
                {
                    SetOrb(hit.collider.gameObject);
                }
                else if(hit.collider.GetComponentInParent<MovingPlatform>() && !hit.collider.GetComponentInParent<MovingPlatform>().timeOrbHit)
                {
                    SetOrb(hit.collider.gameObject);
                }
                else if (hit.collider.GetComponent<LaunchPlatform>() && !hit.collider.GetComponent<LaunchPlatform>().timeOrbHit)
                {
                    SetOrb(hit.collider.gameObject);
                }
                else if (hit.collider.GetComponent<RobotAI>() && !hit.collider.GetComponent<RobotAI>().timeOrbHit)
                {
                    SetOrb(hit.collider.gameObject);
                }

                CreateOrb();

                if (hit.collider.GetComponentInParent<TimeInterractableAnimation>() || hit.collider.GetComponentInParent<MovingPlatform>() || hit.collider.GetComponent<LaunchPlatform>() || hit.collider.GetComponent<RobotAI>())
                {
                    if (selectedOrb == 0)
                    {
                        if (slowOrbHitObject && slowOrbHitObject == hit.collider.gameObject)
                        {
                            ResetOrb(slowOrbHitObject);

                            slowOrbHitObject = null;
                        }

                        if (speedOrbHitObject)
                        {
                            ResetOrb(speedOrbHitObject);

                            speedOrbHitObject = hit.collider.gameObject;
                        }
                        else 
                        {
                            speedOrbHitObject = hit.collider.gameObject;
                        }
                    }
                    else if(selectedOrb == 1)
                    {
                        if (speedOrbHitObject && speedOrbHitObject == hit.collider.gameObject)
                        {
                            ResetOrb(speedOrbHitObject);

                            speedOrbHitObject = null;
                        }

                        if (slowOrbHitObject)
                        {
                            ResetOrb(speedOrbHitObject);

                            slowOrbHitObject = hit.collider.gameObject;
                        }
                        else
                        {
                            slowOrbHitObject = hit.collider.gameObject;
                        }
                    }
                }
            }
        }
    }

    void SetOrb(GameObject setObject)
    {
        if (setObject)
        {
            if (setObject.GetComponentInParent<TimeInterractableAnimation>())
            {
                hit.collider.GetComponentInParent<TimeInterractableAnimation>().OrbHit(selectedOrb, timeMultiplier);
            }
            else if (setObject.GetComponentInParent<MovingPlatform>())
            {
                hit.collider.GetComponentInParent<MovingPlatform>().OrbHit(selectedOrb, timeMultiplier);
            }
            else if (setObject.GetComponent<LaunchPlatform>())
            {
                hit.collider.GetComponent<LaunchPlatform>().ChangeLaunchForce(selectedOrb);
            }
            else if (setObject.GetComponent<RobotAI>())
            {
                hit.collider.GetComponent<RobotAI>().TimeOrbHit(selectedOrb, timeMultiplier, gameObject);
            }
        }
    }

    public void ResetOrb(GameObject resetObject)
    {
        if (resetObject)
        {
            if (resetObject.GetComponentInParent<TimeInterractableAnimation>())
            {
                resetObject.GetComponentInParent<TimeInterractableAnimation>().TimeOrbRelease();
            }
            else if (resetObject.GetComponentInParent<MovingPlatform>())
            {
                resetObject.GetComponentInParent<MovingPlatform>().TimeOrbRelease();
            }
            else if (resetObject.GetComponent<LaunchPlatform>())
            {
                resetObject.GetComponent<LaunchPlatform>().TimeOrbRelease();
            }
            else if (resetObject.GetComponent<RobotAI>())
            {
                resetObject.GetComponent<RobotAI>().TimeOrbRelease();
            }
        }
    }

    void MeleeAttack()
    {
        if (canMeleeAttack)
        {
            canMeleeAttack = false;

            animator.Play(meleeAnimation);

            Invoke("MeleeCoolDown", meleeAttackTime);
        }
    }

    void MeleeCoolDown()
    {
        canMeleeAttack = true;

        meleeFistsCollision.GetComponent<PlayerMeleeCollision>().currentNumberOfMeleeHits = 0;
    }

    private void CreateOrb()
    {
        float distance = Vector3.Distance(hit.point, player.transform.position);
        float speed = distance / projectileOrbTime;

        GameObject orb = Instantiate(projectileOrb, orbLaunchPoint.position, orbLaunchPoint.rotation);
        orb.GetComponent<TimeOrbProjectile>().SetStats(speed, hit.point, orbColors[selectedOrb]);

        if(selectedOrb == 0)
        {
            if(audioSource && speedUpTimeSound)
            {
                audioSource.clip = speedUpTimeSound;

                audioSource.Play();
            }
        }
        else if (selectedOrb == 1)
        {
            if (audioSource && slowDownTimeSound)
            {
                audioSource.clip = slowDownTimeSound;

                audioSource.Play();
            }
        }
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
