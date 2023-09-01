using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextCourage : MonoBehaviour
{
    private Player player;
    private int courageValue = 0;
    public TextMeshProUGUI quantity;
    public TextMeshProUGUI percent;
    public Image image;
    public int totalCourages = 0;
    public Sprite[] moonSprites;

    // Start is called before the first frame update
    void Start()
    {
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
            float percentValue = courageValue * 100 / totalCourages;
            percent.text = percentValue + "%";
            quantity.text = "" + courageValue;
            SetMoonImage(percentValue);
        }
    }
    public void SetMoonImage(float percentage)
    {
        int index = Mathf.Clamp((int)((percentage / 100f) * 16), 0, 15);
        image.sprite = moonSprites[index];
    }
}
