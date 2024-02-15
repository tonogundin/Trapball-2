using System.Collections;
using UnityEngine;
using TMPro;

public class Bocadillo : MonoBehaviour
{
    public float growDuration = 2f;

    private RectTransform rectTransform;

    // Referencia al CanvasGroup para objetos UI
    private CanvasGroup canvasGroup;

    // Duración de la animación de desaparición
    public float fadeDuration = 1.0f;

    // Tiempo antes de comenzar a desaparecer
    public float waitTimeBeforeFade = 10.0f;

    public TextMeshProUGUI textBocadillo;

    public void ActiveBocadillo(string text)
    {
        text = text.Replace("\\n", "\n");

        textBocadillo.text = text;
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
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        // Inicia la coroutine para crecer
        StartCoroutine(GrowFromZeroToOriginalSize());
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

        float currentTime = 0;

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
