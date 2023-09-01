using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "Player":
                StartCoroutine(delayChangeScene());
                break;
        }
    }

    IEnumerator delayChangeScene()
    {
        yield return new WaitForSeconds(0.5f);
        FMOD.Studio.Bus masterBus;
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadSceneAsync("Menu");
    }
}
