using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    public GameObject menuPause;
    private Button menuPauseContinueButton;
    private Button menuPauseSettingsButton;
    private Button menuPauseExitButton;
    private Button menuPauseExitSettingsButton;
    public GameObject menuSettings;
    public PlayerInput playerInput;
    private Button[] buttons;

    private Button lastSelectedButton;


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
        menuPauseExitSettingsButton = menuSettings.transform.Find("ExitButton").GetComponent<Button>();
        buttons = new Button[] { menuPauseContinueButton, menuPauseSettingsButton, menuPauseExitButton };
        lastSelectedButton = menuPauseContinueButton;
    }

    private void Update()
    {
        // Verifica si no hay ningún botón seleccionado
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            lastSelectedButton.Select();
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && Time.timeScale == 0)
        {
            FMODUtils.setSnapshotPause(true);
        }
    }

    public void OnMouse(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnMouseMove(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }


    public void buttonContinue()
    {
        playerInput.enabled = false;
        menuPause.SetActive(false);
        Time.timeScale = 1;
        FMODUtils.setSnapshotPause(false);
        FMODUtils.setPauseEventsFX(false);
        menuPauseContinueButton.interactable = false;
        menuPauseContinueButton.interactable = true;
    }

    public void buttonExit()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        playerInput.enabled = false;
        FMODUtils.setSnapshotPause(false);
        SceneManager.LoadSceneAsync("Menu");
        menuPauseExitButton.interactable = false;
        menuPauseExitButton.interactable = true;
    }

    public void buttonSettings()
    {
        EventSystem.current.SetSelectedGameObject(menuPauseExitSettingsButton.gameObject);
        menuPause.SetActive(false);
        menuSettings.SetActive(true);
        menuPauseSettingsButton.interactable = false;
        menuPauseSettingsButton.interactable = true;
    }


    private void showMenuPause()
    {
        if (!menuPause.activeSelf && !menuSettings.activeSelf)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            EventSystem.current.SetSelectedGameObject(buttons[(int)MenuButtons.CONTINUE].gameObject);
            playerInput.enabled = true;
            FMODUtils.setSnapshotPause(true);
            FMODUtils.setPauseEventsFX(true);
            menuPause.SetActive(true);
            Time.timeScale = 0;
        } else if (menuSettings.activeSelf)
        {
            showReturnMenuPause();
        } else
        {
            buttonContinue();
        }
    }
    private void showReturnMenuPause()
    {
        if (!menuPause.activeSelf && !menuSettings.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(buttons[(int)MenuButtons.SETTINGS].gameObject);
            menuPause.SetActive(true);
        }
    }


    private enum MenuButtons
    {
        CONTINUE = 0,
        SETTINGS = 1,
        EXIT = 2
    }
}
