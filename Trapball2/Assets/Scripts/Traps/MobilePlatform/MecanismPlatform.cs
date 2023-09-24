using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanismPlatform : MonoBehaviour
{
    public GameObject platformRotation;
    public GameObject stagnantWater;

    public float maxHeightScale; // Escala máxima en y del agua
    public float minHeightScale; // Escala mínima en y del agua
    public float maxRotation = 36f;
    public float minRotation = 24f;

    // Update is called once per frame
    void Update()
    {
        RotatePlatformBasedOnWaterLevel();
    }

    void RotatePlatformBasedOnWaterLevel()
    {
        float waterHeightScale = stagnantWater.transform.localScale.y;

        // Se calcula el porcentaje de la escala en y del agua entre minHeightScale y maxHeightScale
        float heightPercentage = Mathf.InverseLerp(minHeightScale, maxHeightScale, waterHeightScale);

        // Se calcula el ángulo de rotación deseado. El valor mínimo es 36 y el máximo es 24.
        float desiredRotation = Mathf.Lerp(maxRotation, minRotation, heightPercentage);

        Vector3 currentRotation = platformRotation.transform.localEulerAngles;
        platformRotation.transform.localEulerAngles = new Vector3(currentRotation.x, desiredRotation, currentRotation.z);
    }




}

