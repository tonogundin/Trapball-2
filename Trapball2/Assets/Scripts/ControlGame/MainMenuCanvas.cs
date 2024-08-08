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

    // Start is called before the first frame update
    void Start()
    {
        menuNewGame = menu.transform.Find("NewGame").GetComponent<Button>();
        menuSettings = menu.transform.Find("Settings").GetComponent<Button>();
        state = StateMainMenu.INITIAL;
        soundStart = FMODUtils.createInstance(FMODConstants.HUD.GAME_START);
        EventSystem.current.SetSelectedGameObject(menuNewGame.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnJump(InputValue value)
    {
        if (!value.isPressed)
        {
            switch(state)
            {
                case StateMainMenu.INSTRUCTIONS:
                    state = StateMainMenu.LOADING;
                    FMODUtils.stopAllEvents();
                    soundStart.start();
                    StartCoroutine(delayStepLoading());
                    break;
            }

        }
    }

    public void selectNewGame()
    {
        StartCoroutine(delayStep());
        logoTrapBall.SetActive(false);
        instructions.SetActive(true);
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
        SceneManager.LoadSceneAsync("Intro");
    }

    private enum StateMainMenu
    {
        INITIAL,
        INSTRUCTIONS,
        LOADING,
        FINAL
    }

}
