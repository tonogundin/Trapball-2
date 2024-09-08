using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Courage : MonoBehaviour
{

    private FMOD.Studio.EventInstance soundCourage;
    // Start is called before the first frame update
    void Start()
    {
        soundCourage = FMODUtils.createInstance(FMODConstants.OBJECTS.GRAB_MOON);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case Player.TAG:
                soundCourage.start();
                Destroy(gameObject);
                break;
        }
    }
}
