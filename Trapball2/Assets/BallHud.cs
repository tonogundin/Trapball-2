using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallHud : MonoBehaviour
{
    private Image targetImage;
    public Sprite initialSprite;
    public Sprite damageSprite;
    public Color flashColor = Color.red;
    public float flashDuration = 0.5f;
    public int flashCount = 3;
    public float changeInterval = 5f;

    private Color originalColor;

    void Start()
    {
        targetImage = GetComponent<Image>();
        originalColor = targetImage.color;
        StartCoroutine(ChangeImageAndColorRoutine());
    }

    IEnumerator ChangeImageAndColorRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(FlashColor());

            targetImage.sprite = initialSprite;

            yield return new WaitForSeconds(changeInterval);
        }
    }

    IEnumerator FlashColor()
    {
        targetImage.sprite = damageSprite;
        float timer = 0;

        for (int i = 0; i < flashCount; i++)
        {
            while (timer < flashDuration)
            {
                // Cambia el color suavemente al flashColor
                targetImage.color = Color.Lerp(originalColor, flashColor, timer / flashDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;

            while (timer < flashDuration)
            {
                // Cambia el color suavemente al originalColor
                targetImage.color = Color.Lerp(flashColor, originalColor, timer / flashDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;
        }
    }
}


