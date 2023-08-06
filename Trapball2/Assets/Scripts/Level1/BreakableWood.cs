using System.Collections;
using UnityEngine;

public class BreakableWood : MonoBehaviour
{
    Rigidbody rb;
    FMOD.Studio.EventInstance PlatformCrack;
    FMOD.Studio.EventInstance PlatformHit;
    FMOD.Studio.EventInstance PlatformSplash;

    State state = State.NORMAL;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        PlatformHit = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformHit");
        PlatformCrack = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformCrack");
        PlatformSplash = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/PlatformSplash");

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") &&
            (collision.gameObject.GetComponent<Player>().state == StatePlayer.END_BOMB_JUMP ||
            collision.gameObject.GetComponent<Player>().state == StatePlayer.BOMBJUMP)
            &&
            state == State.NORMAL) {
            state = State.BREAK;
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
        if (collision.gameObject.CompareTag("Player") && state == State.BREAK)
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
        yield return new WaitForSeconds(0.5f);
        while(transform.localScale.y > 0.1f)
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }

    public enum State
    {
        NORMAL,
        BREAK
    }
}
