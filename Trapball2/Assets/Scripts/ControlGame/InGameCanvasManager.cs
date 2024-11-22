using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameCanvasManager : MonoBehaviour
{

    public float fadeDuration = 3f; // Duración del fundido en segundos
    public float stayBlack = 1f;
    public float stayTransparent = 1f;
    public Image fundidoNegro;
    private void Awake()
    {
        // Establece el color inicial con alfa 0 (completamente transparente)
        fundidoNegro.color = new Color(0f, 0f, 0f, 0f);
        
    }
    private void Start()
    {
        StartCoroutine(fadeToTransparent());
        GameEvents.instance.finishGame.AddListener(runFadetoBlack);
    }
    public void OnPauseButtonClicked()
    {
        Time.timeScale = 0; //Time stops.
    }
    public void OnContinueButtonClicked()
    {
        Time.timeScale = 1;
    }
    public void OnExitButtonClicked()
    {
        Time.timeScale = 1; //Reinicio del tiempo debido a que se paró para hacer el pause.
        GameManager.gM.bombExploding = false; //Por si se ha quedado una bomba a medio explotar al salir del juego.
        SceneManager.LoadScene("LevelMenu");
    }
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("LevelMenu");
    }
    public void OnNextPhaseButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator fadeToTransparent()
    {
        // Establece el color inicial con alfa 1 (completamente opaco)
        fundidoNegro.color = new Color(0f, 0f, 0f, 1f);

        yield return new WaitForSeconds(stayBlack);
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
            float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);

            // Asigna el nuevo valor de alfa al color
            color.a = alpha;
            fundidoNegro.color = color;
            yield return null;
        }

        // Asegura que el alfa final sea 0 (completamente transparente)
        color.a = 0f;
        fundidoNegro.color = color;

        // Desactiva el objeto si es necesario
        fundidoNegro.enabled = false;
    }

    private void runFadetoBlack()
    {
        StartCoroutine(fadeToBlack());
    }

    private IEnumerator fadeToBlack()
    {
        fundidoNegro.enabled = true;
        // Establece el color inicial con alfa 0 (completamente transparente)
        fundidoNegro.color = new Color(0f, 0f, 0f, 0f);
        yield return new WaitForSeconds(stayTransparent);
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
            yield return null;
        }

        // Asegura que el alfa final sea 1 (completamente opaco)
        color.a = 1f;
        fundidoNegro.color = color;
    }
}
