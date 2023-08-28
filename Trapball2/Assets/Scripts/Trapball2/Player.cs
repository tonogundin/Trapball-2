using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IResettable
{
    [HideInInspector] public Rigidbody rb;
    private Transform transform;
    float movementPlayer;
    float speedLimit = 5f;
    float movementForce = 10f;
    [SerializeField] LayerMask jumpable;
    ParticlesExplosion particles;
    SphereCollider coll;
    public float jumpForce;
    [SerializeField] PhysicMaterial bouncy;
    [SerializeField] float jumpDelta; //Define cuánto de rápido se alcanza el límite de fuerza de salto.
    [SerializeField] float jumpLimit = 10; //Define la mayor fuerza de salto posible a aplicar.
    [SerializeField] float initGravityFactor;
    float currentGravityFactor; //Añade un extra de gravedad para saltos más fluidos y rápidos. Tener en cuenta: A mayor factor, más nos costará saltar --> Incrementar jumpLimit
    bool freeFall;
    CameraShake camShakeScript;
    FMOD.Studio.EventInstance playerSoundroll;
    FMOD.Studio.EventInstance impactFloor;
    FMOD.Studio.EventInstance impactObjetc;
    FMOD.Studio.EventInstance underWater;
    FMOD.Studio.EventInstance impactWater;

    public Vector2 velocityBall;
    public int live = 8;
    public int valor = 0;
    private int liveInit;

    public StatePlayer state = StatePlayer.NORMAL;

    public  float jumpLowLimit = 3;
    private float jumpLowPercent = 0.70f;
    private float jumpLowLimitBomb;
    private float jumpLowBombPercent = 0.85f;

    private bool jumpCharge = false;

    private bool debugger = false;

    private bool isBalancin = false;
    private bool jumpBombEnabled = false;

    void Awake()
    {
        liveInit = live;
        rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        coll = GetComponent<SphereCollider>();
        particles = transform.GetChild(0).GetComponent<ParticlesExplosion>();
        currentGravityFactor = initGravityFactor;
        jumpLowLimit = jumpLimit * jumpLowPercent;
        jumpLowLimitBomb = jumpLimit * jumpLowBombPercent;
    }

    void Start()
    {
        state = StatePlayer.JUMP;
        camShakeScript = GameManager.gM.cam.GetComponent<CameraShake>();
        playerSoundroll = FMODUnity.RuntimeManager.CreateInstance("event:/Desplazamiento/SFXPlayerRollMud");
        impactFloor = FMODUnity.RuntimeManager.CreateInstance("event:/Saltos/ImpactoTerreno");
        impactObjetc = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/ImpactObject");
        underWater = FMODUnity.RuntimeManager.CreateInstance("event:/Ambientes/AmbienteUnderwater");
        impactWater = FMODUnity.RuntimeManager.CreateInstance("event:/Saltos/ImpactWater");
        playerSoundroll.start();
    }

    void FixedUpdate() {
        rb.AddForce(new Vector3(movementPlayer, 0, 0) * movementForce, ForceMode.Force); //Para movimiento.
        manageExtraGravity();
        if (!freeFall) {
            ManageBallSpeed();
        }
        velocityBall = new Vector2(rb.velocity.x, rb.velocity.y);
        playerSoundroll.setParameterByName("speed", velocityBall.x);
        impactFloor.setParameterByName("speed", velocityBall.y);
    }

    void Update() {
        if (state != StatePlayer.DEAD && state != StatePlayer.FINISH)
        {
            processJumpForce();
            touchingFloor();
            checkSoundRoll();
            checkRotations();
        }
    }

    private void checkRotations()
    {
        if (rb.rotation.x != 0.000f || rb.rotation.y != 0.000f)
        {
            rb.rotation = new Quaternion(0.0f, 0.0f, rb.rotation.z, rb.rotation.w);
        }
    }
    void touchingFloor()
    {
        if (rb.velocity.y <= 0 && state != StatePlayer.INIT_JUMP)
        {
            if (isColliderPlatforms())
            {
                collisionFloor();
            }
            else if (state == StatePlayer.NORMAL)
            {
                state = StatePlayer.JUMP;
            }
        }
        else if (rb.velocity.y > 0 && state == StatePlayer.NORMAL && !isColliderPlatforms())
        {
            resetJumpForce();
            state = StatePlayer.JUMP;
        }
    }

    private bool isColliderPlatforms()
    {
        Vector3 centralOffset = new(0, -0.35f, 0);
        Vector3 leftOffset = new(-0.35f, -0.35f, 0);
        Vector3 rightOffset = new(0.35f, -0.35f, 0);
        Collider[] centralColls = Physics.OverlapSphere(transform.position + centralOffset, 0.1f, jumpable.value);
        bool result = centralColls.Length > 0;
        if (!result)
        {
            // Si no hay colisión en el centro, entonces verificamos los lados
            Collider[] leftColls = Physics.OverlapSphere(transform.position + leftOffset, 0.1f, jumpable.value);
            Collider[] rightColls = Physics.OverlapSphere(transform.position + rightOffset, 0.1f, jumpable.value);
            result = leftColls.Length > 0 && rightColls.Length > 0;
        }
        return result;
    }

    private void collisionFloor()
    {
        if (state == StatePlayer.BOMBJUMP)
        {
            endBombJump();
        }
        else if (state != StatePlayer.NORMAL)
        {
            playerSoundroll.setVolume(1);
            state = StatePlayer.NORMAL;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //Volvemos a discreto para consumir menos recursos.
            jumpBombEnabled = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/ImpactoTerreno", GetComponent<Transform>().position);
        }
    }

    void processJumpForce() {
        switch (state)
        {
            case StatePlayer.NORMAL:
                if (jumpCharge && jumpForce < jumpLimit)
                {
                    jumpForce += jumpDelta * Time.deltaTime;
                    if (jumpForce > jumpLimit)
                    {
                        jumpForce = jumpLimit;
                    }
                }
                break;
        }

        
    }

    void handleButtonDown()
    {
        jumpCharge = true;
        switch (state)
        {
            case StatePlayer.NORMAL:
                
                break;
            case StatePlayer.JUMP:
                if (jumpBombEnabled)
                {
                    bombJump();
                }
                break;
        }
    }
    void handleButtonUp() {
        jumpCharge = false;
        switch (state) {
            case StatePlayer.NORMAL:
                adjustJumpForce();
                simpleJump();
                break;
            case StatePlayer.JUMP:
                resetJumpForce();
                break;
        }
    }

    void adjustJumpForce() {
        if (jumpForce > 0) {
            jumpForce = Mathf.Clamp(jumpForce, jumpLowLimit, jumpLimit);
        }
    }

    public void resetJumpForce()
    {
        jumpForce = 0;
    }

    void simpleJump() {
        state = StatePlayer.INIT_JUMP;
        if (jumpForce < jumpLowLimit)
        {
            jumpForce = jumpLowLimit;
        }
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //Para salto.
        playerSoundroll.setVolume(0);
        if (jumpForce > jumpLowLimitBomb) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/SaltoBomba", transform.position);
        } else {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/SaltoLow", transform.position);
        }
        StartCoroutine(stateJump());
    }

    void bombJump() {
        state = StatePlayer.BOMBJUMP;
        jumpBombEnabled = false;
        coll.material = bouncy; //Le ponemos un material rebotante.
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //Cambiamos a dinámico por si atraviesa.
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/SaltoBomba", transform.position);
        playerSoundroll.setVolume(0);
    }

    IEnumerator stateJump() {
        yield return new WaitForSeconds(0.15f);
        state = StatePlayer.JUMP;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //Volvemos a discreto para consumir menos recursos.
        if (jumpForce > jumpLowLimitBomb)
        {
            jumpBombEnabled = true;
        }
        resetJumpForce();
    }
    IEnumerator stateNormal()
    {
        yield return new WaitForSeconds(0.5f);
        state = StatePlayer.NORMAL;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //Volvemos a discreto para consumir menos recursos.
    }
    void endBombJump() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/ImpactoTerrenoBomba", transform.position);
        StartCoroutine(camShakeScript.Shake(0.10f, 0.15f));
        coll.material = null;
        playerSoundroll.setVolume(1);
        state = StatePlayer.END_BOMB_JUMP;
        StartCoroutine(stateNormal());
    }

    private void checkSoundRoll()
    {
        float valueSoundRoll;
        playerSoundroll.getParameterByName("onTheFloor", out valueSoundRoll);
        if (state == StatePlayer.NORMAL && valueSoundRoll == 0)
        {
            playerSoundroll.setParameterByName("onTheFloor", 1);
        }
        else if (state != StatePlayer.NORMAL && valueSoundRoll == 1)
        {
            playerSoundroll.setParameterByName("onTheFloor", 0);
        }
    }

    void manageExtraGravity() {
        if (state != StatePlayer.NORMAL) {
            Vector3 vel = rb.velocity;
            vel.y -= currentGravityFactor * Time.fixedDeltaTime;
            rb.velocity = vel;
        }
    }
    void ManageBallSpeed() {
        //Límite de velocidad
        if (Mathf.Abs(rb.velocity.x) > speedLimit) {
            rb.velocity = new Vector3(speedLimit * Mathf.Sign(rb.velocity.x), rb.velocity.y, rb.velocity.z);
        }
        //Ayuda para que no cueste tanto dejar la bola quieta
        //Si la bola a penas se mueve, no hay input de usuario y no está en una rampa (y == 0 + TouchingFloor) se parará por completo.
        else if (Mathf.Abs(rb.velocity.x) < 0.3f && movementPlayer == 0 && rb.velocity.y == 0) {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        string tag = collision.gameObject.tag;

        switch (tag) {
            case "SueloPantanoso":
                setTerrainParametersAndStart(0);
                break;
            case "Balancin":
            case "SueloMadera":
            case "Box":
                float collisionForce = collision.relativeVelocity.magnitude * 250;
                impactObjetc.setParameterByName("speed", collisionForce);
                setTerrainParametersAndStart(1);
                impactObjetc.start();
                impactObjetc.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            case "SueloPiedra":
                setTerrainParametersAndStart(2);
                break;
            default:
                // Handle other cases or do nothing
                break;
        }

        switch (tag)
        {
            case "Balancin":
                isBalancin = true;
                break;
            default:
                // Handle other cases or do nothing
                break;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        string tag = collision.gameObject.tag;

        switch (tag)
        {
            case "Balancin":
                isBalancin = false;
                break;
            default:
                // Handle other cases or do nothing
                break;
        }
    }

    private void setTerrainParametersAndStart(int terrainValue) {
        playerSoundroll.setParameterByName("Terrain", terrainValue);
        impactFloor.setParameterByName("Terrain", terrainValue);
        impactFloor.start();
    }


    private void OnTriggerEnter(Collider other) {
        string tag = other.tag;
        switch (tag) {
            case "Water":
                rb.velocity = new Vector3(rb.velocity.x, -0.5f, rb.velocity.z);
                impactWater.start();
                underWater.start();
                break;
            case "TubeEnter":
                freeFall = true;
                break;
            case "TubeExit":
                freeFall = false;
                break;
            case "Courage":
                valor++;
                break;
            case "Exit":
                state = StatePlayer.FINISH;
                rb.velocity = new Vector3(0, 0, 0);
                playerSoundroll.setParameterByName("speed", 0);
                playerSoundroll.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;
            default:
                // Handle other cases or do nothing
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water")) 
        {
            currentGravityFactor = -5f;
            rb.angularDrag = 4;
            rb.drag = 2.2f;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Water")) {
            currentGravityFactor = initGravityFactor;
            rb.angularDrag = 0.05f;
            rb.drag = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/ImpactoTerrenoBomba", GetComponent<Transform>().position);
            underWater.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    public void die(){
        FMODUnity.RuntimeManager.PlayOneShot("event:/Daño/ImpactoPinchos", GetComponent<Transform>().position);
        genericDie();
    }

    public void dieLive()
    {
        genericDie();
    }

    private void genericDie()
    {
        GameManager.gM.ChangeGravityScale(-9.81f); //También cambio la gravedad aquí porque si no se nota más gravedad en las partículas.
        particles.Explode();
        StartCoroutine(delayDead());
        FMODUnity.RuntimeManager.PlayOneShot("event:/Daño/DeathVoice", GetComponent<Transform>().position);
        rb.isKinematic = true;
        GetComponent<Renderer>().enabled = false;
        state = StatePlayer.DEAD;
    }

    IEnumerator delayDead()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.gM.InstantiateNewBall(2);
    }

    public void resetObject()
    {
        rb.isKinematic = false;
        GetComponent<Renderer>().enabled = true;
        state = StatePlayer.NORMAL;
        jumpBombEnabled = false;
        particles.resetObject();
        currentGravityFactor = initGravityFactor;
        rb.angularDrag = 0.05f;
        rb.drag = 0;
        valor = 0;
        live = liveInit;
    }

    public bool isTouchFloor() {
        return state == StatePlayer.NORMAL;
    }

    public bool isOnBalancin()
    {
        return state == StatePlayer.NORMAL && isBalancin;
    }

    public float getJumpForce()
    {
        return jumpForce;
    }

    public bool isJumpBombEnabled()
    {
        return jumpBombEnabled;
    }

    public float getJumpLimit()
    {
        return jumpLimit;
    }
    public float getJumpLowLimit()
    {
        return jumpLowLimit;
    }

    public void addDamage()
    {
        live--;
        if (live<=0)
        {
            dieLive();
        }
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            handleButtonDown();
        } else
        {
            handleButtonUp();
        }
    }

    public void OnMove(InputValue value)
    {
        if (state != StatePlayer.DEAD && state != StatePlayer.FINISH)
        {
#if UNITY_STANDALONE

#endif
#if UNITY_ANDROID
                                //movementPlayer = Input.acceleration.x * 2;
#endif

            movementPlayer = value.Get<Vector2>().x;
        }
    }

    public void OnMenu()
    {
        StartCoroutine(delayChangeScene());
    }
    IEnumerator delayChangeScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadSceneAsync("Menu");
    }


    void OnGUI()
    {
        if (debugger)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(50, 50, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperCenter;
            style.fontSize = h * 4 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            string text = string.Format("X: {0:0.000} Y:{1:0.000}", velocityBall.x, velocityBall.y);
            GUI.Label(rect, text, style);

            Rect rect2 = new Rect(50, 250, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperCenter;
            style.fontSize = h * 4 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            string text2 = string.Format("State =" + state);
            GUI.Label(rect2, text2, style);
        }
    }

}

public enum StatePlayer
{
    NORMAL,
    INIT_JUMP,
    JUMP,
    BOMBJUMP,
    END_BOMB_JUMP,
    FALL,
    DEAD,
    FINISH
}