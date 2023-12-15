using UnityEngine;
using System.Collections;


public class Beatle : MonoBehaviour
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
    public GameObject normalCollider;
    public GameObject smashCollider;
    public float ForceXImpact = 7;
    public GameObject objectReference;
    public float maxPositionZ;

    public State state;
    public State antState;
    private State initialState;

    private Vector3 initialPosition;

    private float velocityZ = 0;

    const string animBeatleIdle = "BeatleIdle";
    const string animBeatleMove = "BeatleMove";
    const string animBeatleMoveAgressive = "BeatleMoveAgressive";
    const string animBeatleFall = "BeatleFall";
    const string animBeatlePreAttack = "BeatlePreAttack";
    const string animBeatleAttack = "BeatleAttack";

    private const int layerPlatform = 9;
    
    public float waitTimePreAttack = 0.75f; // Tiempo de espera en segundos
    public float waitTimeAttack = 0.25f; // Tiempo de espera en segundos

    public bool rotateDown = true;



    // Start is called before the first frame update
    void Start()
    {
        initialState = state;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    void Update()
    {
        if (isActiveAndEnabled)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
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
                    }
                }
                else
                {
                    playerDetected = false;
                }
                if (!isDetectedPlayerNear() && state == State.NORMAL)
                {
                    
                }
            }
            if (state != State.PREPARE_ATTACK && state != State.ATTACK)
            {
                ManageRotation();
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

    private bool isDetectPlayer()
    {
        distToPlayer = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);

        if (state == State.HANG)
        {
            return Mathf.Abs(distToPlayer.x) < 2.5f // tenga un rango x para visualizarlo desde la altura.
                   && distToPlayer.y < -1 // el player esté por debajo del beatle en y.
                   && distToPlayer.y > -10; // que el player no esté a una distancia muy grande en altura.
        } else
        {
            return Mathf.Abs(distToPlayer.x) <= 10f;
        }
    }

    private bool isDetectedPlayerNear()
    {
        return Mathf.Abs(distToPlayer.x) <= 0.75f
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
                if (state != State.HANG && state != State.FALL && state != State.NAIL && state != State.PREPARE_ATTACK && state != State.ATTACK)
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
                    StartCoroutine(changeStateAttackAndNormal());
                    break;
                case State.HANG:

                    break;
                case State.FALL:
                        animator.SetTrigger(animBeatleFall);
                        break;
                case State.MOVE:
                    animator.SetTrigger(animBeatleMove);

                    break;
                case State.MOVE_AGRESSIVE:
                    animator.SetTrigger(animBeatleMoveAgressive);
                    break;
                case State.PREPARE_ATTACK:
                    animator.SetTrigger(animBeatlePreAttack);
                    break;
                case State.ATTACK:
                    animator.SetTrigger(animBeatleAttack);
                    break;
            }
        }
    }

    void ManageRotation()
    {
        if (state != State.HANG && state != State.FALL && state != State.NAIL)
        {
            float rotVelocity = 2.5f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0); // Restricción en Z

            Vector3 directionToTarget = (objectReference != null && outofObjectReference) ? dirToObjectReference : dirToPlayer;
            Quaternion rotToPlayer = Quaternion.LookRotation(directionToTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, rotVelocity * Time.deltaTime);
        }
    }

    void AnimateRotation(bool rotateDown)
    {
        // Configura los valores para la rotación hacia abajo y hacia arriba
        float downRotation = -10f; // Ángulo al que rota hacia abajo
        float upRotation = 15f; // Ángulo al que rota hacia arriba
        float rotationSpeed = rotateDown ? 0.5f : 3; // Velocidad de la rotación

        // Calcula la rotación deseada basada en el estado
        float targetRotationY = rotateDown ? downRotation : upRotation;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0); // Restricción en Z

        Vector3 directionToTarget = dirToPlayer;
        directionToTarget.y = directionToTarget.y + targetRotationY;
        Quaternion rotToPlayer = Quaternion.LookRotation(directionToTarget, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, rotationSpeed * Time.deltaTime);
    }

    IEnumerator changeStateAttackAndNormal()
    {
        yield return new WaitForSeconds(2);
        state = State.PREPARE_ATTACK;
        rotateDown = true;
        yield return new WaitForSeconds(waitTimePreAttack);
        rotateDown = false;
        state = State.ATTACK;
        yield return new WaitForSeconds(waitTimeAttack);
        rotateDown = true;
        state = State.NORMAL;
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
            float collisionForce = collision.relativeVelocity.magnitude;

            ContactPoint[] contacts = collision.contacts;
            if (contacts != null && contacts.Length > 0)
            {
                Rigidbody rbPlayer = collision.gameObject.GetComponent<Rigidbody>();
                float normalY = contacts[0].normal.y;
                if (normalY < compareTop())
                {
                    float impact = collision.relativeVelocity.y * -1;
                    if (impact > 0)
                    {
                        if (impact > 9)
                        {
                            impact = 9;
                        }
                        if (impact < 5)
                        {
                            impact = 5;
                        }
                        rbPlayer.AddForce(new Vector3(0, impact, 0), ForceMode.Impulse);
                    }
                }
                else
                {
                    // Obtenemos la dirección en X hacia el otro objeto
                    float directionToOtherX = Mathf.Sign(collision.transform.position.x - transform.position.x);

                    // Obtenemos la dirección en X de la velocidad de nuestro objeto
                    float velocityDirectionX = Mathf.Sign(rb.velocity.x);

                    // Comparamos las dos direcciones
                    if (directionToOtherX == velocityDirectionX && collisionForce > 2 && rb.velocity.magnitude > 1)
                    {
                        float forceX = rb.velocity.x;
                        if (forceX > ForceXImpact)
                        {
                            forceX = ForceXImpact;
                        }
                        rbPlayer.AddForce(new Vector3(forceX, 0, 0), ForceMode.Impulse);
                        collision.gameObject.GetComponent<Player>().addDamage();
                    }

                }

            }
        }

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
            state = State.NAIL;
            StartCoroutine(stateNormal());
        }
    }

    IEnumerator stateNormal()
    {
        yield return new WaitForSeconds(0.5f);
        state = State.NORMAL;
    }

    public bool isStayonShip()
    {
        return stayonShip;
    }

    public void resetObject()
    {
        outofObjectReference = false;
        playerDetected = false;
        state = initialState;
        transform.localPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        if (!rb.isKinematic)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
    }

    private bool isObjetEqualsPlayer(GameObject gameObject)
    {
        return gameObject == player;
    }

    private float compareTop()
    {
        return -0.6f;
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
        PREPARE_ATTACK,
        ATTACK
     }

    private enum Animations
    {
        IDLE,
        MOVE,
        FALL,
    }

}
