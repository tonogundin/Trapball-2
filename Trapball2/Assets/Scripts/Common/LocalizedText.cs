using UnityEngine;
using TMPro;
using System.Collections;

public class LocalizedText : MonoBehaviour
{
    public string locationKey;
    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }
    private void Start()
    {
        UpdateText();
    }

    public void UpdateText(string newLocationKey)
    {
        locationKey = newLocationKey;
        UpdateText();
    }
    public void UpdateText()
    {
        if (textComponent != null && locationKey != null && locationKey.Length > 0)
        {
            textComponent.text = LocationManager.Instance.GetLocalizedValue(locationKey);
        }
    }

    public void UpdateTextTyping(string newLocationKey, float typingSpeed, float fadeDuration)
    {
        if (textComponent != null && newLocationKey != null && newLocationKey.Length > 0)
        {
            string text = LocationManager.Instance.GetLocalizedValue(newLocationKey);
            StartCoroutine(TypeText(text, typingSpeed, fadeDuration));
        }
    }
    private IEnumerator TypeText(string fullText, float typingSpeed, float fadeDuration)
    {
        textComponent.text = "";
        yield return new WaitForSeconds(fadeDuration);
        
        foreach (char letter in fullText)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    private IEnumerator TypeTextWord(string fullText, float typingSpeed, float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDuration);

        string[] words = fullText.Split(' ');

        foreach (string word in words)
        {
            textComponent.text += word + " ";
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
