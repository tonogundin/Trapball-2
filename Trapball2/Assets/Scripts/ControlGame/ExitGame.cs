using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void QuitGame()
    {
        // Cierra la aplicación
        Application.Quit();
        // Si estás en el editor de Unity, esto detendrá la ejecución del juego
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
