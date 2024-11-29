using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string locationKey;
    private TMP_Text textComponent;
    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
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
}
