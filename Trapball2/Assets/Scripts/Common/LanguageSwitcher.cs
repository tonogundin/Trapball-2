using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    public void SwitchLanguage(string languageCode)
    {
        LocationManager.Instance.LoadLocalization(languageCode);
        UpdateAllLocalizedTexts();
    }

    private void UpdateAllLocalizedTexts()
    {
        var localizedTexts = FindObjectsByType<LocalizedText>(FindObjectsSortMode.None);
        foreach (var localizedText in localizedTexts)
        {
            localizedText.UpdateText();
        }
    }
}
