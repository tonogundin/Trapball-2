using UnityEngine;
using System.Collections;

public class HydraulicLift : MonoBehaviour
{
    public Transform piston; // El objeto cilíndrico que actúa como pistón.
    public float speed = 0.5f; // Velocidad de expansión y contracción.
    public float speedUp = 0.5f; // Velocidad de expansión y contracción.
    public float maxHeight = 2.54f; // Altura máxima del pistón.
    public float minHeight = 1f; // Altura mínima (inicial) del pistón.

    public bool isExpanding = true; // Para alternar entre expansión y contracción.
    public bool velocityNormal = true;
    public bool isDown = false;
    public bool isUp = false;
    public bool isHold = false;
    public float timeSecondsHold = 5f;

    private void Start()
    {
        piston = transform;
    }
    void Update()
    {
        float newYScale = piston.localScale.y;

        if (isExpanding)
        {
            newYScale += (speedUp * (velocityNormal ? 1 : 0.25f)) * Time.deltaTime;
            isDown = false;
            if (newYScale >= maxHeight)
            {
                newYScale = maxHeight;
                isUp = true;
            }
        }
        else
        {
            newYScale -= speed * Time.deltaTime;
            isUp = false;
            if (!isDown && newYScale <= minHeight)
            {
                newYScale = minHeight;
                isHold = true;
                StartCoroutine(delayHold());
            }
        }

        piston.localScale = new Vector3(piston.localScale.x, newYScale, piston.localScale.z);
    }
    public bool isNearDownHydraulic()
    {
        return !isExpanding && (piston.localScale.y <= minHeight + 0.65f);
    }

    public bool isMinUpHydraulic()
    {
        return isExpanding && (piston.localScale.y >= minHeight + 0.65f);
    }

    IEnumerator delayHold()
    {
        yield return new WaitForSeconds(timeSecondsHold);
        isHold = false;
        isDown = true;
    }
}

