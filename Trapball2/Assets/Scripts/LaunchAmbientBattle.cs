using UnityEngine;
using System.Collections;

public class LaunchAmbientBattle : MonoBehaviour
{
    public bool launchOnStart = false;
    private bool isPlay = false;
    void Start()
    {
        if (launchOnStart)
        {
            StartCoroutine(activeSound());
        }
    }

    void Update()
    {

    }
    private IEnumerator activeSound()
    {
        yield return new WaitForSeconds(0.0001f);
        isPlay = true;
        FMODUtils.playOneShot(FMODConstants.AMBIENT.BATTLE, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!launchOnStart && !isPlay && other.gameObject.CompareTag(Player.TAG))
        {
            StartCoroutine(activeSound());
        }
    }
}
