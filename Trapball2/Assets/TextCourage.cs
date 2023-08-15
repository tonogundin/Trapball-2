using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextCourage : MonoBehaviour
{

    private Player player;
    private int courageValue = 0;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null && player == null)
            {
                player = playerObject.GetComponent<Player>();
            }
        }
        if (player != null)
        {
            courageValue = player.valor;
            text.text = "Valor: " + courageValue;
        }
    }
}
