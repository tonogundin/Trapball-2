using UnityEngine;
using System.Collections;

public class OvalainLaugh : MonoBehaviour
{
    public bool launchOnStart = false;
    private bool isPlay = false;
    void Awake()
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
        yield return null;
        isPlay = true;
        FMODUtils.playOneShot(FMODConstants.AMBIENT.OVALAIN_LAUGH);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!launchOnStart && !isPlay && other.gameObject.CompareTag(Player.TAG))
        {
            StartCoroutine(activeSound());
        }
    }
}
