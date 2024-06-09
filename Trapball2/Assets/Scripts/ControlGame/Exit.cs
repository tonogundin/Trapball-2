using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public string scene;
    FMODUnity.StudioEventEmitter emitter;
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case Player.TAG:
                StartCoroutine(delayChangeScene());
                break;
        }
    }

    IEnumerator delayChangeScene()
    {
        emitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.SetParameter(FMODConstants.STATE_MUSIC, (int) FMODConstants.MUSIC_STATE.FINAL);
        yield return new WaitForSeconds(5.5f);
        FMODUtils.stopAllEvents();
        SceneManager.LoadSceneAsync(scene);
    }
}
