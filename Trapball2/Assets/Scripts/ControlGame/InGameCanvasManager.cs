using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameCanvasManager : MonoBehaviour
{

    private void Awake()
    {
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
}
