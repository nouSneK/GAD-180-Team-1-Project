using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float bulletSpeed = 10;
    public float time = 1;

    public int bulletDamage = 1;

    public int pierceCount = 1;
    private bool gravity = false;

    public GameObject destroyFx;

    private void Update()
    {
        if (!gravity)
        {
            transform.Translate(Vector3.forward * bulletSpeed * time * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!gravity)
        {
            gravity = true;

            gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RobotCollisionBox>() && pierceCount > 0)
        {
            pierceCount--;

            other.GetComponent<RobotCollisionBox>().robotParent.GetComponent<RobotAI>().TakeDamage(bulletDamage);
            other.GetComponent<RobotCollisionBox>().BreakOff();

            Debug.Log("Hit Robot 2");

            if (destroyFx)
            {
                GameObject fx = Instantiate(destroyFx, transform.position, transform.rotation);
                Destroy(fx, 1);
            }

            if (pierceCount <= 0) 
            {
                Destroy(gameObject);
            }
        }
    }
}
