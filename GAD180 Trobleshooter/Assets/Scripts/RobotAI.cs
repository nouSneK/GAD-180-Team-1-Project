using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour
{
    public GameObject[] limbs;

    public int health = 1;
    private int weaponType = 0;

    public float turningSpeed = 3;
    public float movementSpeed = 3.5f;
    public float attackRate = 2;
    public float meleeRange = 2;
    public float meleeAttackRange = 2;
    public float meleeAttackTime = 0.5f;
    public float dropWeaponTime = 1;
    private float attackTimer;

    public bool isAlive = true;
    public bool isTriggered;
    private bool meleeWeapon;
    private bool meleeAttack;

    public string runningAnimation;
    public string aimAnimation;

    public GameObject target;
    public GameObject startingWeapon;
    public GameObject weapon;
    public GameObject weaponHand;
    //public GameObject[] meleeHitBoxes;
    private GameObject weaponProjectile;

    private NavMeshAgent navMesh;

    private Animator animator;

    void Start()
    {
        StopRagdoll();

        if (gameObject.GetComponent<NavMeshAgent>())
        {
            navMesh = gameObject.GetComponent<NavMeshAgent>();
        }

        if (gameObject.GetComponent<Animator>())
        {
            animator = gameObject.GetComponent<Animator>();
        }

        SetStartingWeapon();
    }

    void Update()
    {
        if (isAlive)
        {
            Movement();
            Attacking();

            if (Input.GetKeyDown(KeyCode.T))
            {
                target = GameObject.Find("Player");

                Trigger();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Die();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                animator.Play(runningAnimation);
            }

            if (isTriggered && attackTimer > 0)
            {
                attackTimer -= 1 * Time.deltaTime;
            }
        }
    }

    public void Trigger()
    {
        isTriggered = true;

        animator.SetBool("isTriggered", true);

        if (weapon)
        {
            if(weapon.GetComponent<Weapon>().projectile != null)
            {
                meleeWeapon = false;

                navMesh.enabled = false;

                animator.Play(aimAnimation);
            }
            else
            {
                meleeWeapon = true;

                animator.Play(runningAnimation);
            }
        }
        else
        {
            meleeWeapon = true;

            animator.Play(runningAnimation);
        }

        //animator.SetBool("isTriggered", true);
    }

    void Attacking()
    {
        if (isTriggered && attackTimer < 0)
        {
            if (weapon)
            {
                if(weaponType == 1)
                {
                    //For assult rifles
                }
                else
                {
                    weapon.GetComponent<Weapon>().FireWeapon();

                    attackTimer = attackRate;
                }
            }
            else
            {
                if(Vector3.Distance(transform.position, target.transform.position) < meleeRange && !meleeAttack)
                {
                    /*
                    foreach (GameObject box in meleeHitBoxes)
                    {
                        box.SetActive(true);
                    }

                    Invoke("TurnOffMeleeHitBoxes", meleeAttackTime);
                    */

                    animator.SetBool("isRunning", false);
                    animator.SetBool("meleeAttack", true);

                    Invoke("MeleeAttackReset", meleeAttackTime);

                    attackTimer = attackRate + meleeAttackTime;

                    navMesh.speed = 0;

                    meleeAttack = true;
                }
            }
        }
    }

    void MeleeAttackReset()
    {
        animator.SetBool("meleeAttack", false);

        if (Vector3.Distance(transform.position, target.transform.position) > meleeRange)
        {
            navMesh.speed = movementSpeed;

            animator.SetBool("isRunning", true);
        }

        meleeAttack = false;
    }

    /*
    void TurnOffMeleeHitBoxes()
    {
        foreach (GameObject box in meleeHitBoxes)
        {
            box.SetActive(false);
        }

        animator.SetBool("meleeAttack", false);
    }
    */

    void Movement()
    {
        if (isTriggered && target && !meleeAttack && Vector3.Distance(transform.position, target.transform.position) > meleeRange)
        {
            if (meleeWeapon)
            {
                if(navMesh.speed == 0)
                {
                    navMesh.speed = movementSpeed;

                    animator.SetBool("meleeAttack", false);
                    animator.SetBool("isRunning", true);
                }

                navMesh.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y - 1, target.transform.position.z));
            }
            else
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, turningSpeed);
                Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

                transform.LookAt(targetPos);
            }
        }
    }

    void SetStartingWeapon()
    {
        if (startingWeapon)
        {
            weapon = Instantiate(startingWeapon, weaponHand.transform.TransformPoint(Vector3.zero), weaponHand.transform.rotation);

            weaponType = weapon.GetComponent<Weapon>().weaponType;

            weapon.GetComponent<Weapon>().enemyControlled = true;
            weapon.GetComponent<Weapon>().ammoCost = 0;
            weapon.GetComponent<Weapon>().shootPoint = weapon.GetComponent<Weapon>().muzzleFlashPoint;
            weapon.GetComponent<Weapon>().WeaponMode();

            weapon.transform.parent = weaponHand.transform;
        }

        attackTimer = attackRate;
    }

    public void StopRagdoll()
    {
        foreach (GameObject limb in limbs)
        {
            if (limb.GetComponent<Rigidbody>())
            {
                limb.GetComponent<Rigidbody>().useGravity = false;
                limb.GetComponent<Rigidbody>().isKinematic = true;
            }

            if (limb.GetComponent<BoxCollider>())
            {
                limb.GetComponent<BoxCollider>().enabled = true;
                limb.GetComponent<BoxCollider>().isTrigger = true;
            }

            if (limb.GetComponent<SphereCollider>())
            {
                limb.GetComponent<SphereCollider>().enabled = true;
                limb.GetComponent<SphereCollider>().isTrigger = true;
            }

            if (limb.GetComponent<CapsuleCollider>())
            {
                limb.GetComponent<CapsuleCollider>().enabled = true;
                limb.GetComponent<CapsuleCollider>().isTrigger = true;
            }
        }
    }

    public void Ragdoll()
    {
        animator.enabled = false;
        navMesh.enabled = false;

        foreach (GameObject limb in limbs)
        {
            if (limb.GetComponent<Rigidbody>())
            {
                limb.GetComponent<Rigidbody>().useGravity = true;
                limb.GetComponent<Rigidbody>().isKinematic = false;
            }

            if (limb.GetComponent<BoxCollider>())
            {
                limb.GetComponent<BoxCollider>().enabled = true;
                limb.GetComponent<BoxCollider>().isTrigger = false;
            }

            if (limb.GetComponent<SphereCollider>())
            {
                limb.GetComponent<SphereCollider>().enabled = true;
                limb.GetComponent<SphereCollider>().isTrigger = false;
            }

            if (limb.GetComponent<CapsuleCollider>())
            {
                limb.GetComponent<CapsuleCollider>().enabled = true;
                limb.GetComponent<CapsuleCollider>().isTrigger = false;
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        CheckForDeath();
    }

    void CheckForDeath()
    {
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isAlive = false;

        Ragdoll();

        if (weapon)
        {
            Invoke("DropWeapon", dropWeaponTime);
        }
    }

    void DropWeapon()
    {
        weapon.GetComponent<Weapon>().enemyControlled = false;
        weapon.GetComponent<Weapon>().ammoCost = 1;
        weapon.GetComponent<Weapon>().shootPoint = null;
        weapon.GetComponent<Weapon>().WeaponModeOff();

        weapon.transform.parent = null;

        weapon = null;
    }
}
