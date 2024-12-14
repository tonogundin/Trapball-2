using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class MainMenuCanvas : MonoBehaviour
{

    public GameObject logoTrapBall;
    public GameObject instructions;
    public GameObject loading;
    public GameObject menu;

    private StateMainMenu state = StateMainMenu.INITIAL;
    private FMOD.Studio.EventInstance soundStart;

    private Button menuNewGame;
    private Button menuSettings;
    private Button menuExit;
    private GameObject selectButton;
    private GameObject antSelectButton;

    private const string newGame = "NewGame";
    private const string settings = "Settings";
    private const string exit = "Exit";
    public bool interact = true;

    // Start is called before the first frame update
    private void Awake()
    {
        menuNewGame = menu.transform.Find(newGame).GetComponent<Button>();
        menuSettings = menu.transform.Find(settings).GetComponent<Button>();
        menuExit = menu.transform.Find(exit).GetComponent<Button>();
        state = StateMainMenu.INITIAL;
        soundStart = FMODUtils.createInstance(FMODConstants.HUD.GAME_START);
        EventSystem.current.SetSelectedGameObject(menuNewGame.gameObject);
        selectButton = EventSystem.current.currentSelectedGameObject;
        antSelectButton = selectButton;
        FMODUtils.setPauseEventsFX(false);
        interact = true;
    }

    private void Start()
    {
        FMODUtils.getVolumeSettings();
    }

    void Update()
    {
        selectButton = EventSystem.current.currentSelectedGameObject;
        if (selectButton != antSelectButton)
        {
            if (selectButton != null && selectButton.GetComponent<Button>() != null)
            {
                antSelectButton = selectButton;
            } else
            {
                selectButton = antSelectButton;
                EventSystem.current.SetSelectedGameObject(selectButton);
            }   
        }
    }
    public void launchNewGame()
    {
        if (interact)
        {
            switch (state)
            {
                case StateMainMenu.INITIAL:
                    interact = false;
                    Navigation noNavigation = new Navigation
                    {
                        mode = Navigation.Mode.None
                    };
                    menuNewGame.navigation = noNavigation;
                    menuExit.navigation = noNavigation;
                    StartCoroutine(delayStepFinal());
                    FMODUtils.stopAllEvents();
                    soundStart.start();
                    break;
            }
        }
    }

    public void launchExitGame()
    {
        if (interact)
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
    public void OnPressButton(InputValue value)
    {
        if (!value.isPressed && selectButton != null && interact)
        {
            switch (selectButton.name)
            {
                case newGame:
                    launchNewGame();
                    break;
                case exit:
                    launchExitGame();
                    break;
            }
        }
    }

    public void OnDetectControllerOrKeyboard(InputValue value)
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }
    }
    public void OnMouseMove(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }

    IEnumerator delayStepFinal()
    {
        state = StateMainMenu.FINAL;
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadSceneAsync("Intro");
    }

    private enum StateMainMenu
    {
        INITIAL,
        FINAL
    }

}
