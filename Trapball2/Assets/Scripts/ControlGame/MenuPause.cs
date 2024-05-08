using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    public GameObject menuPause;
    private Button menuPauseContinueButton;
    private Button menuPauseSettingsButton;
    private Button menuPauseExitButton;
    public GameObject menuSettings;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.pauseScene.AddListener(showMenuPause);
        GameEvents.instance.returnPauseScene.AddListener(showReturnMenuPause);
        var (musicVolume, fxVolume) = DataManager.Instance.LoadVolumeSettings();
        FMODUtils.setVolumenBankMaster(fxVolume);
        FMODUtils.setVolumenBankMusic(musicVolume);
        menuPauseContinueButton = menuPause.transform.Find("ContinueButton").GetComponent<Button>();
        menuPauseSettingsButton = menuPause.transform.Find("SettingsButton").GetComponent<Button>();
        menuPauseExitButton = menuPause.transform.Find("ExitButton").GetComponent<Button>();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && Time.timeScale == 0)
        {
            FMODUtils.setSnapshot(true);
        }
    }
    public void OnMouse(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }

    public void OnMouseMove(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }

    public void OnMove(InputValue value)
    {
        
    }
    public void OnPressButton(InputValue value)
    {

    }
    public void buttonContinue()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        FMODUtils.setSnapshot(false);
        menuPauseContinueButton.interactable = false;
        menuPauseContinueButton.interactable = true;
    }

    public void buttonExit()
    {
        FMODUtils.setSnapshot(true);
        SceneManager.LoadSceneAsync("Menu");
        menuPauseExitButton.interactable = false;
        menuPauseExitButton.interactable = true;
    }

    public void buttonSettings()
    {
        menuPause.SetActive(false);
        menuSettings.SetActive(true);
        menuPauseSettingsButton.interactable = false;
        menuPauseSettingsButton.interactable = true;
    }


    private void showMenuPause()
    {
        if (!menuPause.activeSelf && !menuSettings.activeSelf)
        {
            FMODUtils.setSnapshot(true);
            menuPause.SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void showReturnMenuPause()
    {
        if (!menuPause.activeSelf && !menuSettings.activeSelf)
        {
            menuPause.SetActive(true);
        }
    }
}
