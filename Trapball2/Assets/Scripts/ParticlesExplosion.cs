using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesExplosion : MonoBehaviour, IResettable
{
    Light redLight;
    GameObject lightgO;
    private void Awake()
    {

    }
    public void Explode()
    {
        transform.SetParent(null);
        GetComponent<ParticleSystem>().Play();
    }

    public void resetObject()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
        GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
