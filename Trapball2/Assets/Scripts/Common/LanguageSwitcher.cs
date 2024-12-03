using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    private void Start()
    {
        string language = PlayerPrefs.GetString("language");
        SwitchLanguage(convertStrintToLanguage(language));
    }
    public void SwitchLanguage(Languages languageCode)
    {
        PlayerPrefs.SetString("language", languageCode.ToString());
        PlayerPrefs.Save();
        LocationManager.Instance.LoadLocalization(languageCode);
        UpdateAllLocalizedTexts();
    }

    public void buttonENG()
    {
        SwitchLanguage(Languages.ENG);
    }
    public void buttonES()
    {
        SwitchLanguage(Languages.ES);
    }
    public void buttonGAL()
    {
        SwitchLanguage(Languages.GAL);
    }
    public void buttonCAT()
    {
        SwitchLanguage(Languages.CAT);
    }

    private void UpdateAllLocalizedTexts()
    {
        var localizedTexts = FindObjectsByType<LocalizedText>(FindObjectsSortMode.None);
        foreach (var localizedText in localizedTexts)
        {
            localizedText.UpdateText();
        }
    }

    private Languages convertStrintToLanguage(string language)
    {
        if (System.Enum.TryParse(language, true, out Languages result))
        {
            return result;
        }
        else
        {
            Debug.LogWarning($"Language '{language}' not recognized. Defaulting to English.");
            return Languages.ES; // Valor predeterminado
        }
    }
}
