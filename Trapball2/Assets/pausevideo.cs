using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;


public class pausevideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private State state = State.PART_ZERO;
    public GameObject panel;
    public GameObject buttons;
    public GameObject buttonAPress;
    public GameObject buttonALoad;
    public Image fundidoNegro;
    private bool activeAnimationButton = false;

    public Image progressCircle; // El Image UI del círculo de progreso
    private float holdTime = 3f; // Tiempo necesario para mantener presionado el botón
    private bool isHolding = false;
    private float holdTimer = 0f;
    public float fadeDuration = 1f; // Duración del fundido en segundos
    public StudioEventEmitter musicEmitter; // El FMOD Studio Event Emitter para la música de fondo


    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        progressCircle.fillAmount = 0f;
        StartCoroutine(PauseAndPlayVideo());
    }    // Actualiza cada frame
    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            float fillAmount = (holdTimer / holdTime);
            Debug.Log("new Fill Amount: " + progressCircle.fillAmount);
            if (fillAmount > 0.2f)
            {
                if (!buttonALoad.activeSelf)
                {
                    buttonALoad.SetActive(true);
                } else
                {
                    progressCircle.fillAmount = fillAmount;
                }

            }
            if (holdTimer >= holdTime)
            {
                StartFade();
            }
        }
    }
    public void StartFade()
    {
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        // Obtener la instancia del evento FMOD
        EventInstance musicInstance = musicEmitter.EventInstance;
        float fmodInitialVolume;
        musicInstance.getVolume(out fmodInitialVolume);
        float initialVolume = (float)videoPlayer.GetDirectAudioVolume(0);
        fundidoNegro.color = new Color(0f, 0f, 0f, 0.01f);
        isHolding = false;
        Debug.Log("isHolding a false fadetoblack");
        holdTimer = 0f;
        Color color = fundidoNegro.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            color.a = alpha;
            fundidoNegro.color = color;
            videoPlayer.SetDirectAudioVolume(0, Mathf.Lerp(initialVolume, 0, alpha));
            musicInstance.setVolume(Mathf.Lerp(fmodInitialVolume, 0, alpha));

            yield return null;
        }

        // Asegurarse de que el alpha esté exactamente en 1 al final
        color.a = 1f;
        fundidoNegro.color = color;
        yield return new WaitForSeconds(1f);
        launchStage1();
    }
    IEnumerator PauseAndPlayVideo()
    {
        yield return new WaitForSeconds(getTimeForState());
        videoPlayer.Pause();
        buttons.SetActive(true);
        buttonAPress.SetActive(false);
        activeAnimationButton = true;
        StartCoroutine(PressButton());
        Debug.Log("Video pausado");
    }
    IEnumerator PressButton()
    {
        yield return new WaitForSeconds(0.5f);
        if (buttons.activeSelf)
        {
            buttonAPress.SetActive(!buttonAPress.activeSelf);
        }
        if (activeAnimationButton)
        {
            StartCoroutine(PressButton());
        }
    }
    public void OnJump(InputValue value)
    {
        if (!value.isPressed && videoPlayer.isPaused && !buttonALoad.activeSelf && fundidoNegro.color.a == 0)
        {
            videoPlayer.Play();
            switchPart();
            panel.SetActive(false);
            buttons.SetActive(false);
            activeAnimationButton = false;
            if (state != State.PART_FOUR)
            {
                StartCoroutine(PauseAndPlayVideo());
            } else
            {
                StartCoroutine(delayLaunchStage1());
            }
            Debug.Log("Video reanudado");
        }

        if (value.isPressed)
        {
            isHolding = true;
            holdTimer = 0f;
            progressCircle.fillAmount = 0f; // Reiniciar el círculo de progreso
        }
        else if (!value.isPressed)
        {
            isHolding = false;
            holdTimer = 0f;
            progressCircle.fillAmount = 0f; // Resetear el círculo de progreso si se suelta antes de tiempo
            buttonALoad.SetActive(false);
        }
    }
    IEnumerator delayLaunchStage1()
    {
        yield return new WaitForSeconds(18f);
        StartFade();
    }

    private void launchStage1()
    {
        SceneManager.LoadSceneAsync("Level1_develop_newMusic");
    }
    private void switchPart()
    {
        state = (State)(((int)state + 1) % Enum.GetValues(typeof(State)).Length);

    }

    private float getTimeForState()
    {
        switch (state)
        {
            case State.PART_ZERO: return 6.10f;
            case State.PART_ONE: return 34.05f;
            case State.PART_TWO: return 19.87f;
            case State.PART_THREE: return 14.07f;
            case State.PART_FOUR: return 14.05f;
            default: return 1f;
        }
    }

    enum State
    {
        PART_ZERO,
        PART_ONE,
        PART_TWO,
        PART_THREE,
        PART_FOUR
    }
}
