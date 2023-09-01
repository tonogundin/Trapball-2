using System.Collections;
using UnityEngine;

public class CheckMouseCursos : MonoBehaviour
{
    private bool isUsingMouse;

    private void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        // Detecta la entrada del ratón
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            if (!isUsingMouse)
            {
                ShowCursor();
                isUsingMouse = true;
            }
        }

        // Detecta la entrada del teclado
        if (Input.anyKeyDown)
        {
            HideCursor();
            isUsingMouse = false;
        }

        // Detecta la entrada del mando (puedes expandir esto para detectar más botones/axes si es necesario)
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            HideCursor();
            isUsingMouse = false;
        }
    }

    void HideCursor()
    {
        Cursor.visible = false;
    }

    void ShowCursor()
    {
        Cursor.visible = true;
    }
}
