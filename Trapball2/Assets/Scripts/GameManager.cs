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
    public Vector3 initPosForPlayer;
    public float zCamOffset;
    public float[] xLimits = new float[2];
    public float[] yLimits = new float[2];
    public bool[] levelsPassed = new bool[20];
    public delegate void Event();
    public static event Event NewPlayer;
    //Singleton pattern.
    public static GameManager gM;
    public Camera cam;
    private void Awake()
    {
        if (gM == null)
            gM = this;
        else if (gM != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += NewSceneLoaded;
        Setup();

    }
    void Setup()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SetupCamera();
        
    }

    /* Método que ajusta el campo de visión de la cámara y las diferentes distancias
     * en Z en función del aspect ratio y nivel */
    void SetupCamera()
    {
        cam = Camera.main;
        cam.fieldOfView = 30f;
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
            //cam.fieldOfView = 75.8f;
            zCamOffset = -15.82f;
            xLimits[0] = 0.69f;
            xLimits[1] = 10.1f;
            yLimits[0] = -18.7f;
            yLimits[1] = 7.16f;
        }

        else if (Mathf.Abs(ratio - 1.77f) < 0.1f) //16:9
        {
            //cam.fieldOfView = 69;
            zCamOffset = -13.88f;
            xLimits[0] = 1.78f;
            xLimits[1] = 9.05f;
            yLimits[0] = -19.27f;
            yLimits[1] = 7.75f;
        }
        else if (Mathf.Abs(ratio - 2) < 0.1f) //18:9
        {
            //cam.fieldOfView = 66f;
            zCamOffset = -12.97f;
            xLimits[0] = 2.22f;
            xLimits[1] = 8.57f;
            yLimits[0] = -19.5f;
            yLimits[1] = 7.95f;
        }
        else if (Mathf.Abs(ratio - 2.16f) < 0.1f) //2340 x 1080
        {
            //cam.fieldOfView = 59f;
            zCamOffset = -11.13f;
            xLimits[0] = 1.78f;
            xLimits[1] = 9.0f;
            yLimits[0] = -20f;
            yLimits[1] = 8.52f;
        }
    }

    void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gM == this) //Para prevenir llamadas por el evento desde el falso gM --> Pega nulo.
        {
            SetupCamera();
            //StopAllCoroutines(); //Prevenir fallos cuando Waiting se queda corriendo, se hace pausa y se carga una nueva escena retomando Waiting.
            initPosForPlayer = new Vector3(-1.547f, 8.72f, 0.8f);
            InstantiateNewBall(0, initPosForPlayer);
        
        }
    }
    public void InstantiateNewBall(float secToWait, Vector3 initPos)
    {
        StartCoroutine(Waiting(secToWait, initPos));
    }
    IEnumerator Waiting(float secToWait, Vector3 initPos)
    {
        yield return new WaitForSeconds(secToWait);
        //A new player is instantiated and player variable is overwritten.
        player = Instantiate(playerPrefab, initPos, Quaternion.Euler(0, -30, 0));
        plScript = player.GetComponent<Player>();
        NewPlayer();
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
