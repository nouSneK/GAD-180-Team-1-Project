using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float bulletSpeed = 10;
    public float bulletForce = 100;
    public float time = 1;

    public int bulletDamage = 1;

    public int pierceCount = 1;
    private bool gravity = false;

    public GameObject destroyFx;

    private void FixedUpdate()
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
            if (collision.collider.GetComponent<VentOpening>() && !collision.collider.GetComponent<VentOpening>().isOpened)
            {
                Debug.Log("Hit Vent");

                collision.collider.GetComponent<VentOpening>().Open(gameObject);

                ProjectileHit(collision.gameObject);
            }
            else if (collision.collider.GetComponent<PlayerHealth>())
            {
                collision.collider.GetComponent<PlayerHealth>().TakeDamage(bulletDamage);

                ProjectileHit(collision.gameObject);
            }
            else if (!gravity)
            {
                Debug.Log("Hit Collider");

                gravity = true;

                gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gravity)
        {
            if (other.GetComponent<RobotCollisionBox>() && pierceCount > 0)
            {
                other.GetComponent<RobotCollisionBox>().robotParent.GetComponent<RobotAI>().TakeDamage(bulletDamage);
                other.GetComponent<RobotCollisionBox>().BreakOff();

                Debug.Log("Hit Robot 2");

                ProjectileHit(other.gameObject);
            }
        }
    }

    void ProjectileHit(GameObject objectHit)
    {
        pierceCount--;

        if (objectHit.GetComponent<Rigidbody>())
        {
            objectHit.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.forward * bulletForce, transform.position);
        }

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
