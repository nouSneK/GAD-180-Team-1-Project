using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Boss1Robot : MonoBehaviour
{
    public int health = 100;

    public bool isAlive = true;

    public int startingHealth = 100;

    public float turningSpeed = 3;
    public float movementSpeed = 3.5f;
    public float normalAttackRate = 2;
    public float behaviourChangeRate = 7;
    public float behaviour2Time = 5;
    public float behaviour3Time = 5;
    public float attack1MissileTime = 1.5f;
    public float meleeAttackTime = 2f;
    public float meleeRange = 2;
    public float stunTime = 5;
    private float normalAttackTimer;
    private float attackingTimer;
    private float behaviourTimer;
    private float behaviour = 1;
    private float timeMultiplier = 1;

    public bool isTriggered;
    public bool timeOrbHit;
    public bool isCharging;
    public bool meleeAttackHit;
    private bool hasSightOfTarget;
    private bool isAttacking;
    private bool meleeAttack;

    public GameObject target;
    public GameObject visionTrigger;
    public GameObject visionRaycaster;
    public GameObject destroyFx;
    public GameObject homingMissile;
    public GameObject normalMissile;
    public GameObject robotTopHalf;
    public GameObject stompFx;
    private GameObject player;
    private GameObject timeHands;

    public Transform[] attack1MissileShootPoints;
    public Transform[] stompFxLocation;
    public Transform normalMissileShootPoint;

    private NavMeshAgent navMesh;

    private Animator animator;

    public ParticleSystem speedUpParticles;
    public ParticleSystem slowDownParticles;

    public List<GameObject> limbs;

    void Start()
    {
        if (gameObject.GetComponent<NavMeshAgent>())
        {
            navMesh = gameObject.GetComponent<NavMeshAgent>();
        }

        if (gameObject.GetComponent<Animator>())
        {
            animator = gameObject.GetComponent<Animator>();
        }

        player = GameObject.Find("Player");

        health = startingHealth;
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

            if (isCharging)
            {
                Charging();
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
        behaviourTimer = behaviourChangeRate;

        normalAttackTimer = normalAttackRate;

        isTriggered = true;

        animator.SetBool("isTriggered", true);

        visionTrigger.SetActive(false);

        navMesh.speed = movementSpeed;

        GameObject[] robots = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject robot in robots)
        {
            if (robot.GetComponent<RobotAI>() && !robot.GetComponent<RobotAI>().isTriggered)
            {
                robot.GetComponent<RobotAI>().PlayerTrigger();
            }
        }

        animator.SetBool("isTriggered", true);
    }

    public void LookForPlayer()
    {
        RaycastHit hit;

        visionRaycaster.transform.LookAt(player.transform);

        if (!isTriggered && Physics.SphereCast(visionRaycaster.transform.position, 0.5f, visionRaycaster.transform.forward, out hit, 400))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            if (hit.collider.gameObject == GameObject.Find("Player"))
            {
                PlayerTrigger();
            }
        }
    }

    void Attacking()
    {
        if (isTriggered)
        {
            if (normalAttackTimer > 0 && behaviour == 1 && !isAttacking && !isCharging)
            {
                normalAttackTimer -= timeMultiplier * Time.deltaTime;

                Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

                robotTopHalf.transform.LookAt(targetPos);
            }

            if (meleeAttackHit)
            {
                CreateStompFx();
            }

            if (behaviourTimer > 0 && !isCharging && !meleeAttack)
            {
                behaviourTimer -= timeMultiplier * Time.deltaTime;
            }

            if (normalAttackTimer <= 0 && behaviour == 1)
            {
                NormalRangedAttack();

                normalAttackTimer = normalAttackRate;
            }

            if(attackingTimer > 0 && isAttacking)
            {
                attackingTimer -= timeMultiplier * Time.deltaTime;
            }

            if(attackingTimer <= 0 && isAttacking)
            {
                isAttacking = false;

                Behaviour1();
            }

            if(Vector3.Distance(transform.position, player.transform.position) < meleeRange && !isAttacking && !isCharging)
            {
                MeleeAttack();
            }

            if (behaviourTimer <= 0 && !isAttacking)
            {
                int randomBehaviour = Random.Range(1, 4);

                if (randomBehaviour == 2)
                {
                    behaviour = 2;

                    attackingTimer = behaviour2Time;

                    isAttacking = true;

                    Attack1();

                    Debug.Log("Attack 1 (Missile)");
                }
                else if (randomBehaviour == 3)
                {
                    behaviour = 3;

                    isAttacking = true;

                    Attack2();

                    Debug.Log("Attack 2 (Charge)");
                }
                else
                {
                    Behaviour1();

                    Debug.Log("Defult Attack (Walk + Shoot)");
                }
            }
        }
    }

    void CreateStompFx()
    {
        foreach (Transform spawnPoint in stompFxLocation)
        {
            Instantiate(stompFx, spawnPoint.position, Quaternion.identity);
        }
    }

    void MeleeAttack()
    {
        meleeAttack = true;

        animator.SetBool("meleeAttack", true);

        attackingTimer = meleeAttackTime * timeMultiplier;

        isAttacking = true;

        Invoke("Behaviour1", meleeAttackTime * timeMultiplier);
    }

    void NormalRangedAttack()
    {
        normalMissileShootPoint.transform.LookAt(player.transform.position);

        Instantiate(normalMissile, normalMissileShootPoint.position, normalMissileShootPoint.rotation);

        Debug.Log("Fire Defult Missile");

        //Play fx & sounds & stuff
    }

    void Behaviour1()
    {
        animator.SetBool("meleeAttack", false);

        behaviour = 1;

        behaviourTimer = behaviourChangeRate;

        meleeAttack = false;

        Debug.Log("Set behaviour to 1");
    }

    void Attack1()
    {
        animator.SetBool("Attack1", true);

        navMesh.SetDestination(transform.position);

        Invoke("Attack1Missiles", attack1MissileTime);
    }

    void Attack1Missiles()
    {
        Debug.Log("Attack 1: Launch homing missiles");

        foreach (Transform spawnPoint in attack1MissileShootPoints)
        {
            Instantiate(homingMissile, spawnPoint.position, spawnPoint.rotation);
        }

        animator.SetBool("Attack1", false);
    }

    void Attack2()
    {
        RaycastHit hit;

        //Physics.SphereCast(visionRaycaster.transform.position, 2f, visionRaycaster.transform.forward, out hit, 400);

        //navMesh.SetDestination(hit.point);

        animator.SetBool("isCharging", true);

        navMesh.speed = 9;

        Debug.Log("Attack 2 Set");

        Invoke("GetUp", 10);

        isCharging = true;

        //Play ready charge animation that leads into charge animation, the charge animation sets the isCharging bool to true
    }

    void Charging()
    {
        transform.position += transform.forward * timeMultiplier * Time.deltaTime * 10;

        RaycastHit hit;

        if (Physics.SphereCast(visionRaycaster.transform.position, 2f, visionRaycaster.transform.forward, out hit, 4))
        {
            if (hit.collider != null)
            {
                GetUp();

                //Fall animation

                Invoke("GetUp", stunTime * timeMultiplier);
            }
        }
    }

    void GetUp()
    {
        isCharging = false;

        animator.SetBool("isCharging", false);

        target = player;

        navMesh.speed = movementSpeed;

        Debug.Log("Get up");

        Behaviour1();
    }

    void Movement()
    {
        if (isTriggered && target)
        {
            if (behaviour == 1 && Vector3.Distance(transform.position, target.transform.position) > meleeRange && !isCharging)
            {
                if (navMesh.speed == 0)
                {
                    navMesh.speed = movementSpeed;
                }

                animator.SetBool("meleeAttack", false);
                animator.SetBool("isRunning", true);

                if (target)
                {
                    navMesh.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y - 1, target.transform.position.z));
                }
            }
        }
    }

    public void Ragdoll()
    {
        animator.enabled = false;
        navMesh.enabled = false;

        if (gameObject.GetComponent<CapsuleCollider>())
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }

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
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isAlive = false;

        Ragdoll();

        if (timeOrbHit)
        {
            timeHands.GetComponent<TimeHand>().ResetOrb(gameObject);
        }

        if (destroyFx)
        {
            Instantiate(destroyFx, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Quaternion.identity);
        }
    }    

    public void TimeOrbHit(int type, float multiplier, GameObject hands)
    {
        if (type == 0)
        {
            navMesh.speed = navMesh.speed * multiplier;

            animator.speed = animator.speed * multiplier;

            timeMultiplier = timeMultiplier * multiplier;

            if (speedUpParticles && slowDownParticles)
            {
                speedUpParticles.Play();
                slowDownParticles.Stop();
            }

            //gameObject.GetComponent<SkinnedMeshRenderer>().material = speedUpMaterial;
        }
        else if (type == 1)
        {
            navMesh.speed = navMesh.speed / (3 * multiplier);
            animator.speed = animator.speed / (3 * multiplier);

            timeMultiplier = timeMultiplier / (multiplier);

            if (speedUpParticles && slowDownParticles)
            {
                slowDownParticles.Play();
                speedUpParticles.Stop();
            }

            //gameObject.GetComponent<SkinnedMeshRenderer>().material = slowDownMaterial;
        }

        timeHands = hands;

        timeOrbHit = true;
    }

    public void TimeOrbRelease()
    {
        navMesh.speed = movementSpeed;
        animator.speed = 1;
        timeMultiplier = 1;

        timeOrbHit = false;

        if (speedUpParticles && slowDownParticles)
        {
            speedUpParticles.Stop();
            slowDownParticles.Stop();
        }

        //gameObject.GetComponent<SkinnedMeshRenderer>().material = defultMaterial;
    }
}
