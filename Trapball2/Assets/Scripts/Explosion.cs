﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
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
            Player plScript = other.gameObject.GetComponent<Player>();
            plScript.Die();
        }
    }

    public void ExplosionEnded()
    {
        GameManager.gM.bombExploding = false;
        Destroy(gameObject);
    }

}
