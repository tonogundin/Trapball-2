using UnityEngine;

public class MouseBall2 : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;
    GameObject player;
    bool playerDetected;
    float distToPlayer;
    float distToPlayerY;
    float OldDirToPlayerX;
    Vector3 dirToPlayer;
    bool velocityBreak = false;
    float extraGravityFactor = 10;
    float movementForce = 5;
    [SerializeField] LayerMask enemObstacleLayer;
    [SerializeField] Mesh meshGizmos;
    Vector3 checkerOffset;
    Timer timeKnockOut;
    Timer timeRecover;
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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        state = State.NONE;
        timeKnockOut = new Timer(4000, new CallBackSmashTimer(this));
        timeRecover = new Timer(2500, new CallBackRecoverTimer(this));
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
            OldDirToPlayerX = dirToPlayer.x;
            distToPlayer = player.transform.position.x - transform.position.x;
            dirToPlayer = (player.transform.position - transform.position).normalized;
            distToPlayerY = player.transform.position.y - transform.position.y;
            if ((OldDirToPlayerX > 0 && dirToPlayer.x <0) || ( OldDirToPlayerX < 0 && dirToPlayer.x >0))
            {
                velocityBreak = true;
            }
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
            checkerOffset = new Vector3(Mathf.Sign(distToPlayer), -0.1f, 0);
            Collider[] obstacles = Physics.OverlapSphere(transform.position + checkerOffset, 0.25f, enemObstacleLayer.value);
            if (playerDetected && obstacles.Length == 0)
            {
                if (state != State.SWIMMING)
                {
                    state = State.MOVE;
                }
                if (distToPlayer > 1)
                {
                    distToPlayer = 1;
                }
                else if (distToPlayer < -1)
                {
                    distToPlayer = -1;
                }
                if (velocityBreak)
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                    velocityBreak = false;
                }
                else
                {
                    float forceY = 0;
                    if (state == State.SWIMMING)
                    {
                        forceY = Mathf.Sign(distToPlayerY);
                    }
                    rb.AddForce(new Vector3(Mathf.Sign(distToPlayer), forceY, 0) * movementForce, ForceMode.Acceleration);
                }
            }
            else if (obstacles.Length > 0)
            {
                //Reinicio de físicas.
                rb.isKinematic = true;
                rb.isKinematic = false;
                velocityBreak = false;
                if (state == State.SWIMMING)
                {
                    rb.AddForce(new Vector3(Mathf.Sign(distToPlayer), Mathf.Sign(distToPlayerY), 0) * movementForce, ForceMode.Impulse);
                }
            }
            if (!playerDetected)
            {
                state = State.NORMAL;
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                velocityBreak = false;
            }
        } else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        checkState();
    }

    private void checkState()
    {
        if (antState != state)
        {
            antState = state;
            switch(state)
            {
                case State.SWIMMING:
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
                    animator.SetTrigger(animMouseSmash);
                    break;
                case State.RECOVER:
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
        if (collision.gameObject == player)
        {
            ContactPoint[] contacts = collision.contacts;
            if (contacts != null && contacts.Length > 0)
            {
                float normalY = contacts[0].normal.y;
                if (normalY < compareTop())
                {
                    timeRecover.stopTimer();
                    timeKnockOut.stopTimer();
                    timeKnockOut.startTimer();
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
        if (collision.gameObject.layer == 9) // Platform.
        {
            stayonShip = true;
            mouseAsociateShip = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) // Agua
        {
            state = State.SWIMMING;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            stayonShip = false;
        }
    }

    public bool isStayonShip()
    {
        return stayonShip;
    }
    private void setRecovery()
    {
        if(state == State.SMASH)
        {
            state = State.RECOVER;
            timeRecover.startTimer();
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
}
