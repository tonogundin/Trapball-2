using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallHud : MonoBehaviour
{
    private Image targetImage;
    public Sprite initialSprite;
    public Sprite damageSprite;
    public Sprite deadSprite;
    public Color flashColor = Color.red;
    public float flashDuration = 0.5f;
    public int flashCount = 3;
    public float changeInterval = 5f;
    private Color originalColor;
    public GameObject live;
    public GameObject energy;


    private Player player;
    private StatePlayer statePlayer;
    private Vector3 initialPosition;
    private Transform transform;
    private Image imageLive;
    private Image imageEnergy;

    public Sprite[] spritesLive;
    public Sprite[] spritesEnergy;

    private float limitEnergy = 0;

    void Start()
    {
        transform = GetComponent<Transform>();
        initialPosition = transform.position;
        targetImage = GetComponent<Image>();
        originalColor = targetImage.color;
        imageLive = live.GetComponent<Image>();
        imageEnergy = energy.GetComponent<Image>();

        //        StartCoroutine(ChangeImageAndColorRoutine());
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<Player>();
                statePlayer = player.state;
            }
        }
        if (player != null)
        {
            setPlayerStateImage(player.state);
            if (limitEnergy == 0)
            {
                limitEnergy = player.getJumpLimit();
            }
            setPlayerEnergy(player.getJumpForce());

        }

    }

    private void setPlayerEnergy(float jumpForce)
    {
        // Primero, normalizamos el valor de energía entre 0 y 1
        float normalizedEnergy = jumpForce / limitEnergy;

        // Luego, lo escalamos al rango de índices de nuestros gráficos (0 a 8)
        int spriteIndex = Mathf.RoundToInt(normalizedEnergy * (spritesEnergy.Length - 1));

        // Nos aseguramos de que el índice esté en el rango correcto
        spriteIndex = Mathf.Clamp(spriteIndex, 0, spritesEnergy.Length - 1);

        // Finalmente, establecemos el sprite de la barra de energía
        imageEnergy.sprite = spritesEnergy[spriteIndex];
    }

    private void setPlayerStateImage(StatePlayer state)
    {
        if (statePlayer != state)
        {
            switch (state)
            {
                case StatePlayer.NORMAL:
                    setHiddenLiveEnergy(true);
                    targetImage.color = originalColor;
                    targetImage.sprite = initialSprite;
                    transform.position = initialPosition;
                    break;

                case StatePlayer.JUMP:
                    setHiddenLiveEnergy(true);
                    break;
                case StatePlayer.BOMBJUMP:

                    break;
                case StatePlayer.DEAD:
                    setHiddenLiveEnergy(false);
                    targetImage.sprite = deadSprite;
                    transform.position = new Vector3(initialPosition.x, initialPosition.y - 20);
                    StartCoroutine(MoveDiagonally());
                    break;

            }
            statePlayer = state;
        }
    }


    private void setHiddenLiveEnergy(bool value)
    {
        live.SetActive(value);
        energy.SetActive(value);
    }


    IEnumerator MoveDiagonally()
    {
        // Definimos la dirección de movimiento y la distancia
        Vector3 moveDirection = new Vector3(1, 1, 0);
        float moveDistance = 100f;

        // Calculamos la posición final
        Vector3 finalPosition = transform.position + moveDirection * moveDistance;

        // Duración de la animación en segundos
        float animationDuration = 1f;

        // Guardamos la posición inicial
        Vector3 initialPosition = transform.position;

        Color initialColor = targetImage.color;
        Color finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0); // Alpha 0 para transparencia total

        float timer = 0;

        while (timer < animationDuration)
        {
            // Interpolamos entre la posición inicial y la final
            transform.position = Vector3.Lerp(initialPosition, finalPosition, timer / animationDuration);

            // Interpolamos entre el color inicial y el final
            targetImage.color = Color.Lerp(initialColor, finalColor, timer / animationDuration);

            // Avanzamos el tiempo
            timer += Time.deltaTime;

            // Esperamos hasta el próximo frame
            yield return null;
        }

        // Nos aseguramos de que la posición final es la correcta y que el color es el final
        transform.position = finalPosition;
        targetImage.color = finalColor;
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


