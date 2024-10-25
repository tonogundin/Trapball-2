using UnityEngine;

public class Water : MonoBehaviour
{
    public const string TAG = "Water";
    FMOD.Studio.EventInstance enterWater;

    void Start()
    {
        enterWater = FMODUnity.RuntimeManager.CreateInstance("event:/Ambientes/AmbienteUnderwater");
        FMODUtils.setSnapshotUnderwater(false);
        GameEvents.instance.death.AddListener(exitWater);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Player.TAG))
        {
            enterWater.start();
            FMODUtils.setSnapshotUnderwater(true);
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Player.TAG))
        {
            exitWater();
        }
    }
    private void exitWater()
    {
        enterWater.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMODUtils.setSnapshotUnderwater(false);
    }
}
