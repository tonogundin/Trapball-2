using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float bombTimer;
    GameObject player;
    Player plScript;
    [SerializeField] GameObject playerPrefab;
    LevelMenuManager lvlMenu;
    public bool bombExploding;
    public float initPosForPlayerZ;
    public float zCamOffset;
    public bool[] levelsPassed = new bool[20];
    public delegate void Event();
    public static event Event NewPlayer;
    //Singleton pattern.
    public static GameManager gM;
    public Camera cam;
    public CheckPointActive checkPoints;

    // Estos valores dependerán de tu configuración inicial y del resultado deseado.
    public float defaultWidth = 1920f;  // Ancho por defecto
    public float defaultHeight = 1080f; // Altura por defecto
    public float defaultFOV = 30f;      // FOV por defecto

    public bool especialStage = false;

    FMODUnity.StudioEventEmitter emitter;

    private void Awake()
    {
        if (gM == null)
            gM = this;
        else if (gM != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += NewSceneLoaded;
        Setup();

    }
    void Setup()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SetupCamera();
        player = player = GameObject.FindGameObjectWithTag(Player.TAG);
        emitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.SetParameter(FMODConstants.STATE_MUSIC, (int)FMODConstants.MUSIC_STATE.ON_STAGE);
    }

    /* Método que ajusta el campo de visión de la cámara y las diferentes distancias
     * en Z en función del aspect ratio y nivel */
    void SetupCamera()
    {
        cam = Camera.main;
        cam.fieldOfView = defaultFOV;
        #if UNITY_STANDALONE
            CalculateFoV(cam.aspect);
        #endif

        #if UNITY_ANDROID
                    float ratio = (float) Screen.width / (float) Screen.height;
                    CalculateFoV(ratio);
        #endif
    }


    void CalculateFoV(float ratio)
    {
        //pruebaText.text = ratio.ToString();
        if (Mathf.Abs(ratio - 1.33f) < 0.1f) //4:3
        {
            zCamOffset = -15.82f;
        }
        else if (Mathf.Abs(ratio - 2) < 0.1f) //18:9
        {
            zCamOffset = -12.97f;
        }
        else if (Mathf.Abs(ratio - 2.16f) < 0.1f) //2340 x 1080
        {
            zCamOffset = -11.13f;
        }
        else if (Mathf.Abs(ratio - 1.6f) < 0.1f) //16:10
        {
            float lerpFactor = (1.6f - 1.33f) / (1.77f - 1.33f); // Calcula la posición relativa de 16:10 entre 4:3 y 16:9
            zCamOffset = Mathf.Lerp(-15.82f, -13.88f, lerpFactor);
        }
        else // 16:9
        {
            zCamOffset = -13.88f;
        }
        if (especialStage)
        {
            zCamOffset = 4.46f;
        }
    }

    void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gM == this) //Para prevenir llamadas por el evento desde el falso gM --> Pega nulo.
        {
            SetupCamera();
            //StopAllCoroutines(); //Prevenir fallos cuando Waiting se queda corriendo, se hace pausa y se carga una nueva escena retomando Waiting.
            initPosForPlayerZ = especialStage ? 3.83f : 5.45f;// new Vector3(-8f, 1.40f, 5.45f);
            InstantiateNewBall(0.5f, true);
        
        }
    }
    public void InstantiateNewBall(float secToWait)
    {
        InstantiateNewBall(secToWait, false);
    }
    public void InstantiateNewBall(float secToWait, bool firstTime)
    {
        Vector2 position = checkPoints.getPositionLastCheckPoint();
        if (firstTime)
        {
            checkPoints.setActiveCheckpointsObjects(false);
        }
        StartCoroutine(Waiting(secToWait, new Vector3(position.x, position.y, initPosForPlayerZ), firstTime));
    }
    IEnumerator Waiting(float secToWait, Vector3 initPos, bool firstTime)
    {
        yield return new WaitForSeconds(secToWait);
        GameEvents.instance.death.Invoke();
        player.transform.position = initPos;
        plScript = player.GetComponent<Player>();
        plScript.resetObject();
        plScript.especialStage = especialStage;
        if (firstTime)
        {
            plScript.GetComponent<Rigidbody>().AddForce(new Vector3(15f, 0, 0), ForceMode.Impulse);
        }
        NewPlayer();
        checkPoints.setActiveCheckpointsObjects(true);
        checkPoints.setResetCheckpointsObjects();
    }

    public void ChangeGravityScale(float factor)
    {
        Physics.gravity = new Vector3(Physics.gravity.x, factor, Physics.gravity.z);
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= NewSceneLoaded;
    }

}