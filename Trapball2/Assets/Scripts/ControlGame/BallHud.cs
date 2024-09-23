using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class BallHud : MonoBehaviour
{
    private Image targetImage;
    public Sprite initialSprite;
    public Sprite damageSprite;
    public Sprite limitLiveSprite;
    public Sprite deadSprite;
    public Sprite attackSprite;
    public Color flashColor = Color.red;
    public float flashDuration = 0.5f;
    public int flashCount = 1;
    private Color originalColor;
    public GameObject live;
    public GameObject energy;


    private Player player;
    private StatePlayer statePlayer;
    private Vector3 initialPosition;
    private Image imageLive;
    private Image imageEnergy;

    public Sprite[] spritesLive;
    public Sprite[] spritesEnergy;

    private float limitEnergy = 0;
    private float lowLimitEnergy = 0;


    private int limitLive = 0;
    private int limitMaxLive = 0;
    private int bufferLive = 8;

    private bool damage = false;
    Coroutine myCoroutineDamage;
    private bool deadAnimation = false;
    private int bufferEnergyIndex = -1;
    private int bufferLiveIndex = -1;

    public GameController gameController;
    private FMOD.Studio.EventInstance soundChargeEnergy;

    void Start()
    {
        initialPosition = transform.position;
        targetImage = GetComponent<Image>();
        originalColor = targetImage.color;
        imageLive = live.GetComponent<Image>();
        imageEnergy = energy.GetComponent<Image>();
        soundChargeEnergy = FMODUtils.createInstance(FMODConstants.JUMPS.CHARGE_JUMP);
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag(Player.TAG);
            if (playerObject != null)
            {
                player = playerObject.GetComponent<Player>();
                statePlayer = player.state;
                limitMaxLive = player.live;
                bufferLive = limitMaxLive;
            }
        }
        if (player != null)
        {
            setPlayerStateImage(player.state, player.isJumpBombEnabled());
            if (limitEnergy == 0)
            {
                limitEnergy = player.getJumpLimit();
                lowLimitEnergy = player.getJumpLowLimit() - (limitEnergy * 0.025f);
            }
            setPlayerEnergy(player.getJumpForce());
            setPlayerLive(player.live);

        }

    }

    private void setPlayerEnergy(float jumpForce)
    {
        // Primero, normalizamos el valor de energía entre 0 y 1
        float normalizedEnergy = (jumpForce - lowLimitEnergy) / (limitEnergy - lowLimitEnergy);

        // Luego, lo escalamos al rango de índices de nuestros gráficos (0 a 8)
        int spriteIndex = Mathf.RoundToInt(normalizedEnergy * (spritesEnergy.Length - 1));

        // Nos aseguramos de que el índice esté en el rango correcto
        spriteIndex = Mathf.Clamp(spriteIndex, 0, spritesEnergy.Length - 1);

        if (bufferEnergyIndex != spriteIndex)
        {
            if (spriteIndex == 0)
            {
                soundChargeEnergy.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else if (bufferEnergyIndex == 0)
            {
                soundChargeEnergy.setParameterByName(FMODConstants.JUMP_CHARGE, 25);
                soundChargeEnergy.start();
            }
            bufferEnergyIndex = spriteIndex;
            imageEnergy.sprite = spritesEnergy[spriteIndex];
            if (player.isRumbleActive)
            {
                gameController.ApplyRumble(gameController.NormalizeValue(jumpForce, lowLimitEnergy, limitEnergy), 0.05f);
            }
        }
        float normalizedSoundEnergy = Mathf.Clamp((jumpForce - lowLimitEnergy) / (limitEnergy - lowLimitEnergy), 0f, 1f);

        // Escalamos el valor para estar en el rango de 25 a 100
        float scaledSoundEnergy = 0f + (normalizedSoundEnergy * 100f);
        //Debug.Log("SpeedSound: " + scaledSoundEnergy);
        soundChargeEnergy.setParameterByName(FMODConstants.JUMP_CHARGE, scaledSoundEnergy);
    }
    private void setPlayerLive(float live)
    {
        // Primero, normalizamos el valor de energía entre 0 y 1
        float normalizedLive = 1 - ((live - limitLive) / (limitMaxLive - limitLive));

        // Luego, lo escalamos al rango de índices de nuestros gráficos (0 a 8)
        int spriteIndex = Mathf.RoundToInt(normalizedLive * (spritesLive.Length - 1));

        // Nos aseguramos de que el índice esté en el rango correcto
        spriteIndex = Mathf.Clamp(spriteIndex, 0, spritesLive.Length - 1);
        if (bufferLiveIndex != spriteIndex)
        {
            bufferLiveIndex = spriteIndex;
            imageLive.sprite = spritesLive[spriteIndex];
        }
    }

    private void setPlayerStateImage(StatePlayer state, bool jumpBombEnabled)
    {
        bool damagePlayer = state != StatePlayer.DEAD && bufferLive > player.live && player.live > 0;
        if (player.live == limitMaxLive)
        {

            bufferLive = limitMaxLive;
        }
        if (state == StatePlayer.DEAD)
        {
            if (myCoroutineDamage != null)
            {
                StopCoroutine(myCoroutineDamage);
            }
            damage = false;
        }
        if (damagePlayer) {
            bufferLive = player.live;
            damage = true;
            setImageDamage();
        } else if (statePlayer != state && !damage && !deadAnimation)
        {
            switch (state)
            {
                case StatePlayer.NORMAL:
                    setStateNormal();
                    break;
                case StatePlayer.JUMP:
                    setHiddenLiveEnergy(true);
                    if (jumpBombEnabled)
                    {
                        targetImage.color = originalColor;
                        targetImage.sprite = attackSprite;
                    }
                    break;
                case StatePlayer.END_BOMB_JUMP:
                case StatePlayer.BOMBJUMP:
                    targetImage.color = originalColor;
                    targetImage.sprite = attackSprite;
                    break;
                case StatePlayer.DEAD:
                    setHiddenLiveEnergy(false);
                    targetImage.color = originalColor;
                    targetImage.sprite = deadSprite;
                    transform.position = new Vector3(initialPosition.x, initialPosition.y - 20);
                    StartCoroutine(MoveDiagonally());
                    break;

            }
            statePlayer = state;
        }
    }

    private Sprite imageDependLive()
    {
        if (player != null && player.live < 3)
        {
            return limitLiveSprite;
        } else
        {
            return initialSprite;
        }   
    }

    private void setStateNormal()
    {
        setHiddenLiveEnergy(true);
        targetImage.color = originalColor;
        targetImage.sprite = imageDependLive();
        transform.position = initialPosition;
    }

    private void setHiddenLiveEnergy(bool value)
    {
        live.SetActive(value);
        energy.SetActive(value);
    }

    IEnumerator MoveDiagonally()
    {
        deadAnimation = true;
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
        deadAnimation = false;
    }



    private void setImageDamage()
    {
        targetImage.sprite = damageSprite;
        myCoroutineDamage = StartCoroutine(FlashColor());
    }

    IEnumerator FlashColor()
    {
        float timer = 0;
        foreach (int i in Enumerable.Range(0, flashCount))
        {
            timer = 0;

            while (timer < flashDuration)
            {
                // Cambia el color suavemente al originalColor
                targetImage.color = Color.Lerp(flashColor, originalColor, timer / flashDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;
            while (timer < flashDuration)
            {
                // Cambia el color suavemente al flashColor
                targetImage.color = Color.Lerp(originalColor, flashColor, timer / flashDuration);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        damage = false;
        setStateNormal();
    }
}


