using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audiotest2 : MonoBehaviour
{

    
    public string selectsound;
     public FMOD.Studio.EventInstance soundevent;

    public KeyCode presstoplaysound;


    void Start()
    {
        soundevent = FMODUnity.RuntimeManager.CreateInstance(selectsound);
    }

    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundevent, GetComponent<Transform>(), GetComponent<Rigidbody>());
        Playsound();
    }

    void Playsound()
    {
        if (Input.GetKey(presstoplaysound))
        {
            FMOD.Studio.PLAYBACK_STATE fmodPbState;
            soundevent.getPlaybackState(out fmodPbState);
            if (fmodPbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                soundevent.start();
            }
        }
        if (Input.GetKeyUp(presstoplaysound))
        {
            soundevent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}