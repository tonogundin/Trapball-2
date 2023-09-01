using System.Collections;
using TMPro;
using UnityEngine;

public class TextBlink : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(BlinkText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator BlinkText()
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();

        while (true)
        {
            textComponent.enabled = !textComponent.enabled;
            yield return new WaitForSeconds(0.5f);  // Puedes ajustar este tiempo para controlar la velocidad del parpadeo.
        }
    }

}
