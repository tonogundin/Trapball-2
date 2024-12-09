using UnityEngine;

public class SelectLanguage : MonoBehaviour
{
    private Languages languagueSelected = Languages.ES;

    void Start()
    {
        updatePosition();
    }

    void Update()
    {
        
    }

    public void updatePosition()
    {
        languagueSelected = GameObject.Find("LocationManager").GetComponent<LanguageSwitcher>().languageSelected;
        Vector3 buttonPosition = GameObject.Find("Button" + languagueSelected.ToString()).transform.position;
        transform.position = buttonPosition;
    }
}
