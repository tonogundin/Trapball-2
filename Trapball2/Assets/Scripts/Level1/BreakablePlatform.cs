using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IResettable
{
    Rigidbody rb;
    FMOD.Studio.EventInstance PlatformCrack;
    FMOD.Studio.EventInstance PlatformHit;
    FMOD.Studio.EventInstance PlatformSplash;

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
            //PlatformHit.setParameterByName("VolPlat", 1);



        }

        if (collision.gameObject.CompareTag("SueloPiedra"))
        {

            PlatformHit.start();
            //PlatformHit.setParameterByName("VolPlat", 1);




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
        rb.isKinematic = false;
        StartCoroutine(Disappear());
        PlatformHit.setParameterByName("VolPlat", 0);
        PlatformCrack.start();
    }






    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(1f);
        while(transform.localScale.magnitude > 0.1f)
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.SetActive(false);
    }
}
