using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class MainMenuCanvas : MonoBehaviour
{

    public GameObject LogoTrapBall;
    public GameObject Instructions;
    public GameObject Loading;

    private StateMainMenu state = StateMainMenu.INITIAL;
    private FMOD.Studio.EventInstance soundStart;
    // Start is called before the first frame update
    void Start()
    {
        state = StateMainMenu.INITIAL;
        soundStart = FMODUtils.createInstance(FMODConstants.HUD.GAME_START);
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
                case StateMainMenu.INITIAL:
                    StartCoroutine(delayStep());
                    LogoTrapBall.SetActive(false);
                    Instructions.SetActive(true);
                    break;
                case StateMainMenu.INSTRUCTIONS:
                    state = StateMainMenu.LOADING;
                    FMODUtils.stopAllEvents();
                    soundStart.start();
                    StartCoroutine(delayStepLoading());
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
        yield return new WaitForSeconds(2f);
        Instructions.SetActive(false);
        Loading.SetActive(true);
        state = StateMainMenu.FINAL;
        StartCoroutine(delayStepFinal());
    }
    IEnumerator delayStepFinal()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadSceneAsync("Level1_develop_newMusic");
    }

    private enum StateMainMenu
    {
        INITIAL,
        INSTRUCTIONS,
        LOADING,
        FINAL
    }

}
