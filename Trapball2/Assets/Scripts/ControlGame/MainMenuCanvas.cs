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
    }
    void Start()
    {

    }

    // Update is called once per frame
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
    public void OnJump(InputValue value)
    {
        if (!value.isPressed && selectButton != null)
        {
            switch (selectButton.name)
            {
                case newGame:
                    switch (state)
                    {
                        case StateMainMenu.INITIAL:
                            StartCoroutine(delayStep());
                            logoTrapBall.SetActive(false);
                            instructions.SetActive(true);
                            break;
                        case StateMainMenu.INSTRUCTIONS:
                            state = StateMainMenu.LOADING;
                            FMODUtils.stopAllEvents();
                            soundStart.start();
                            StartCoroutine(delayStepLoading());
                            break;
                    }
                    break;
                case exit:
                    // Cierra la aplicación
                    Application.Quit();
                    // Si estás en el editor de Unity, esto detendrá la ejecución del juego
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
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
    public void OnDetectMouse(InputValue value)
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }
    IEnumerator delayStep()
    {
        yield return new WaitForSeconds(0.5f);
        state = StateMainMenu.INSTRUCTIONS;
    }

    IEnumerator delayStepLoading()
    {
        yield return new WaitForSeconds(3f);
        instructions.SetActive(false);
        loading.SetActive(true);
        state = StateMainMenu.FINAL;
        StartCoroutine(delayStepFinal());
    }
    IEnumerator delayStepFinal()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadSceneAsync("Level1");
    }

    private enum StateMainMenu
    {
        INITIAL,
        INSTRUCTIONS,
        LOADING,
        FINAL
    }

}
