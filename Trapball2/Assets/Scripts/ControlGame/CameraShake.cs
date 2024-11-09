using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Shake(float duration, float initialMagnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float timer = 0.0f;

        while (timer < duration)
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

            // Esperar al siguiente frame
            yield return null;
        }

        // Restaurar la posición original al final
        transform.localPosition = originalPos;
    }



}
