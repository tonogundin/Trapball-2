using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IResettable
{
    Rigidbody rb;

    private Collider coll;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private FMOD.Studio.EventInstance impactFloor;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        initialScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        initialRotation = transform.rotation;
        impactFloor = FMODUtils.createInstance(FMODConstants.JUMPS.IMPACT_TERRAIN_ENEMIES);
    }

    public void resetObject()
    {
        gameObject.SetActive(true);
        rb.isKinematic = true;
        rb.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        transform.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
        transform.rotation = initialRotation;
        rb.rotation = initialRotation;
        coll.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isActiveAndEnabled)
        {
            if(collision.gameObject.CompareTag(Player.TAG))
            {
                FMODUtils.playOneShot(FMODConstants.OBJECTS.PLATFORM_BREAKING, transform.position);
                StartCoroutine(StarDisappear());
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (isActiveAndEnabled)
        {
            string tag = other.tag;
            switch (tag)
            {
                case "Water":
                    impactFloor.setParameterByName(FMODConstants.SPEED, 8);
                    FMODUtils.setTerrainParametersAndStart3D(impactFloor, FMODConstants.MATERIAL.WATER, transform);
                    break;
                default:
                    // Handle other cases or do nothing
                    break;
            }
        }
    }


    IEnumerator StarDisappear()
    {
        yield return new WaitForSeconds(1f);
        if (isActiveAndEnabled)
        {
            rb.isKinematic = false;
            yield return new WaitForSeconds(1f);
            int cicles = 15;
            while (transform.localScale.magnitude > 0.1f && cicles > 0)
            {
                transform.localScale -= new Vector3(0.15f, 0.15f, 0.15f);
                cicles--;
                if (cicles < 10)
                {
                    coll.enabled = false;
                }
                yield return new WaitForSeconds(0.05f);
            }
            gameObject.SetActive(false);
        }
    }

}
