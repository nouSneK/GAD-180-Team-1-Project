using System.Collections;
using System.Collections.Generic;
using UnityEditor.Android;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public bool weaponMode = false;
    public bool enemyControlled = false;
    private bool canShoot = true;
    private bool meleeAttack;
    public bool mouseHold;

    public int bulletsPerShot = 1;
    public int weaponType = 0; //(For Enemy AI) Type 0 = Pistol    Type 1 = AssultRifle/Burst
    public float spread = 0;
    private List<Quaternion> bullets;

    public int clipSize = 10;
    public int maxClipSize = 30;
    public int startingAmmo = 10;
    public int ammoCost = 1;
    public int meleeDamage = 1;
    public int enemiesPerMeleeAttack = 3;
    private int currentEnemiesPerMelleAttack;
    private int ammoOutClip;
    private int ammoInClip;

    public float fireForce = 1000;
    public float fireRate = 0.1f;
    public float reloadTime = 1;
    public float bulletDestroyTime = 10f;
    public float meleeTime = 0.1f;

    public string attackAnimation;

    public GameObject projectile;
    public GameObject shootPoint;
    public GameObject muzzleFlashPoint;

    private Rigidbody rb;

    public Text ammoCounter;

    private Animator animator;

    private void Awake()
    {
        bullets = new List<Quaternion>(bulletsPerShot);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            bullets.Add(Quaternion.Euler(Vector3.zero));
        }
    }

    void Start()
    {
        if (startingAmmo >= clipSize)
        {
            ammoInClip = clipSize;
            ammoOutClip = startingAmmo - clipSize;
        }
        else
        {
            ammoInClip = startingAmmo;
            ammoOutClip = 0;
        }

        if (gameObject.GetComponent<Animator>())
        {
            animator = gameObject.GetComponent<Animator>();
        }

        if (gameObject.GetComponent<Rigidbody>())
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        if (weaponMode && !enemyControlled)
        {
            if (!mouseHold && Input.GetMouseButtonDown(0) && ammoInClip > 0 && canShoot)
            {
                FireWeapon();
            }
            else if (mouseHold && Input.GetMouseButton(0) && ammoInClip > 0 && canShoot)
            {
                FireWeapon();
            }

            if (ammoInClip < clipSize && ammoOutClip > 0 && Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }

        if(!weaponMode && !meleeAttack && rb.velocity.magnitude > 15)
        {
            meleeAttack = true;
        }
        else if (!weaponMode && meleeAttack && rb.velocity.magnitude < 15)
        {
            meleeAttack = false;
        }
    }

    public void FireWeapon()
    {
        canShoot = false;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            bullets[i] = Random.rotation;
            if (projectile) 
            {
                GameObject bullet = Instantiate(projectile, shootPoint.transform.position, shootPoint.transform.rotation);
                //bullet.GetComponent<Rigidbody>().AddForce(shootPoint.transform.forward * fireForce);
                bullet.GetComponent<Projectile>().bulletSpeed = fireForce;
                bullet.transform.rotation = Quaternion.RotateTowards(shootPoint.transform.rotation, bullets[i], spread);

                Destroy(bullet, bulletDestroyTime);
            }
            else
            {
                Debug.Log("Melee Attack");

                meleeAttack = true;

                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.GetComponent<BoxCollider>().enabled = true;

                animator.Play(attackAnimation);

                Invoke("MeleeReset", meleeTime);
            }
        }

        ammoInClip -= ammoCost;
        if(ammoInClip <= 0)
        {
            Reload();
        }
        else
        {
            Invoke("FireRateReset", fireRate);
        }

        if (!enemyControlled)
        {
            UpdateAmmoCounter();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (meleeAttack)
        {
            if (other.GetComponent<RobotCollisionBox>())
            {
                other.GetComponent<RobotCollisionBox>().robotParent.GetComponent<RobotAI>().TakeDamage(meleeDamage);
                other.GetComponent<RobotCollisionBox>().BreakOff();

                currentEnemiesPerMelleAttack++;

                if (currentEnemiesPerMelleAttack >= enemiesPerMeleeAttack)
                {
                    MeleeReset();
                }
            }
            else if (other.GetComponent<VentOpening>())
            {
                other.GetComponent<VentOpening>().Open();
                other.GetComponent<Rigidbody>().AddForce(Vector3.forward * fireForce);
            }
        }
    }

    private void MeleeReset()
    {
        if (weaponMode)
        {
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            currentEnemiesPerMelleAttack = 0;

            meleeAttack = false;
        }
    }

    private void FireRateReset()
    {
        canShoot = true;
    }

    private void Reload()
    {
        //Play reload animation

        Invoke("FinishReloading", reloadTime);
    }

    private void FinishReloading()
    {
        ammoOutClip += ammoInClip;
        ammoInClip = 0;

        if (clipSize < ammoOutClip)
        {
            ammoInClip = clipSize;
            ammoOutClip -= clipSize;
        }
        else
        {
            ammoInClip = ammoOutClip;
            ammoOutClip = 0;
        }

        canShoot = true;
        FireRateReset();
        UpdateAmmoCounter();
    }

    public void PickedUp(GameObject sp, Text ammoC)
    {
        WeaponMode();

        shootPoint = sp;
        ammoCounter = ammoC;

        UpdateAmmoCounter();
    }

    public void WeaponMode()
    {
        if (animator)
        {
            animator.enabled = true;
        }

        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;

        weaponMode = true;
    }

    public void Drop(Vector3 direction, float force)
    {
        WeaponModeOff();

        gameObject.GetComponent<Rigidbody>().AddForce(direction * force);
        gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * (force/5));
    }

    public void WeaponModeOff()
    {
        if (animator)
        {
            animator.enabled = false;
        }

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<BoxCollider>().isTrigger = false;        

        weaponMode = false;
        meleeAttack = false;
    }

    public void UpdateAmmoCounter()
    {
        if (gameObject.activeSelf && weaponMode)
        {
            ammoCounter.text = "Ammo: " + ammoInClip + " / " + ammoOutClip;
        }
    }
}
