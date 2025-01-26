using UnityEngine;

public class DinamicWater : MonoBehaviour
{
    public StateWaterDinamic state = StateWaterDinamic.Download;
    public float maxUpload = 2f;
    public float maxDownload = 1f;

    public float speed = 0.5f;

    private float currentScale;
    public GameObject waterBase;


    void Start()
    {
        currentScale = transform.localScale.y;
    }

    void Update()
    {
        switch (state)
        {
            case StateWaterDinamic.Upload:
                if (currentScale < maxUpload)
                {
                    currentScale += speed * Time.deltaTime;
                }
                if (currentScale > maxDownload + 0.15f)
                {
                    waterBase.SetActive(false);
                }
                break;
            case StateWaterDinamic.Download:
                if (currentScale > maxDownload)
                {
                    currentScale -= speed * Time.deltaTime;
                    if (currentScale <= maxDownload)
                    {
                        gameObject.SetActive(false);
                    }
                    if (currentScale <= maxDownload + 0.15f)
                    {
                        waterBase.SetActive(true);
                    }
                }

                break;
        }
        // Asegurarse de que la escala está en el rango permitido
        currentScale = Mathf.Clamp(currentScale, maxDownload, maxUpload);

        // Aplicar la nueva escala
        transform.localScale = new Vector3(transform.localScale.x, currentScale, transform.localScale.z);
    }

}

public enum StateWaterDinamic
{
    Upload,
    Download,
    Normal
}
