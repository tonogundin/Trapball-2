using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] GameObject[] gOButtons;
    void Start()
    {
        for (int i = 0; i < gOButtons.Length; i++)
        {
            int level = int.Parse(gOButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text); //Obtengo el texto del botón y lo parseo a entero.
            Button currentButton = gOButtons[i].GetComponent<Button>();
            currentButton.onClick.AddListener(() => LoadLevel(level));
            if (GameManager.gM.levelsPassed[i])
            {
                ColorBlock colorBlock = currentButton.colors;
                colorBlock.normalColor = new Color32(213, 180, 153, 255);
                currentButton.colors = colorBlock;
            }
        }
        
    }
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }


}
