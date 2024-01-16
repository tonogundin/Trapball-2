using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IResettable
{
    Rigidbody rb;
    
    FMOD.Studio.EventInstance PlatformCrack;
    FMOD.Studio.EventInstance PlatformHit;
    FMOD.Studio.EventInstance PlatformSplash;
    

    public Collider collider;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        PlatformHit = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformHit");
        PlatformCrack = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformCrack");
        PlatformSplash = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformSplash");
        
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        initialScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        initialRotation = transform.rotation;
    }

    public void resetObject()
    {
        gameObject.SetActive(true);
        rb.isKinematic = true;
        rb.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        transform.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
        transform.rotation = initialRotation;
        rb.rotation = initialRotation;
        collider.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StarDisappear());

        }

        if (collision.gameObject.CompareTag("LugarDeCaida"))
        {

            PlatformHit.start();
            PlatformHit.setParameterByName("VolPlat", 1);



        }

        if (collision.gameObject.CompareTag("SueloPiedra"))
        {

            PlatformHit.start();
            PlatformHit.setParameterByName("VolPlat", 1);




        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlatformCrack.release();
        }

        if (collision.gameObject.CompareTag("LugarDeCaida"))
        {
            PlatformHit.release();
        }


    }


    IEnumerator StarDisappear()
    {
        yield return new WaitForSeconds(1f);
        if (this.isActiveAndEnabled)
        {
            rb.isKinematic = false;
            PlatformHit.setParameterByName("VolPlat", 0);
            PlatformCrack.start();
            yield return new WaitForSeconds(1f);
            int cicles = 15;
            while (transform.localScale.magnitude > 0.1f && cicles > 0)
            {
                transform.localScale -= new Vector3(0.15f, 0.15f, 0.15f);
                cicles--;
                if (cicles < 10)
                {
                    collider.enabled = false;
                }
                yield return new WaitForSeconds(0.05f);
            }
            gameObject.SetActive(false);
        }
    }

}
