using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] Material exitOnMat;
    [SerializeField] GameObject phaseCompletedMenu;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //gameObject.GetComponent<MeshRenderer>().material = exitOnMat;
            //Destroy(other.gameObject);
            //GameManager.gM.OnLevelCompleted();
            //phaseCompletedMenu.SetActive(true);

        }
    }
}
