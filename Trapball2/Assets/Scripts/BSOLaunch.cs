using UnityEngine;
using FMODUnity;

public class BSOLaunch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Player.TAG))
        {
            Camera.main.gameObject.GetComponent<StudioEventEmitter>().Play();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
