using UnityEngine;

public class MouseBall2 : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;
    GameObject player;
    bool playerDetected;
    float distToPlayer;
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
    public bool isStayonShip = false;

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
        timeKnockOut = new Timer(3000, new CallBackTimer(this));
        timeRecover = new Timer(2500, new CallBackTimer(this));
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
                state = State.MOVE;
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
                    rb.AddForce(new Vector3(Mathf.Sign(distToPlayer), 0, 0) * movementForce, ForceMode.Acceleration);
                }
            }
            else if (obstacles.Length > 0)
            {
                //Reinicio de físicas.
                rb.isKinematic = true;
                rb.isKinematic = false;
                velocityBreak = false;
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
        animations();
    }

    private void animations()
    {
        if (antState != state)
        {
            antState = state;
            switch(state)
            {
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
        if (!isStayonShip)
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
            if (state != State.SMASH)
            {
                ContactPoint[] contacts = collision.contacts;
                if (contacts != null && contacts.Length > 0)
                {
                    float normalY = contacts[0].normal.y;
                    if (normalY < -0.6f)
                    {
                        timeKnockOut.startTimer();
                        state = State.SMASH;
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer == 9)
        {
            isStayonShip = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            isStayonShip = false;
        }
    }

    private void changeStateTimer()
    {
        switch(state)
        {
            case State.SMASH:
                state = State.RECOVER;
                timeRecover.startTimer();
                break;
            case State.RECOVER:
                state = State.NORMAL;
                break;
        }
    }

    private enum State
    {
        NONE,
        NORMAL,
        MOVE,
        MOVE_AGRESSIVE,
        SMASH,
        RECOVER
    }

    class CallBackTimer : Timer.Callback
    {
        private MouseBall2 obj;
        public CallBackTimer(MouseBall2 obj)
        {
            this.obj = obj;
        }

        public void shot()
        {
            obj.changeStateTimer();
        }

        public MonoBehaviour getMonoBehaviour()
        {
            return obj;
        }
    }
}
