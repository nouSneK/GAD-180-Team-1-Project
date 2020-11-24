using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponController : MonoBehaviour
{
    public int maxNumberOfWeapons = 2;
    public int numberOfWeapons = 1;
    private int selectedWeapon;
    private int rayLayerMask = 1 << 14;

    public float pickUpRange = 5f;
    public float weaponDropForce = 10;
    public float weaponThrowForce = 30;
    public float weaponThrowTorque = 200;
    public float weaponpickUpTime = 1.5f;

    public GameObject startingWeapon;
    public GameObject hands;

    public Transform weaponPosition;

    public List<GameObject> weapons;

    public Text ammoCounter;

    private RaycastHit hit;

    private bool hasHands;

    void Start()
    {
        SetStartingWeapons();
    }

    void Update()
    {
        //Mouse scroll for weapon switching
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            SwitchWeapon(true);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SwitchWeapon(false);
        }

        Physics.SphereCast(transform.position, 1, transform.forward, out hit, pickUpRange, rayLayerMask);
        Debug.DrawLine(transform.position, hit.point);
        
        if (hit.collider != null && hit.collider.GetComponent<Weapon>() && Input.GetKeyDown(KeyCode.E))
        {
            if (weapons[selectedWeapon].tag != "Hands")
            {
                Invoke("PickUpTimer", weaponpickUpTime);
            }
            else
            {
                PickUpWeapon(hit.collider.gameObject);
            }
        }

        if (Input.GetMouseButtonDown(1) && weapons[selectedWeapon].tag != "Hands")
        {
            ThrowWeapon();
        }
    }

    public void SwitchWeapon(bool up)
    {
        Debug.Log("Switch Weapons");

        //Scroll up or down
        if (up)
        {
            if (selectedWeapon >= weapons.Count - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        else
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = weapons.Count - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        int i = 0;
        foreach (GameObject weapon in weapons)
        {
            if (weapon.GetComponent<Animator>())
            {
                weapon.GetComponent<Animator>().Rebind();
            }

            //Enable / Disable weapon
            if (i == selectedWeapon)
            {
                weapon.SetActive(true);

                if (weapon.GetComponent<Weapon>())
                {
                    weapon.GetComponent<Weapon>().UpdateAmmoCounter();
                }
                else
                {
                    ammoCounter.text = "";
                }
            }
            else
            {
                weapon.SetActive(false);
            }

            i++;
        }
    }

    private void PickUpTimer()
    {
        if (hit.collider != null && hit.collider.GetComponent<Weapon>() && Input.GetKey(KeyCode.E))
        {
            PickUpWeapon(hit.collider.gameObject);
        }
    }

    private void PickUpWeapon(GameObject weapon)
    {
        foreach (GameObject w in weapons)
        {
            if(w.GetComponent<Weapon>() && w.GetComponent<Weapon>().weaponID == weapon.GetComponent<Weapon>().weaponID && weapon.GetComponent<Weapon>().projectile)
            {
                while (weapon.GetComponent<Weapon>().ammoInClip > 0 && w.GetComponent<Weapon>().ammoOutClip < w.GetComponent<Weapon>().maxClipSize)
                {
                    w.GetComponent<Weapon>().ammoOutClip++;
                    weapon.GetComponent<Weapon>().ammoInClip--;
                }

                if (weapon.GetComponent<Weapon>().ammoOutClip > 0 && w.GetComponent<Weapon>().ammoOutClip < w.GetComponent<Weapon>().maxClipSize)
                {
                    w.GetComponent<Weapon>().ammoOutClip++;
                    weapon.GetComponent<Weapon>().ammoOutClip--;
                }

                if (weapon.GetComponent<Weapon>().ammoOutClip == 0 && weapon.GetComponent<Weapon>().ammoInClip == 0)
                {
                    weapon.SetActive(false);
                }

                w.GetComponent<Weapon>().UpdateAmmoCounter();

                return;
            }
        }

        Debug.Log("picked up " + weapon.name);

        weapon.transform.position = weaponPosition.position;
        weapon.transform.rotation = weaponPosition.rotation;
        weapon.transform.parent = weaponPosition.transform;

        DropWeapon(weapons[selectedWeapon]);

        weapons.Add(weapon);

        weapon.GetComponent<Weapon>().PickedUp(gameObject, ammoCounter);

        selectedWeapon = weapons.Count - 1;
    }

    private void DropWeapon(GameObject weapon)
    {
        Debug.Log("Dropped " + weapon.name);

        //Drop Weapon
        if (weapon.GetComponent<Weapon>())
        {
            weapon.GetComponent<Weapon>().Drop(-weaponPosition.up, weaponDropForce);

            weapons.Remove(weapon);
            weapon.transform.parent = null;
        }
        else if (weapon.tag == "Hands" && numberOfWeapons < maxNumberOfWeapons)
        {
            weapon.SetActive(false);

            hasHands = true;

            numberOfWeapons++;
        }
        else if (weapon.tag == "Hands" && numberOfWeapons == maxNumberOfWeapons)
        {
            weapons.Remove(weapon);
            weapon.transform.parent = null;

            Destroy(weapon);

            hasHands = false;
        }

        ammoCounter.text = "";
    }

    private void ThrowWeapon()
    {
        GameObject weapon = weapons[selectedWeapon];

        Debug.Log("Throw " + weapon.name);

        if (weapon.GetComponent<Weapon>())
        {
            RaycastHit rayHit;
            Physics.SphereCast(transform.position, 0.1f, transform.forward, out rayHit);
            Debug.DrawLine(transform.position, rayHit.point);

            if (weapon.GetComponent<Weapon>().weaponType != 2)
            {
                //weapon.GetComponent<Weapon>().Drop(-weaponPosition.up, weaponThrowForce);
                weapon.GetComponent<Weapon>().ThrowDirection(rayHit.point, weaponThrowForce, weaponThrowTorque);
            }
            else
            {
                weapon.GetComponent<Weapon>().ThrowDirection(rayHit.point, weaponThrowForce, weaponThrowTorque);
            }
            weapon.GetComponent<Weapon>().weaponThrown = true;

            weapons.Remove(weapon);
            weapon.transform.parent = null;
        }

        if (hasHands)
        {
            foreach (GameObject w in weapons)
            {
                if (w.tag == "Hands")
                {
                    w.SetActive(true);

                    weapons[weapons.IndexOf(w)].SetActive(true);
                    selectedWeapon = weapons.IndexOf(w);
                }
            }

            numberOfWeapons--;
        }
        else
        {
            GameObject newHands = Instantiate(hands, weaponPosition.position, weaponPosition.rotation);
            newHands.transform.parent = weaponPosition.transform;

            weapons.Add(newHands);

            selectedWeapon = weapons.Count - 1;
        }

        ammoCounter.text = "";

        hasHands = true;
    }

    private void SetStartingWeapons()
    {
        Debug.Log("Set Start Weapons");

        for (int i = 1; i <= maxNumberOfWeapons; i++)
        {
            if (i == 1 && startingWeapon)
            {
                GameObject weapon = Instantiate(startingWeapon, weaponPosition.position, weaponPosition.rotation);
                weapon.transform.parent = weaponPosition.transform;

                weapons.Add(weapon);
            }
            else if(!hasHands)
            {
                GameObject defultHands = Instantiate(hands, weaponPosition.position, weaponPosition.rotation);
                defultHands.transform.parent = weaponPosition.transform;

                weapons.Add(defultHands.gameObject);

                if (i != 1)
                {
                    defultHands.SetActive(false);
                }

                Debug.Log(gameObject.name + "Added Hand");

                hasHands = true;
            }
        }
    }
}
