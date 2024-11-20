using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private bool pauseShake = false;
    // Start is called before the first frame update
    void Start()
    {

        GameEvents.instance.pauseScene.AddListener(setPauseShake);
        GameEvents.instance.resumeScene.AddListener(setDesPauseShake);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setPauseShake()
    {
        pauseShake = true;
    }
    private void setDesPauseShake()
    {
        pauseShake = false;
    }

    public IEnumerator Shake(float duration, float initialMagnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float timer = 0.0f;

        while (timer < duration)
        {
            if (!pauseShake)
            {
                // Calcular la magnitud actual que disminuye con el tiempo
                float currentMagnitude = Mathf.Lerp(initialMagnitude, 0f, timer / duration);

                // Generar desplazamientos aleatorios en x e y, reduciendo los valores por debajo de 1 y -1
                float x = Random.Range(-currentMagnitude, currentMagnitude);
                float y = Random.Range(-currentMagnitude, currentMagnitude);

                // Aplicar la nueva posición al objeto
                transform.localPosition = new Vector3(x, y, originalPos.z);

                // Incrementar el tiempo transcurrido
                timer += Time.deltaTime;
            }

            // Esperar al siguiente frame
            yield return null;
        }

        // Restaurar la posición original al final
        transform.localPosition = originalPos;
    }



}
