using UnityEngine;
using System.Collections;


public class Beatle : MonoBehaviour, IResettable
{
    public const string TAG = "Beatle";
    Rigidbody rb;
    Animator animator;
    GameObject player;
    bool playerDetected;
    Vector2 distToPlayer;
    Vector3 dirToPlayer;
    float distToObjectReference;
    Vector3 dirToObjectReference;
    bool outofObjectReference;
    float extraGravityFactor = 10;
    float movementForce = 5;
    private bool stayonShip = false;
    public float ForceXImpact = 7;
    public GameObject objectReference;
    public float maxPositionZ;

    public State state;
    public State antState;
    private State initialState;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float velocityZ = 0;

    const string animBeatleIdle = "BeatleIdle";
    const string animBeatleMove = "BeatleMove";
    const string animBeatleMoveAgressive = "BeatleMoveAgressive";
    const string animBeatleFall = "BeatleFall";
    const string animBeatleNail = "BeatleNail";
    const string animBeatleFreeNail = "BeatleFreeNail";
    const string animBeatlePreAttack = "BeatlePreAttack";
    const string animBeatleAttack = "BeatleAttack";

    private const int layerPlatform = 9;
    
    public float waitTimePreAttack = 0.75f; // Tiempo de espera en segundos
    public float waitTimeAttack = 0.25f; // Tiempo de espera en segundos

    private Vector3 positionAttack;

    public float forceAttackX = 20;
    public float forceAttackY = 20;
    public bool launchReverse = false;
    private bool launch = false;
    public float distanceXHang = 2.5f;

    FMOD.Studio.EventInstance SoundAttack;
    FMOD.Studio.EventInstance SoundFall;
    FMOD.Studio.EventInstance SoundRun;
    FMOD.Studio.EventInstance SoundHit;
    FMOD.Studio.EventInstance SoundExit;

    private Coroutine currentCoroutine;

    void Start()
    {
        initialState = state;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        initialRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
        SoundAttack = FMODUtils.createInstance(FMODConstants.BEATLE.ATTACK);
        SoundFall = FMODUtils.createInstance(FMODConstants.BEATLE.FALL);
        SoundRun = FMODUtils.createInstance(FMODConstants.BEATLE.RUN);
        SoundExit = FMODUtils.createInstance(FMODConstants.BEATLE.EXIT);
    }
    void Update()
    {
        if (isActiveAndEnabled)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag(Player.TAG);
            }
            else
            {
                if (state == State.MOVE)
                {
                    SoundRun.setParameterByName("speed", rb.velocity.x);
                }
                dirToPlayer = (player.transform.position - transform.position).normalized;
                if (velocityZ != 0)
                {
                    dirToPlayer = new Vector3(dirToPlayer.x, dirToPlayer.y, velocityZ > 0 ? 180 : -180);
                }
                else if (objectReference != null)
                {
                    distToObjectReference = objectReference.transform.position.x - transform.position.x;
                    dirToObjectReference = (player.transform.position - transform.position).normalized;
                    outofObjectReference = isOutofObjectReference();
                }

                if (isDetectPlayer())
                {
                    playerDetected = true;
                    if (state == State.HANG)
                    {
                        rb.isKinematic = false;
                        state = State.FALL;
                    }
                    if (isDetectedPlayerNear())
                    {
                        dirToPlayer.y = 0f; //Si estamos muy cerca del player,no rotamos en y para que no haga rotación rara.
                        activePrepareAttack();
                    }
                }
                else
                {
                    playerDetected = false;
                }
            }
            ManageRotation();
            if (state == State.PREPARE_ATTACK)
            {
                transform.position = positionAttack;
            }

            if (rb.velocity.y < -4 && rb.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //Volvemos a discreto para consumir menos recursos.
            }
            else if (rb.collisionDetectionMode != CollisionDetectionMode.Discrete)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //Volvemos a discreto para consumir menos recursos.
            }
        }
    }

    private void activePrepareAttack()
    {
        if (!launch && isBeatleInPositionForAttack() && (state == State.NORMAL || state == State.MOVE))
        {
            launch = true;
            state = State.PREPARE_ATTACK;
            positionAttack = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
    
    private bool isBeatleInPositionForAttack()
    {
        float rotationY = rb.rotation.eulerAngles.y;

        // Define un rango de tolerancia
        float tolerance = 5f; // 5 grados de tolerancia a cada lado

        // Verifica si la rotación está dentro del rango de 90 +/- tolerancia o 270 +/- tolerancia
        return Mathf.Abs(rotationY - 90) <= tolerance || Mathf.Abs(rotationY - 270) <= tolerance;
    }

    private bool isBeatleInPositionLeft()
    {
        float rotationY = rb.rotation.eulerAngles.y;

        // Define un rango de tolerancia
        float tolerance = 5f; // 5 grados de tolerancia a cada lado

        // Verifica si la rotación está dentro del rango de 90 +/- tolerancia o 270 +/- tolerancia
        return Mathf.Abs(rotationY - 90) <= tolerance;
    }
    private bool isDetectPlayer()
    {
        distToPlayer = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);

        if (state == State.HANG)
        {
            return Mathf.Abs(distToPlayer.x) < distanceXHang // tenga un rango x para visualizarlo desde la altura.
                   && distToPlayer.y < -1 // el player esté por debajo del beatle en y.
                   && distToPlayer.y > -10; // que el player no esté a una distancia muy grande en altura.
        } else
        {
            return Mathf.Abs(distToPlayer.x) <= 10f;
        }
    }

    private bool isDetectedPlayerNear()
    {
        return Mathf.Abs(distToPlayer.x) <= 1.35f
               && Mathf.Abs(distToPlayer.y) < 1;
    }

    private void checkVelocity()
    {
        float velocityX = rb.velocity.x;
        if (velocityX > 1)
        {
            velocityX = 1;
        }
        else if (velocityX < -1)
        {
            velocityX = -1;
        }
        float velocity_Z = rb.velocity.z;
        if (velocity_Z > 0.25f)
        {
            velocityX = 0.25f;
        }
        else if (velocity_Z < -0.25f)
        {
            velocityX = -0.25f;
        }
        if (!rb.isKinematic)
        {
            rb.velocity = new Vector3(velocityX, rb.velocity.y, velocity_Z);
        }
    }
    private void FixedUpdate()
    {
        if (isActiveAndEnabled)
        {
            AddExtraGravityForce();
            float velocityX = Mathf.Sign(distToPlayer.x);
            if (transform.localPosition.z < maxPositionZ - 0.05f)
            {
                velocityZ = -0.25f;
            }
            else
            {
                velocityZ = 0;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, maxPositionZ);
            }
            if (!playerDetected && objectReference != null && outofObjectReference)
            {
                velocityX = Mathf.Sign(distToObjectReference);
            }
            if (velocityZ != 0)
            {
                rb.constraints = RigidbodyConstraints.None;
                velocityX = 0;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezePositionZ;
            }
            if (velocityZ != 0 || playerDetected || objectReference != null && outofObjectReference)
            {
                if (state == State.NORMAL)
                {
                    state = State.MOVE;
                }
                if (state == State.MOVE)
                {
                    move(velocityX, 0, velocityZ, ForceMode.Acceleration);
                    checkVelocity();
                }
            }
            else
            {
                if (isNormalsState())
                {
                    state = State.NORMAL;
                    if (!rb.isKinematic)
                    {
                        rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                    }
                }
            }
            checkState();
        }
    }

    private void move(float velocityX, float velocityY, float velocityZ, ForceMode mode)
    {
        velocityX = limitVelocity(velocityX, 1);
        rb.AddForce(new Vector3(velocityX, velocityY, velocityZ) * movementForce, mode);
    }

    private float limitVelocity(float velocity, float limit)
    {
        if (velocity > limit)
        {
            velocity = limit;
        }
        else if (velocity < -limit)
        {
            velocity = -limit;
        }
        return velocity;
    }

    private bool isOutofObjectReference()
    {
        if (objectReference != null)
        {
            // Asegúrate de que el objeto detectado tiene un Collider
            Collider[] detectedColliders = objectReference.GetComponentsInChildren<Collider>();
            if (detectedColliders == null || detectedColliders.Length == 0) return false;

            // Calcula los extremos en el eje x del objeto detectado
            float detectedHalfWidth = detectedColliders[0].bounds.extents.x; // la mitad del ancho en el eje x
            float detectedLeftX = objectReference.transform.position.x - detectedHalfWidth;
            float detectedRightX = objectReference.transform.position.x + detectedHalfWidth;

            // Obtén la posición x y z del GameObject actual (el que tiene este script)
            float currentX = transform.position.x;

            // Compara las posiciones en ambos ejes
            bool isOutOfXBounds = currentX <= detectedLeftX || currentX >= detectedRightX;

            return isOutOfXBounds; // Si está fuera de los límites en cualquiera de los ejes, retorna true
        }
        return false;
    }

    private void checkState()
    {
        if (antState != state)
        {
            antState = state;
            switch (state)
            {
                case State.NORMAL:
                    animator.SetTrigger(animBeatleIdle);
                    break;
                case State.HANG:
                    animator.SetTrigger(animBeatleIdle);
                    break;
                case State.FALL:
                        animator.SetTrigger(animBeatleFall);
                        break;
                case State.MOVE:
                    animator.SetTrigger(animBeatleMove);
                    //SoundRun.start();
                    FMODUtils.play3DSound(SoundRun, transform);
                    break;
                case State.MOVE_AGRESSIVE:
                    animator.SetTrigger(animBeatleMoveAgressive);
                    break;
                case State.PREPARE_ATTACK:
                    prepareAttack();
                    break;
                case State.ATTACK:
                    animator.SetTrigger(animBeatleAttack);
                    break;
                case State.IMPACT:
                    SoundAttack.start();
                    currentCoroutine = StartCoroutine(impulsePlayer());
                    break;
                case State.NAIL:
                    SoundFall.start();
                    animator.SetTrigger(animBeatleNail);
                    currentCoroutine = StartCoroutine(stateFreeNailToNormal());
                    break;
                case State.FREE_NAIL:
                    animator.SetTrigger(animBeatleFreeNail);
                    break;
            }
        }
    }

    private void prepareAttack()
    {
        currentCoroutine = StartCoroutine(changeStateAttackAndNormal());
        animator.SetTrigger(animBeatlePreAttack);
    }

    void ManageRotation()
    {
        ManageRotation(false);
    }
    void ManageRotation(bool force)
    {
        if (isNormalsState() || force)
        {
            float rotVelocity = 2.5f;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); // Restricción en Z

            Vector3 directionToTarget = (objectReference != null && outofObjectReference) ? dirToObjectReference : dirToPlayer;
            Quaternion rotToPlayer = Quaternion.LookRotation(directionToTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, rotVelocity * Time.deltaTime);
        }
    }

    bool isNormalsState()
    {
        return state == State.NORMAL || state == State.MOVE || state == State.MOVE_AGRESSIVE;
    }

    void AddExtraGravityForce()
    {
        if (!rb.isKinematic && !stayonShip)
        {
            Vector3 vel = rb.velocity;
            vel.y -= extraGravityFactor * Time.fixedDeltaTime;
            rb.velocity = vel;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isObjetEqualsPlayer(collision.gameObject))
        {
            ContactPoint[] contacts = collision.contacts;
            if (contacts != null && contacts.Length > 0)
            {
                Rigidbody rbPlayer = collision.gameObject.GetComponent<Rigidbody>();

                float impactY = collision.relativeVelocity.y * -1;
                if (impactY > 0)
                {
                    Debug.Log("impactY: " + impactY);
                    if (Utils.IsCollisionAboveMe(collision, transform.position.y))
                    {
                        soundHit(impactY);
                    }
                    if (impactY > 6)
                    {
                        impactY = 6;
                    }
                    if (impactY < 4)
                    {
                        impactY = 4;
                    }
                    rbPlayer.AddForce(new Vector3(0, impactY, 0), ForceMode.Impulse);
                    rbPlayer.velocity = new Vector3(0, rbPlayer.velocity.y, rbPlayer.velocity.z);
                }
            }
        }
    }

    private void soundHit(float impact)
    {
        int value = 0;

        if (impact >= 0 && impact <= 1.5f)
        {
            // Mapeamos el rango de 0 a 1.5 a 0 a 30
            value = Mathf.RoundToInt(Mathf.Lerp(0, 30, impact / 1.5f));
        }
        else if (impact > 1.5f && impact <= 8)
        {
            // Mapeamos el rango de 1.5 a 8 a 35 a 75
            value = Mathf.RoundToInt(Mathf.Lerp(35, 75, (impact - 1.5f) / (8 - 1.5f)));
        }
        else if (impact > 8)
        {
            // Mapeamos el rango de 8 a 18 a 85 a 100
            value = Mathf.RoundToInt(Mathf.Lerp(85, 100, (impact - 8) / (18 - 8)));
        }

        SoundHit = FMODUtils.createInstance(FMODConstants.BEATLE.HIT);
        SoundHit.setParameterByName(FMODConstants.HIT_FORCE, value);
        SoundHit.start();
        SoundHit.release();
    }


    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Balancin":
                stayonShip = true;
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Balancin":
                stayonShip = false;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == layerPlatform && state == State.FALL)
        {
            rb.isKinematic = true;
            state = State.NAIL;
        }
        if (other.CompareTag(Player.TAG))
        {
            switch(state)
            {
                case State.ATTACK:
                    state = State.IMPACT;
                    break;

                case State.FALL:
                    Player player = other.GetComponent<Player>();
                    if (player.isNotDead())
                    {
                        FMODUtils.playOneShot(FMODConstants.DAMAGE.IMPACT_SPIKES, transform.position);
                        player.die();
                    }
                    break;
            }
        }
        if (other.CompareTag("Water"))
        {
            if (rb.velocity.x > 0.6f)
            {
                rb.velocity = new Vector2(0.6f, rb.velocity.y);
            }
            if (rb.velocity.x < -0.6f)
            {
                rb.velocity = new Vector2(-0.6f, rb.velocity.y);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            rb.angularDrag = 0.05f;
            rb.drag = 0;
        }
    }

    IEnumerator changeStateAttackAndNormal()
    {
        yield return new WaitForSeconds(waitTimePreAttack);
        state = State.ATTACK;
        rb.AddForce(new Vector3(isBeatleInPositionLeft() ? 10 : -10, 0, 0), ForceMode.Impulse);
        yield return new WaitForSeconds(waitTimeAttack);
        state = State.NORMAL;
        yield return new WaitForSeconds(2f);
        launch = false;
    }
    IEnumerator impulsePlayer()
    {
        yield return new WaitForSeconds(0.01f);
        float forceLeft = launchReverse ? -forceAttackX : forceAttackX;
        float forceRight = launchReverse ? forceAttackX : -forceAttackX;
        player.GetComponent<Rigidbody>().AddForce(new Vector3(isBeatleInPositionLeft()? forceLeft : forceRight, forceAttackY), ForceMode.Impulse);
    }
    IEnumerator stateFreeNailToNormal()
    {
        yield return new WaitForSeconds(1f);
        state = State.FREE_NAIL;
        yield return new WaitForSeconds(0.26f);
        SoundExit.start();
        rb.isKinematic = false;
        rb.AddForce(new Vector3(isBeatleInPositionLeft() ? -80 : 80, 40, 0), ForceMode.Impulse);
        ManageRotation(true);
        yield return new WaitForSeconds(0.40f);
        state = State.NORMAL;
    }

    public bool isStayonShip()
    {
        return stayonShip;
    }

    public void resetObject()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null; // Resetear la referencia después de detenerla
        }
        outofObjectReference = false;
        playerDetected = false;
        launch = false;
        state = initialState;
        antState = State.NONE;
        transform.localPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        transform.localRotation = new Quaternion(initialRotation.x, initialRotation.y, initialRotation.z, initialRotation.w);
        if (!rb.isKinematic)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
        rb.isKinematic = true;
    }

    private bool isObjetEqualsPlayer(GameObject gameObject)
    {
        return gameObject == player;
    }
    public enum State
    {
        NONE,
        NORMAL,
        MOVE,
        MOVE_AGRESSIVE,
        HANG,
        FALL,
        NAIL,
        FREE_NAIL,
        PREPARE_ATTACK,
        ATTACK,
        IMPACT
     }

    private enum Animations
    {
        IDLE,
        MOVE,
        FALL,
    }
}
