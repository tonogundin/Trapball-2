using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode
{
    Is2D,
    OnPosition,
    OnGameObject,
}

public class FmodEmitter : MonoBehaviour
{
    [SerializeField] private EventReference m_event;
    private EventDescription m_description;
    [SerializeField] private bool m_instatiateOnAwake;
    private EventInstance m_instance;
    [SerializeField] private FMOD.Studio.STOP_MODE m_stopMode;
    [SerializeField] private PlayMode m_playmode;

    private void Awake()
    {
        m_description = RuntimeManager.GetEventDescription(m_event);

        if (m_instatiateOnAwake)
        {
            CreateInstances();
        }
    }

    private void CreateInstances()
    {
        m_instance = RuntimeManager.CreateInstance(m_event);
    }

    //Reproducir audio
    public void Play()
    {
        if (!m_instance.isValid())
        {
            //Just in time
            CreateInstances();
        }

        if (m_playmode == PlayMode.Is2D)
        {
            m_instance.start();
        }
        else if (m_playmode == PlayMode.OnPosition)
        {
            m_description.is3D(out bool is3D);
            if (!is3D)
            {
                Debug.LogError($"Este evento {m_event} es 2D!!");
                return;
            }

            m_instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            m_instance.start();
        }
        else
        {
            RuntimeManager.AttachInstanceToGameObject(m_instance, transform);
            m_instance.start();
        }
    }

    public void Stop()
    {
        if (m_instance.isValid())
        {
            m_instance.stop(m_stopMode);
        }
    }
}