using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour
{
    public GameObject[] limbs;

    public int health = 1;
    public int assultRifleBurstNumber = 4;
    public int currentAssultRifleBurstNumber;
    private int weaponType = 0;
    private int targetLocationIndex = 0;

    public float turningSpeed = 3;
    public float movementSpeed = 3.5f;
    public float walkingMovementSpeed = 1.5f;
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
    private bool hasSightOfTarget;
    private bool hasSetWanderPoints;

    public string runningAnimation;
    public string aimAnimation;

    public GameObject target;
    public GameObject startingWeapon;
    public GameObject weapon;
    public GameObject weaponHand;
    public GameObject visionTrigger;
    public GameObject visionRaycaster;
    //public GameObject[] meleeHitBoxes;
    private GameObject weaponProjectile;
    private GameObject player;

    private NavMeshAgent navMesh;

    private Animator animator;

    //Wandering Path
    public bool hasWanderingPath;
    public List<GameObject> wanderPoints;

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

        player = GameObject.Find("Player");

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
                PlayerTrigger();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Die();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                animator.Play(runningAnimation);
            }

            if (isTriggered && weapon && weapon.gameObject.GetComponent<Weapon>().projectile && attackTimer > 0)
            {
                RaycastHit hit;
                QueryTriggerInteraction qt = QueryTriggerInteraction.Ignore;

                if (Physics.SphereCast(weapon.GetComponent<Weapon>().shootPoint.transform.position, 0.1f, weapon.GetComponent<Weapon>().shootPoint.transform.forward, out hit, 1000))
                {
                    Debug.DrawLine(weapon.GetComponent<Weapon>().shootPoint.transform.position, hit.point, Color.red);

                    if (hit.collider.gameObject == target)
                    {
                        attackTimer -= 1 * Time.deltaTime;
                    }
                }
            }
            else if (isTriggered && attackTimer > 0)
            {
                attackTimer -= 1 * Time.deltaTime;
            }
        }
    }

    public void PlayerTrigger()
    {
        target = GameObject.Find("Player");

        Invoke("Trigger", 0.1f);
    }

    public void Trigger()
    {
        isTriggered = true;

        animator.SetBool("isTriggered", true);

        visionTrigger.SetActive(false);

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

                animator.SetBool("meleeAttack", false);
                animator.SetBool("isRunning", true);

                //animator.Play(runningAnimation);
            }
        }
        else
        {
            meleeWeapon = true;

            animator.SetBool("meleeAttack", false);
            animator.SetBool("isRunning", true);

            //animator.Play(runningAnimation);
        }

        navMesh.speed = movementSpeed;

        GameObject[] robots = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject robot in robots)
        {
            if (robot.GetComponent<RobotAI>() && !robot.GetComponent<RobotAI>().isTriggered)
            {
                robot.GetComponent<RobotAI>().PlayerTrigger();
            }
        }

        //animator.SetBool("isTriggered", true);
    }

    public void LookForPlayer()
    {
        RaycastHit hit;

        visionRaycaster.transform.LookAt(player.transform);

        if (!isTriggered && Physics.SphereCast(visionRaycaster.transform.position, 0.5f, visionRaycaster.transform.forward, out hit, 400))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            if(hit.collider.gameObject == GameObject.Find("Player"))
            {
                PlayerTrigger();
            }
        }
    }

    void Attacking()
    {
        if (isTriggered && attackTimer < 0)
        {
            if (weapon && weapon.gameObject.GetComponent<Weapon>().projectile)
            {
                if(weaponType == 1)
                {
                    attackTimer = attackRate;

                    AssultRifleFire();
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

                    Debug.Log("isRunning = false");
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

    void AssultRifleFire()
    {
        if (currentAssultRifleBurstNumber < assultRifleBurstNumber)
        {
            currentAssultRifleBurstNumber++;

            weapon.GetComponent<Weapon>().FireWeapon();

            if(currentAssultRifleBurstNumber >= assultRifleBurstNumber)
            {
                currentAssultRifleBurstNumber = 0;
            }
            else
            {
                Invoke("AssultRifleFire", 0.1f);
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
                }

                animator.SetBool("meleeAttack", false);
                animator.SetBool("isRunning", true);

                if(target)
                navMesh.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y - 1, target.transform.position.z));
            }
            else
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, turningSpeed);
                Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

                transform.LookAt(targetPos);

                if(weapon && weapon.GetComponent<Weapon>().shootPoint)
                {
                    weapon.GetComponent<Weapon>().shootPoint.transform.LookAt(target.transform.position);
                }
            }
        }
        else if(!isTriggered && hasWanderingPath)
        {
            if (!hasSetWanderPoints)
            {
                hasSetWanderPoints = true;

                //animator.SetBool("isTriggered", true);

                target = wanderPoints[targetLocationIndex];

                navMesh.enabled = true;
                navMesh.speed = walkingMovementSpeed;
            }

            navMesh.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y - 1, target.transform.position.z));

            if (Vector3.Distance(transform.position, target.transform.position) < 3)
            {
                if(targetLocationIndex < wanderPoints.Count)
                {
                    targetLocationIndex++;

                    Debug.Log("Add Target Index");
                }
                else
                {
                    targetLocationIndex = 0;

                    Debug.Log("Set Target Index to 0");
                }

                target = wanderPoints[targetLocationIndex];
            }
        }
    }

    public void SetStartingWeapon()
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

            Debug.Log(this.name + " weapon set " + weapon.gameObject.name);
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
        weapon.GetComponent<Weapon>().shootPoint = null;
        weapon.GetComponent<Weapon>().WeaponModeOff();

        if (weapon.GetComponent<Weapon>().projectile)
        {
            weapon.GetComponent<Weapon>().ammoCost = 1;
        }

        weapon.transform.parent = null;

        weapon = null;
    }
}
