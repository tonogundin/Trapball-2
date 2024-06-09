using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class Loading : MonoBehaviour
{

    private float velocity = 10f;
    private float rotation = 0.0f;
    public Image image;
    public TMP_Text tmp_text;
    private int indexLanguage;
    public static string[] LOADING = new string[]
     {
        "Loading",
        "Cargando",
        "Cargando"
     };

    private void Awake()
    {

    }
    private void Update()
    {
        rotation -= velocity;
        if (rotation < -360)
        {
            rotation = rotation + 360;
        }
        image.rectTransform.localRotation = Quaternion.Euler(0, 0, rotation);
    }
}