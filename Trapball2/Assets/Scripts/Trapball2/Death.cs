﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Daño/DeathVoice", GetComponent<Transform>().position);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Daño/ImpactoPinchos", GetComponent<Transform>().position);
            other.transform.position = GameManager.gM.initPosForPlayer;
         
        }
    }


}
