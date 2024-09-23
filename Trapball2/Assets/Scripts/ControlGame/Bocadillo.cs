using System.Collections;
using UnityEngine;
using TMPro;

public class Bocadillo : MonoBehaviour
{
    public float growDuration = 2f;
    public bool isWithScale = true;

    private RectTransform rectTransform;

    // Referencia al CanvasGroup para objetos UI
    private CanvasGroup canvasGroup;

    // Duración de la animación de desaparición
    public float fadeDuration = 1.0f;

    // Tiempo antes de comenzar a desaparecer
    private float waitTimeBeforeFade = 10.0f;

    public TextMeshProUGUI textBocadillo;

    public Bocadillo otherBocadillo;
    public TypeDialog type;

    private FMOD.Studio.EventInstance dialogEight;

    private float typingSpeed = 0.03f;

    void Awake()
    {
        dialogEight = FMODUtils.createInstance(FMODConstants.HUD.VOICE_DIALOGS);
        gameObject.SetActive(false);
    }



    public void ActiveBocadillo(string text, float timeLife)
    {
        closed();
        text = text.Replace("\\n", "\n");
        textBocadillo.text = "";
        waitTimeBeforeFade = timeLife;
        setTypeDialog();
        dialogEight.start();
        // Si es un objeto UI, obtén el CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        // Si estás usando UI, guarda el tamaño original desde el RectTransform
        rectTransform = GetComponent<RectTransform>();

        // Asegura que el objeto sea completamente opaco al inicio
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1.0f;
        }
        // Establece el tamaño inicial a 0
        if (isWithScale)
        {
            transform.localScale = Vector3.zero;
        }
        
        gameObject.SetActive(true);
        otherBocadillo.closed();

        // Inicia la coroutine para crecer
        StartCoroutine(GrowFromZeroToOriginalSize());

        StartCoroutine(TypeText(text, typingSpeed));
    }

    public void closed()
    {
        if (this.isActiveAndEnabled)
        {
            dialogEight.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            StartCoroutine(FadeOutAction());
        }
    }

    private IEnumerator TypeText(string fullText, float typingSpeed)
    {
        yield return new WaitForSeconds(fadeDuration);
        
        string[] words = fullText.Split(' '); // Divide el texto en palabras
                                              // Recorre cada letra del texto
        foreach (char letter in fullText)
        {
            textBocadillo.text += letter; // Añade cada letra al texto del bocadillo
            yield return new WaitForSeconds(typingSpeed); // Espera el tiempo especificado entre cada letra
        }
        /*
        foreach (string word in words)
        {
            textBocadillo.text += word + " "; // Añade la palabra y un espacio
            yield return new WaitForSeconds(typingSpeed); // Espera el tiempo especificado entre cada palabra
        }
        */
    }

    private void setTypeDialog()
    {

        switch (type)
        {
            case TypeDialog.HELP:
                int selectedImage = Random.Range(0, 2);
                dialogEight.setParameterByNameWithLabel(FMODConstants.TYPE_DIALOG, selectedImage == 0 ? FMODConstants.TYPE_DIALOG_THINKING : FMODConstants.TYPE_DIALOG_SOLUTION);
                break;

            case TypeDialog.LORE:
                dialogEight.setParameterByNameWithLabel(FMODConstants.TYPE_DIALOG, FMODConstants.TYPE_DIALOG_WORRIED);
                break;
        }
    }

    IEnumerator GrowFromZeroToOriginalSize()
    {
        float currentTime = 0;

        // Asegura que el objeto empiece con escala 0 para crecer desde ahí.
        transform.localScale = Vector3.zero;

        while (currentTime < growDuration)
        {
            // Incrementa el tiempo actual
            currentTime += Time.deltaTime;

            // Interpola la escala desde 0 hasta 1 basado en el tiempo actual
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, currentTime / growDuration);

            yield return null;
        }

        // Asegura que la escala sea exactamente 1 (su tamaño original) al finalizar
        transform.localScale = Vector3.one;

        // Continúa con la siguiente etapa del proceso
        OnGrowthComplete();
    }

    // Llamado al finalizar el crecimiento para iniciar la desaparición
    void OnGrowthComplete()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Espera un tiempo específico antes de comenzar a desaparecer
        yield return new WaitForSeconds(waitTimeBeforeFade);
        StartCoroutine(FadeOutAction());
    }
    IEnumerator FadeOutAction()
    {
        float currentTime = 0;

        if (canvasGroup.alpha == 1)
        {
            while (currentTime < fadeDuration)
            {
                // Incrementa el tiempo actual
                currentTime += Time.deltaTime;

                // Ajusta el alpha del CanvasGroup de 1 a 0 para desaparecer
                canvasGroup.alpha = Mathf.Lerp(1, 0, currentTime / fadeDuration);

                yield return null;
            }

            // Opcional: Desactiva el GameObject al finalizar la animación de desaparición
            gameObject.SetActive(false);
        }
    }

    public enum TypeDialog
    {
        LORE,
        HELP
    }
 }
