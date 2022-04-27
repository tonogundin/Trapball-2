using UnityEngine;

public class MouseBall2 : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;
    GameObject player;
    bool playerDetected;
    float distToPlayer;
    Vector3 dirToPlayer;
    float extraGravityFactor = 10;
    float movementForce = 5;
    [SerializeField] LayerMask enemObstacleLayer;
    [SerializeField] Mesh meshGizmos;
    Vector3 checkerOffset;
    Timer timeKnockOut;
    Timer timeRecover;
    Timer timeSwimming;
    private bool stayonShip = false;
    public GameObject normalCollider;
    public GameObject smashCollider;
    private bool mouseAsociateShip = false;

    const string animMouseIdle = "MouseBallIddle";
    const string animMouseMove = "MouseBallMove";
    const string animMouseMoveAgressive = "MouseBallMoveAgressive";
    const string animMouseSmash = "MouseBallSmash";
    const string animMouseRecover = "MouseBallRecover";
    

    private State state;
    private State antState;
    private bool contactBall = false;
    private int secondsKnocked = 1500;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        state = State.NONE;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        timeKnockOut = new Timer((int)(clips[(int)Animations.SMASH].length * 1000) + secondsKnocked, new CallBackSmashTimer(this));
        timeRecover = new Timer((int)(clips[(int)Animations.RECOVER].length * 1000), new CallBackRecoverTimer(this));
        timeSwimming = new Timer(150, new CallBackSwimmingTimer(this));
        changeCollider(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else if (!isSmashProcess())
        {
            distToPlayer = player.transform.position.x - transform.position.x;
            dirToPlayer = (player.transform.position - transform.position).normalized;
            if (Mathf.Abs(distToPlayer) <= 5f)
            {
                playerDetected = true;
                if (Mathf.Abs(distToPlayer) <= 0.75f)
                {
                    dirToPlayer.y = 0f; //Si estamos muy cerca del player,no rotamos en y para que no haga rotación rara.
                }
            } else
            {
                playerDetected = false;
            }
        }
        ManageRotation();
    }
    private void FixedUpdate()
    {
        AddExtraGravityForce();
        if (!isSmashProcess())
        {
            float velocityX = Mathf.Sign(distToPlayer);
            if (playerDetected)
            {
                if (state == State.NORMAL)
                {
                    state = State.MOVE;
                }
                if (state != State.SWIMMING)
                {
                    move(velocityX, 0, ForceMode.Acceleration);
                }
            } else
            {
                if (state != State.SWIMMING) {
                    state = State.NORMAL;
                    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                }  
            }
            if (state == State.SWIMMING)
            {
                move(0, 0.25f, ForceMode.Impulse);

                move(limitVelocity(velocityX, 0.25f), 0, ForceMode.Acceleration);
                if (rb.velocity.y > 5)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);
                }
                if (rb.velocity.x > 2)
                {
                    rb.velocity = new Vector3(2, rb.velocity.y, rb.velocity.z);
                }
                if (rb.velocity.x < -2)
                {
                    rb.velocity = new Vector3(-2, rb.velocity.y, rb.velocity.z);
                }
            }
            
        } else
        {
            if (!contactBall)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
        checkState();
    }

    private void move(float velocityX, float velocityY, ForceMode mode)
    {
        velocityX = limitVelocity(velocityX, 1);
        rb.AddForce(new Vector3(velocityX , velocityY, 0) * movementForce, mode);
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

    private void checkState()
    {
        // AQUI EL SONIDO CHIMO.
        if (antState != state)
        {
            antState = state;
            switch(state)
            {
                case State.SWIMMING:
                    animator.SetTrigger(animMouseIdle);
                    break;
                case State.NORMAL:
                    animator.SetTrigger(animMouseIdle);
                    break;
                case State.MOVE:
                    animator.SetTrigger(animMouseMoveAgressive);
                    break;
                case State.MOVE_AGRESSIVE:
                    animator.SetTrigger(animMouseMoveAgressive);
                    break;
                case State.SMASH:
                    timeRecover.stopTimer();
                    timeKnockOut.stopTimer();
                    timeKnockOut.startTimer();
                    animator.SetTrigger(animMouseSmash);
                    break;
                case State.RECOVER:
                    timeKnockOut.stopTimer();
                    timeRecover.stopTimer();
                    timeRecover.startTimer();
                    animator.SetTrigger(animMouseRecover);
                    break;
            }
        }
    }

    private bool isSmashProcess()
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
        Quaternion rotationAnt = new Quaternion(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, rotVelocity * Time.deltaTime);
    }
    void AddExtraGravityForce()
    {
        if (!stayonShip)
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
                float normalY = contacts[0].normal.y;
                if (normalY < compareTop())
                {
                    state = State.SMASH;
                    changeCollider(true);
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
                        player.GetComponent<Rigidbody>().AddForce(new Vector3(0, impact, 0), ForceMode.Impulse);
                    }
                }
            }
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (isObjetEqualsPlayer(collision.gameObject))
        {
            contactBall = true;
        }
        switch (collision.gameObject.layer)
        {
            case Layers.PLATFORM:
                stayonShip = true;
                mouseAsociateShip = true;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case Layers.AGUA:
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

    private void OnCollisionExit(Collision collision)
    {
        switch(collision.gameObject.layer)
        {
            case Layers.PLATFORM:
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

    private bool isObjetEqualsPlayer(GameObject gameObject)
    {
        return gameObject == player;
    }
    private void setRecovery()
    {
        if(state == State.SMASH)
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
        } else
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
