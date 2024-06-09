using FMOD;
using System;
using UnityEngine;
public class FMODUtils
{
    static FMOD.Studio.EventInstance snapshotInstance = FMODUnity.RuntimeManager.CreateInstance($"snapshot:/Pause");
    public static FMOD.Studio.EventInstance createInstance<T>(T sound) where T : Enum
    {
        string soundPath = GetStringValue(sound);
        return FMODUnity.RuntimeManager.CreateInstance(soundPath);
    }

    public static void playOneShot<T>(T sound, Vector3 position) where T : Enum
    {
        string soundPath = GetStringValue(sound);
        FMODUnity.RuntimeManager.PlayOneShot(soundPath, position);
    }

    public static void play3DSound(FMOD.Studio.EventInstance sound, Transform transform)
    {
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        sound.start();
    }

    public static void setTerrainParametersAndStart3D(FMOD.Studio.EventInstance sound, FMODConstants.MATERIAL material, Transform transform)
    {
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        sound.setParameterByName(FMODConstants.TERRAIN, (int)material);
        sound.start();
    }


    public static string GetStringValue<T>(T value) where T : Enum
    {
        Type type = value.GetType();
        var fieldInfo = type.GetField(value.ToString());
        StringValueAttribute[] attrs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
        return attrs.Length > 0 ? attrs[0].StringValue : null;
    }

    public static void setSnapshot(bool value)
    {
        if (value)
        {
            snapshotInstance.start();
        }
        else
        {
            snapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public static void stopAllEvents()
    {
        FMODUnity.RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public static void pauseAllEvents()
    {
        setPauseEvents(true);
    }

    public static void resumeAllEvents()
    {
        setPauseEvents(false);
    }

    private static void setPauseEvents(bool value)
    {
        FMODUnity.RuntimeManager.GetBus("bus:/").setPaused(value);
        
    }

    public static void setVolumenBankMaster(float value)
    {
        setVolumenBank(FMODConstants.BUSES.MASTER, value);
    }

    public static void setVolumenBankMusic(float value)
    {
        setVolumenBank(FMODConstants.BUSES.MUSIC, value);

    }

    private static void setVolumenBank(FMODConstants.BUSES bank, float value)
    {
        FMODUnity.RuntimeManager.GetBus(GetStringValue(bank)).setVolume(value);
    }


    public class StringValueAttribute : Attribute
    {
        public string StringValue { get; private set; }

        public StringValueAttribute(string stringValue)
        {
            StringValue = stringValue;
        }
    }



}