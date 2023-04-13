using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCollision : MonoBehaviour
{
    // Start is called before the first frame update
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Daño/DeathVoice", GetComponent<Transform>().position);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Daño/ImpactoPinchos", GetComponent<Transform>().position);
            other.transform.position = GameManager.gM.initPosForPlayer;

        }
    }

}
