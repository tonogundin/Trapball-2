using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;



public class Loading : MonoBehaviour
{
    public GameObject image1;
    public GameObject image2;
    public TMP_Text tmp_text;
    private int indexLanguage;
    private bool animateActive = false;
    public Sprite[] sprites1;
    public Sprite[] sprites2;
    private Image imageSelected;
    private Sprite[] spritesSelected;
    public static string[] LOADING = new string[]
     {
        "Loading",
        "Cargando",
        "Cargando"
     };

    private void Awake()
    {
        int selectedImage = Random.Range(0, 2);

        image1.SetActive(selectedImage == 0);
        image2.SetActive(!image1.activeSelf);

        spritesSelected = selectedImage == 0 ? sprites1 : sprites2;
        imageSelected = selectedImage == 0 ? image1.GetComponent<Image>() : image2.GetComponent<Image>();
    }

    private void Start()
    {
        FMODUtils.getVolumeSettings();
        StartCoroutine(delayStepFinal());
    }

    private void Update()
    {
        if (!animateActive)
        {
            StartCoroutine(startAnimation());
        }
    }

    private IEnumerator startAnimation()
    {
        animateActive = true;
        yield return new WaitForSeconds(0.50f);
        imageSelected.sprite = spritesSelected[1];
        yield return new WaitForSeconds(0.50f);
        imageSelected.sprite = spritesSelected[0];
        animateActive = false;
    }

    IEnumerator delayStepFinal()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadSceneAsync(SceneLoaderManager.nextScene);
    }
}