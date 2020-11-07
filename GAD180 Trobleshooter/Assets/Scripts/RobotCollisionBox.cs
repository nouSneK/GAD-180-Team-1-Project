using UnityEngine;

public class RobotCollisionBox : MonoBehaviour
{
    public GameObject robotParent;

    public GameObject destroyFx;

    public void BreakOff()
    {
        if (gameObject.GetComponent<CharacterJoint>())
        {
            gameObject.GetComponent<CharacterJoint>().breakForce = 0;

            if (destroyFx && gameObject.GetComponent<CharacterJoint>())
            {
                GameObject fx = Instantiate(destroyFx, transform.TransformPoint(Vector3.zero), Quaternion.identity);

                fx.transform.parent = gameObject.transform;
                //fx.transform.position = gameObject.GetComponent<CharacterJoint>().connectedAnchor;
                fx.transform.localScale = new Vector3(1, 1, 1);
                fx.GetComponent<ParticleSystem>().Play();

                Destroy(fx, 1);
            }
        }
    }
}
