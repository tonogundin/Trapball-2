using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    public GameObject menuPause;
    public GameObject menuSettings;
    // Start is called before the first frame update
    void Awake()
    {
        GameEvents.instance.pauseScene.AddListener(showMenuPause);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonContinue()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        FMODUtils.resumeAllEvents();
    }

    public void buttonExit()
    {
        FMODUtils.stopAllEvents();
        SceneManager.LoadSceneAsync("Menu");
    }

    public void buttonSettings()
    {
        menuPause.SetActive(false);
        menuSettings.SetActive(true);
    }


    public void showMenuPause()
    {
        if (!menuPause.activeSelf && !menuSettings.activeSelf)
        {
            FMODUtils.pauseAllEvents();
            menuPause.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
