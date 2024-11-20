using UnityEngine;
using System.Collections;

public class LaunchAmbientBattle : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(activeSound());
    }

    void Update()
    {

    }
    private IEnumerator activeSound()
    {
        yield return new WaitForSeconds(0.0001f);
        FMODUtils.playOneShot(FMODConstants.AMBIENT.BATTLE, transform.position);
    }
}
