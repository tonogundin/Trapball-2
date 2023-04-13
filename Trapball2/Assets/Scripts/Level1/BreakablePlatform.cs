using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    Rigidbody rb;
    FMOD.Studio.EventInstance PlatformCrack;
    FMOD.Studio.EventInstance PlatformHit;
    FMOD.Studio.EventInstance PlatformSplash;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        PlatformHit = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformHit");
        PlatformCrack = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformCrack");
        PlatformSplash = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformSplash");

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false;
            StartCoroutine(Disappear());
            PlatformHit.setParameterByName("VolPlat", 0);
            PlatformCrack.start();
          




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









    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(3f);
        while(transform.localScale.magnitude > 0.1f)
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
}
