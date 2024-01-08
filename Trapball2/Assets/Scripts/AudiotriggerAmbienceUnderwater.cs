using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiotriggerAmbienceUnderwater : MonoBehaviour
{

  //  FMOD.Studio.EventInstance enterWater;
    // Start is called before the first frame update
    void Start()
    {
   //     enterWater = FMODUnity.RuntimeManager.CreateInstance("event:/Ambientes/AmbienteUnderwater");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggeEnter(Collider other)
    {
     //   enterWater.start();
    }

    private void OnTriggerExit(Collider other)
    {
     //   enterWater.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
