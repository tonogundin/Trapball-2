using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class pausevideo : MonoBehaviour
{
    public GameObject panel;
    public GameObject buttons;
    public GameObject buttonAPress;
    public GameObject buttonALoad;
    public Image fundidoNegro;

    public Image progressCircle; // El Image UI del círculo de progreso
    private float holdTime = 2f; // Tiempo necesario para mantener presionado el botón
    private bool isHolding = false;
    private float holdTimer = 0f;
    private float fadeDuration = 2f; // Duración del fundido en segundos
    private float fadeOpaqueBackgroundDuration = 2.1f; // Duración del fundido en segundos
    private float fadeTranslucidBackgroundDuration = 1f; // Duración del fundido en segundos
    private Image colorBackgroundBlack;
    private Image colorBackgroundBlack2;
    private Image colorBackgroundYellow;
    private GameObject frontblack;
    private State state = State.STOP;
    private VideoPlayer videoPlayer;
    private TMP_Text textComic;
    private LocalizedText subtitles;
    public float[] timesSubtitles;
    public string keySubtitles;
    private AudioSource audioES;
    private AudioSource audioGAL;
    private AudioSource audioENG;
    private AudioSource audioCAT;

    private void Awake()
    {
        frontblack = GameObject.Find("frontblack");
        colorBackgroundBlack = transform.Find("PanelText").GetComponent<Image>();
        colorBackgroundBlack.color = new Color(colorBackgroundBlack.color.r, colorBackgroundBlack.color.g, colorBackgroundBlack.color.b, 0f);

        colorBackgroundBlack2 = transform.Find("PanelText").Find("PanelEnd").GetComponent<Image>();
        colorBackgroundBlack2.color = new Color(colorBackgroundBlack2.color.r, colorBackgroundBlack2.color.g, colorBackgroundBlack2.color.b, 0f);

        colorBackgroundYellow = transform.Find("PanelText").Find("ColorBackground").GetComponent<Image>();
        colorBackgroundYellow.color = new Color(colorBackgroundYellow.color.r, colorBackgroundYellow.color.g, colorBackgroundYellow.color.b, 0f);
        textComic = transform.Find("PanelText").Find("text").GetComponent<TMP_Text>();
        subtitles = transform.Find("Subtitle").GetComponent<LocalizedText>();
        textComic.color = new Color(textComic.color.r, textComic.color.g, textComic.color.b, 0f);
        videoPlayer = GetComponent<VideoPlayer>();
        audioES = GetComponents<AudioSource>()[0];
        audioENG = GetComponents<AudioSource>()[1];
        videoPlayer.Stop();
        videoPlayer.time = 0;
        audioES.Stop();
        audioENG.Stop();
    }
    void Start()
    {
        var (musicVolume, fxVolume) = FMODUtils.getVolumeSettings();
        videoPlayer.Play();
        if (LocationManager.Instance.currentLanguage == Languages.ENG)
        {
            audioENG.volume = musicVolume;
            audioENG.Play();
        } else
        {
            audioES.volume = musicVolume;
            audioES.Play();
        }
        progressCircle.fillAmount = 0f;
        StartCoroutine(quitblackfront());
    }  


    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            float fillAmount = (holdTimer / holdTime);
            if (fillAmount > 0.05f)
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
                StartCoroutine(fadeToBlack());
            }
        }
        switch(state)
        {
            case State.STOP:
                state = State.PLAYING;
                StartCoroutine(playingVideo());
                StartCoroutine(subtitlesThread());
                return;
            case State.FINISH:
                launchLoading();
                return;
        }
    }

    IEnumerator subtitlesThread()
    {
        int index = 0;
        foreach (float time in timesSubtitles)
        {
            subtitles.UpdateText(keySubtitles + "_line_" + index);
            index++;
            yield return new WaitForSeconds(time);
        }
    }

    IEnumerator quitblackfront()
    {
        yield return new WaitForSeconds(0.2f);
        frontblack.SetActive(false);
    }
    IEnumerator playingVideo()
    {
        StartCoroutine(fadeToOpaque());
        StartCoroutine(fadeToTranslucid());
        yield return new WaitForSeconds(105f);
        state = State.FINISH;
    }
    private IEnumerator fadeToOpaque()
    {
        yield return new WaitForSeconds(1.2f);
        float elapsedTime = 0f;
        Color startColorBlack = colorBackgroundBlack.color;
        Color startColorBlack2 = colorBackgroundBlack2.color;
        Color startColorYellow = colorBackgroundYellow.color;
        Color startColorText = textComic.color;
        Color targetColorBlack = new Color(startColorBlack.r, startColorBlack.g, startColorBlack.b, 1f); // Color opaco
        Color targetColorBlack2 = new Color(startColorBlack2.r, startColorBlack2.g, startColorBlack2.b, 1f); // Color opaco
        Color targetColorYellow = new Color(startColorYellow.r, startColorYellow.g, startColorYellow.b, 1f); // Color opaco
        Color targetColorText = new Color(startColorText.r, startColorText.g, startColorText.b, 1f); // Color opaco

        while (elapsedTime < fadeOpaqueBackgroundDuration)
        {
            elapsedTime += Time.deltaTime;
            colorBackgroundBlack.color = Color.Lerp(startColorBlack, targetColorBlack, elapsedTime / fadeOpaqueBackgroundDuration);
            colorBackgroundBlack2.color = Color.Lerp(startColorBlack2, targetColorBlack2, elapsedTime / fadeOpaqueBackgroundDuration);
            colorBackgroundYellow.color = Color.Lerp(startColorYellow, targetColorYellow, elapsedTime / fadeOpaqueBackgroundDuration);
            textComic.color = Color.Lerp(startColorText, targetColorText, elapsedTime / fadeOpaqueBackgroundDuration);
            yield return null;
        }
        colorBackgroundBlack.color = targetColorBlack;
        colorBackgroundBlack2.color = targetColorBlack2;
        colorBackgroundYellow.color = targetColorYellow;
        textComic.color = targetColorText;
    }
    private IEnumerator fadeToTranslucid()
    {
        yield return new WaitForSeconds(6.5f);
        float elapsedTime = 0f;
        Color startColorBlack = colorBackgroundBlack.color;
        Color startColorBlack2 = colorBackgroundBlack2.color;
        Color startColorYellow = colorBackgroundYellow.color;
        Color startColorText = textComic.color;
        Color targetColorBlack = new Color(startColorBlack.r, startColorBlack.g, startColorBlack.b, 0f);
        Color targetColorBlack2 = new Color(startColorBlack2.r, startColorBlack2.g, startColorBlack2.b, 0f);
        Color targetColorYellow = new Color(startColorYellow.r, startColorYellow.g, startColorYellow.b, 0f);
        Color targetColorText = new Color(startColorText.r, startColorText.g, startColorText.b, 0f);

        while (elapsedTime < fadeTranslucidBackgroundDuration)
        {
            elapsedTime += Time.deltaTime;
            colorBackgroundBlack.color = Color.Lerp(startColorBlack, targetColorBlack, elapsedTime / fadeTranslucidBackgroundDuration);
            colorBackgroundBlack2.color = Color.Lerp(startColorBlack2, targetColorBlack2, elapsedTime / fadeTranslucidBackgroundDuration);
            colorBackgroundYellow.color = Color.Lerp(startColorYellow, targetColorYellow, elapsedTime / fadeTranslucidBackgroundDuration);
            textComic.color = Color.Lerp(startColorText, targetColorText, elapsedTime / fadeTranslucidBackgroundDuration);
            yield return null;
        }
        colorBackgroundBlack.color = targetColorBlack;
        colorBackgroundBlack2.color = targetColorBlack2;
        colorBackgroundYellow.color = targetColorYellow;
        textComic.color = targetColorText;
        panel.SetActive(false);
    }
    private IEnumerator fadeToBlack()
    {
        state = State.GO_LOADING;

        // Obtén el componente AudioSource del objeto que lo contiene
        AudioSource audioSource = GetComponent<AudioSource>();

        // Asegúrate de que el AudioSource no sea nulo
        if (audioSource == null)
        {
            Debug.LogError("No se encontró el AudioSource en el objeto.");
            yield break; // Detén la ejecución si no hay un AudioSource
        }

        // Obtén el volumen inicial del AudioSource
        float initialVolume = audioSource.volume;

        // Establece el color inicial con alfa 0 (completamente transparente)
        fundidoNegro.color = new Color(0f, 0f, 0f, 0f);

        // Reinicia los temporizadores
        isHolding = false;
        holdTimer = 0f;

        // Copia el color actual
        Color color = fundidoNegro.color;

        // Inicia el temporizador
        float elapsedTime = 0f;

        // Mientras el tiempo transcurrido sea menor a la duración del fade
        while (elapsedTime < fadeDuration)
        {
            // Incrementa el tiempo transcurrido por el deltaTime (tiempo por frame)
            elapsedTime += Time.deltaTime;

            // Calcula el nuevo alfa basado en el tiempo transcurrido
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Asigna el nuevo valor de alfa al color
            color.a = alpha;
            fundidoNegro.color = color;

            // Ajusta el volumen del AudioSource de manera proporcional
            audioSource.volume = Mathf.Lerp(initialVolume, 0, alpha);

            // Espera al siguiente frame antes de continuar
            yield return null;
        }

        // Asegura que el alfa final sea 1 (completamente opaco)
        color.a = 1f;
        fundidoNegro.color = color;
        yield return new WaitForSeconds(0.5f);
        // Llama a la función para continuar el proceso después del fade
        launchLoading();
    }


    public void OnPressButton(InputValue value)
    {
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

    private void launchLoading()
    {
        state = State.GO_LOADING;
        videoPlayer.Stop();
        videoPlayer.time = 0;
        frontblack.SetActive(true);
        SceneManager.LoadSceneAsync("Loading");
        state = State.STOP;
        audioES.Stop();
        audioENG.Stop();
    }

    enum State
    {
        STOP,
        PLAYING,
        FINISH,
        GO_LOADING
    }
}
