using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public SCENE scene;
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
        FMODUtils.setPauseEventsFX(true);
        emitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.SetParameter(FMODConstants.STATE_MUSIC, (int) FMODConstants.MUSIC_STATE.FINAL);
        GameEvents.instance.finishGame.Invoke();
        SceneLoaderManager.prepareSceneLoad(scene);
        yield return new WaitForSeconds(5.5f);
        FMODUtils.stopAllEvents();
        SceneManager.LoadSceneAsync(FMODUtils.GetStringValue(SCENE.LOADING));
    }
}
