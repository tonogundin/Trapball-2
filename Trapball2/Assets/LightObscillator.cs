using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightObscillator : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [SerializeField] private float minLightIntensity = 0.5f;
    [SerializeField] private float maxLightIntensity = 2.5f;
    [SerializeField] private float minFlickerDuration = 0.3f; // 300ms
    [SerializeField] private float maxFlickerDuration = 0.5f; // 500ms
    [SerializeField] private float randomFlickerAmount = 0.4f; // Variabilidad extra

    private List<Light> _lights = new();

    private void Awake()
    {
        CacheChildLights();
        StartFlicker();
    }

    private void CacheChildLights()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Light lightComponent))
            {
                _lights.Add(lightComponent);
            }
        }
    }

    private void StartFlicker()
    {
        StartCoroutine(FlickerLights());
    }

    private IEnumerator FlickerLights()
    {
        while (true)
        {
            float duration = Random.Range(minFlickerDuration, maxFlickerDuration);
            float targetIntensity = Random.Range(minLightIntensity, maxLightIntensity);

            // Añadir una variación aleatoria al target
            targetIntensity += Random.Range(-randomFlickerAmount, randomFlickerAmount);
            targetIntensity = Mathf.Clamp(targetIntensity, minLightIntensity, maxLightIntensity);

            // Guardar los valores iniciales de cada luz
            Dictionary<Light, float> initialIntensities = new();
            foreach (var light in _lights)
            {
                initialIntensities[light] = light.intensity;
            }

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                foreach (var light in _lights)
                {
                    if (light != null)
                    {
                        light.intensity = Mathf.Lerp(initialIntensities[light], targetIntensity, elapsedTime / duration);
                    }
                }
                yield return null;
            }

            // Esperar un breve tiempo antes del siguiente cambio para hacerlo más natural
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }

}