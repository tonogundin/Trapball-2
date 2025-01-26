using UnityEngine;
using System.Collections;

public class MouseBall2 : MonoBehaviour, IResettable
{
    public const string TAG = "RataBola";
    Rigidbody rb;
    Animator animator;
    GameObject player;
    bool playerDetected;
    float distToPlayer;
    Vector3 dirToPlayer;
    float distToObjectReference;
    Vector3 dirToObjectReference;
    bool outofObjectReference;
    float extraGravityFactor = 10;
    float movementForce = 5;
    Vector3 checkerOffset;
    Timer timeKnockOut;
    Timer timeRecover;
    Timer timeSwimming;
    private bool stayonShip = false;
    public GameObject normalCollider;
    public GameObject smashCollider;
    public float ForceXImpact = 7;
    public GameObject objectReference;
    public float maxPositionZ;

    const string animMouseIdle = "MouseBallIddle";
    const string animMouseMove = "MouseBallMove";
    const string animMouseMoveAgressive = "MouseBallMoveAgressive";
    const string animMouseSmash = "MouseBallSmash";
    const string animMouseRecover = "MouseBallRecover";


    private State state;
    private State antState;
    private bool contactBall = false;
    private int secondsKnocked = 1500;

    private Vector3 initialPosition;

    private float velocityZ = 0;

    //Instancias de FMOD
    FMOD.Studio.EventInstance BallMouseRun;
    FMOD.Studio.EventInstance BallMouseHurt;
    FMOD.Studio.EventInstance BallMouseHit;
    FMOD.Studio.EventInstance BallMouseInflatingPop;
    FMOD.Studio.EventInstance BallMouseJump;
    FMOD.Studio.EventInstance BallMouseScream;
    private FMOD.Studio.EventInstance impactObjetc;
    private FMOD.Studio.EventInstance impactFloor;
    private FMOD.Studio.EventInstance exitTerrain;

    private ParticlesWaterController particlesWaterController;

    private static bool canApplyImpulse = true; // Bandera para controlar si se puede aplicar el impulso
    private static float impulseCooldown = 0.1f; // Duración del cooldown en segundos


    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        state = State.NONE;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        timeKnockOut = new Timer((int)(clips[(int)Animations.SMASH].length * 1000) + secondsKnocked, new CallBackSmashTimer(this));
        timeRecover = new Timer((int)(clips[(int)Animations.RECOVER].length * 1000), new CallBackRecoverTimer(this));
        timeSwimming = new Timer(150, new CallBackSwimmingTimer(this));
        changeCollider(false);
        particlesWaterController = GetComponent<ParticlesWaterController>();

        BallMouseRun = FMODUtils.createInstance(FMODConstants.BALLMOUSE.RUN);
        BallMouseHurt = FMODUtils.createInstance(FMODConstants.BALLMOUSE.HURT);
        BallMouseHit = FMODUtils.createInstance(FMODConstants.BALLMOUSE.HIT);
        BallMouseInflatingPop = FMODUtils.createInstance(FMODConstants.BALLMOUSE.INFLATING_POP);
        BallMouseJump = FMODUtils.createInstance(FMODConstants.BALLMOUSE.JUMP);
        BallMouseScream = FMODUtils.createInstance(FMODConstants.BALLMOUSE.SCREAM);
        impactFloor = FMODUtils.createInstance(FMODConstants.JUMPS.IMPACT_TERRAIN_ENEMIES);
        exitTerrain = FMODUtils.createInstance(FMODConstants.JUMPS.EXIT_TERRAIN_ENEMIES);
        impactObjetc = FMODUtils.createInstance(FMODConstants.OBJECTS.IMPACT_OBJECT_ENEMIES);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveAndEnabled)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag(Player.TAG);
            }
            else if (!isSmashProcess())
            {
                distToPlayer = player.transform.position.x - transform.position.x;
                dirToPlayer = (player.transform.position - transform.position).normalized;
                if (velocityZ != 0)
                {
                    dirToPlayer = new Vector3(dirToPlayer.x, dirToPlayer.y, velocityZ > 0 ? 180 : -180);

                } else if (objectReference != null)
                {
                    distToObjectReference = objectReference.transform.position.x - transform.position.x;
                    dirToObjectReference = (player.transform.position - transform.position).normalized;
                    outofObjectReference = isOutofObjectReference();
                }
                
                if (Mathf.Abs(distToPlayer) <= 5f)
                {
                    playerDetected = true;
                    if (Mathf.Abs(distToPlayer) <= 0.75f)
                    {
                        dirToPlayer.y = 0f; //Si estamos muy cerca del player,no rotamos en y para que no haga rotación rara.
                    }
                }
                else
                {
                    playerDetected = false;
                }
            }
            ManageRotation();
            if (rb.linearVelocity.y < -4 && rb.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //Volvemos a discreto para consumir menos recursos.
            }
            else if (rb.collisionDetectionMode != CollisionDetectionMode.Discrete)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //Volvemos a discreto para consumir menos recursos.
            }
        }
    }
    private void checkVelocity()
    {
        float velocityX = rb.linearVelocity.x;
        if (velocityX > 6)
        {
            velocityX = 6;
        } else if (velocityX < -6)
        {
            velocityX = -6;
        }
        float velocity_Z = rb.linearVelocity.z;
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
            rb.linearVelocity = new Vector3(velocityX, rb.linearVelocity.y, velocity_Z);
        }
    }
    private void FixedUpdate()
    {
        if (isActiveAndEnabled)
        {
            if (state == State.MOVE)
            {
                BallMouseRun.setParameterByName("speed", rb.linearVelocity.x);
            }
            AddExtraGravityForce();
            if (!isSmashProcess())
            {
                float velocityX = Mathf.Sign(distToPlayer);
                if (transform.localPosition.z < maxPositionZ - 0.05f)
                {
                    velocityZ = -0.25f;
                } else
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
                    rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
                }
                if (velocityZ != 0 || playerDetected || objectReference != null && outofObjectReference)
                {
                    if (state == State.NORMAL)
                    {
                        state = State.MOVE;
                    }
                    if (state != State.SWIMMING)
                    {
                        move(velocityX, 0, velocityZ, ForceMode.Acceleration);
                        checkVelocity();
                    }
                }
                else
                {
                    if (state != State.SWIMMING)
                    {
                        state = State.NORMAL;
                        if (!rb.isKinematic)
                        {
                            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
                        }
                    }
                }
                if (state == State.SWIMMING)
                {
                    move(0, 0.25f, 0, ForceMode.Impulse);

                    move(limitVelocity(velocityX, 0.25f), 0, velocityZ, ForceMode.Acceleration);

                    if (rb.linearVelocity.y > 5)
                    {
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5, rb.linearVelocity.z);
                    }
                    if (rb.linearVelocity.x > 2)
                    {
                        rb.linearVelocity = new Vector3(2, rb.linearVelocity.y, rb.linearVelocity.z);
                    }
                    if (rb.linearVelocity.x < -2)
                    {
                        rb.linearVelocity = new Vector3(-2, rb.linearVelocity.y, rb.linearVelocity.z);
                    }
                }

            }
            else
            {
                if (!contactBall)
                {
                    rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
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
                case State.SWIMMING:
                    animator.SetTrigger(animMouseIdle);
                    break;
                case State.NORMAL:
                    animator.SetTrigger(animMouseIdle);
                    break;
                case State.MOVE:
                    animator.SetTrigger(animMouseMoveAgressive);
                    FMODUtils.play3DSound(BallMouseRun, transform);
                    FMODUtils.play3DSound(BallMouseScream, transform);                    
                    break;
                case State.MOVE_AGRESSIVE:
                    animator.SetTrigger(animMouseMoveAgressive);
                    
                    break;
                case State.SMASH:
                    timeRecover.stopTimer();
                    timeKnockOut.stopTimer();
                    timeKnockOut.startTimer();
                    BallMouseHurt.start();
                    animator.SetTrigger(animMouseSmash);
                    BallMouseRun.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    break;
                case State.RECOVER:
                    timeKnockOut.stopTimer();
                    timeRecover.stopTimer();
                    timeRecover.startTimer();
                    animator.SetTrigger(animMouseRecover);
                    FMODUtils.play3DSound(BallMouseInflatingPop, transform);
                    break;
            }
        }
    }

    public bool isSmashProcess()
    {
        return state == State.SMASH || state == State.RECOVER;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + checkerOffset, 0.25f);
    }
    void ManageRotation()
    {
        Quaternion rotToPlayer;
        float rotVelocity = 5f;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0); //Para que no rote en "Z" y se vuelque.
        rotToPlayer = Quaternion.LookRotation(dirToPlayer, transform.up);
        if (velocityZ == 0 && !playerDetected && objectReference!= null && outofObjectReference)
        {
            rotToPlayer = Quaternion.LookRotation(dirToObjectReference, transform.up);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, rotVelocity * Time.deltaTime);
    }
    void AddExtraGravityForce()
    {
        if (!rb.isKinematic && !stayonShip)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y -= extraGravityFactor * Time.fixedDeltaTime;
            rb.linearVelocity = vel;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        float yVelocity = Utils.limitValue(collision.relativeVelocity.y, FMODConstants.LIMIT_SOUND_VALUE);
        
        impactFloor.setParameterByName(FMODConstants.SPEED, yVelocity);
        switch (tag)
        {
            case "SueloPantanoso":
                FMODUtils.setTerrainParametersAndStart3D(impactFloor, FMODConstants.MATERIAL.MUD, transform);
                break;
            case "Balancin":
            case "SueloMadera":
            case "Box":
                if (Utils.IsCollisionAboveEnemies(collision, transform.position.y))
                {
                    FMODUtils.setTerrainParametersAndStart3D(impactFloor, FMODConstants.MATERIAL.WOOD, transform);
                }
                else
                {
                    float collisionForce = Utils.limitValue(collision.relativeVelocity.magnitude + 3, FMODConstants.LIMIT_SOUND_VALUE);
                    impactObjetc.setParameterByName(FMODConstants.SPEED, collisionForce);
                    FMODUtils.play3DSound(impactObjetc, transform);
                }
                break;
            case "SueloPiedra":
                FMODUtils.setTerrainParametersAndStart3D(impactFloor, FMODConstants.MATERIAL.STONE, transform);
                break;
            default:
                // Handle other cases or do nothing
                break;
        }
        if (isObjetEqualsPlayer(collision.gameObject))
        {
            float collisionForce = collision.relativeVelocity.magnitude;

            ContactPoint[] contacts = collision.contacts;
            if (contacts != null && contacts.Length > 0)
            {
                Rigidbody rbPlayer = collision.gameObject.GetComponent<Rigidbody>();
                float normalY = contacts[0].normal.y;
                if (normalY < compareTop())
                {
                    if (state == State.SMASH)
                    {
                        BallMouseHit.start();
                        timeKnockOut.stopTimer();
                        timeKnockOut.startTimer();
                    }
                    state = State.SMASH;
                    changeCollider(true);

                    if (canApplyImpulse)
                    {

                        float impact = collision.relativeVelocity.y * -1;
                        Debug.Log("Impact: " + impact);
                        if (impact > 0)
                        {
                            if (impact >= 15)
                            {
                                impact = 12;
                            }
                            else if (impact > 9 && impact < 15)
                            {
                                impact = 9;
                            }
                            else if (impact < 5)
                            {
                                impact = 5;
                            }
                            rbPlayer.linearVelocity = new Vector3(rbPlayer.linearVelocity.x, 0, 0);
                            rbPlayer.AddForce(new Vector3(0, impact, 0), ForceMode.Impulse);
                            StartCoroutine(ImpulseCooldown());
                        }
                    }
                }
                else
                {
                    // Obtenemos la dirección en X hacia el otro objeto
                    float directionToOtherX = Mathf.Sign(collision.transform.position.x - transform.position.x);

                    // Obtenemos la dirección en X de la velocidad de nuestro objeto
                    float velocityDirectionX = Mathf.Sign(rb.linearVelocity.x);

                    // Comparamos las dos direcciones
                    if (directionToOtherX == velocityDirectionX && collisionForce > 2 && rb.linearVelocity.magnitude > 1) {
                        float forceX = rb.linearVelocity.x;
                        if (forceX > ForceXImpact)
                        {
                            forceX = ForceXImpact;
                        }
                        rbPlayer.AddForce(new Vector3(forceX, 0, 0), ForceMode.Impulse);
                        collision.gameObject.GetComponent<Player>().addDamage();
                        state = State.SMASH;
                        BallMouseHit.start();
                        changeCollider(true);
                    }

                }

            }
        }

    }

    private IEnumerator ImpulseCooldown()
    {
        canApplyImpulse = false; // Desactiva el impulso
        yield return new WaitForSeconds(impulseCooldown); // Espera el tiempo de cooldown
        canApplyImpulse = true; // Reactiva el impulso
    }
    private void OnCollisionStay(Collision collision)
    {
        if (isObjetEqualsPlayer(collision.gameObject))
        {
            contactBall = true;
        }
        switch (collision.gameObject.tag)
        {
            case "Balancin":
                stayonShip = true;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Water.TAG))
        {
            FMODUtils.setTerrainParametersAndStart3D(impactFloor, FMODConstants.MATERIAL.WATER, transform);
            particlesWaterController.launchParticlesWaterEnter();
        }
        
    }


    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case Water.TAG:
                if (state != State.SWIMMING)
                {
                    timeKnockOut.stopTimer();
                    timeRecover.stopTimer();
                    state = State.SWIMMING;
                    if (!timeSwimming.activated)
                    {
                        timeSwimming.startTimer();
                    }
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Water.TAG))
        {
            FMODUtils.setTerrainParametersAndStart3D(exitTerrain, FMODConstants.MATERIAL.WATER, transform);
            particlesWaterController.launchParticlesWaterExit();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Balancin":
                stayonShip = false;
                break;
        }
        if (isObjetEqualsPlayer(collision.gameObject))
        {
            contactBall = false;
        }
    }

    public bool isStayonShip()
    {
        return stayonShip;
    }

    public bool isSmash()
    {
        return state == State.SMASH;
    }
    public void resetObject()
    {
        outofObjectReference = false;
        playerDetected = false;
        state = State.NONE;
        changeCollider(false);
        transform.localPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        if (!rb.isKinematic)
        {
            rb.linearVelocity = new Vector3(0, 0, 0);
        }
    }

    private bool isObjetEqualsPlayer(GameObject gameObject)
    {
        return gameObject == player;
    }
    private void setRecovery()
    {
        if (state == State.SMASH)
        {
            state = State.RECOVER;
        }
    }

    private void setNormal()
    {
        if (state == State.RECOVER)
        {
            state = State.NORMAL;
            changeCollider(false);
        }
    }

    private void playSwimming()
    {
        state = State.NORMAL;
    }
    private float compareTop()
    {
        if (normalCollider.activeSelf)
        {
            return -0.6f;
        }
        else
        {
            return -0.6f;
        }
    }
    private void changeCollider(bool smash)
    {
        normalCollider.SetActive(!smash);
        smashCollider.SetActive(smash);
    }

    private enum State
    {
        NONE,
        NORMAL,
        MOVE,
        MOVE_AGRESSIVE,
        SMASH,
        RECOVER,
        SWIMMING
    }

    private enum Animations
    {
        MOUSE_IDLE,
        MOVE,
        MOVE_AGRESIVE,
        RECOVER,
        SMASH
    }
    class CallBackSmashTimer : Timer.Callback
    {
        private MouseBall2 obj;
        public CallBackSmashTimer(MouseBall2 obj)
        {
            this.obj = obj;
        }

        public void shot()
        {
            obj.setRecovery();
        }

        public MonoBehaviour getMonoBehaviour()
        {
            return obj;
        }
    }

    class CallBackRecoverTimer : Timer.Callback
    {
        private MouseBall2 obj;
        public CallBackRecoverTimer(MouseBall2 obj)
        {
            this.obj = obj;
        }

        public void shot()
        {
            obj.setNormal();
        }

        public MonoBehaviour getMonoBehaviour()
        {
            return obj;
        }
    }


    class CallBackSwimmingTimer : Timer.Callback
    {
        private MouseBall2 obj;
        public CallBackSwimmingTimer(MouseBall2 obj)
        {
            this.obj = obj;
        }

        public void shot()
        {
            obj.playSwimming();
        }

        public MonoBehaviour getMonoBehaviour()
        {
            return obj;
        }
    }
}