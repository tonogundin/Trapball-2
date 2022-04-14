using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBall2 : MonoBehaviour
{
    Rigidbody rb;
    GameObject player;
    bool playerDetected;
    float distToPlayer;
    float OldDirToPlayerX;
    float actualPositionX;
    Vector3 dirToPlayer;
    bool velocityBreak = false;
    float extraGravityFactor = 10;
    float movementForce = 5;
    [SerializeField] LayerMask enemObstacleLayer;
    [SerializeField] Mesh meshGizmos;
    Vector3 checkerOffset;
    public bool isStayonShip = false;
    public bool isknockedOut = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        actualPositionX = rb.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else if (!isknockedOut)
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
                ManageRotation();
            }
            actualPositionX = rb.position.x;
        }
    }
    private void FixedUpdate()
    {
        AddExtraGravityForce();
        if (!isknockedOut)
        {
            checkerOffset = new Vector3(Mathf.Sign(distToPlayer), -0.1f, 0);
            Collider[] obstacles = Physics.OverlapSphere(transform.position + checkerOffset, 0.25f, enemObstacleLayer.value);
            Debug.Log(obstacles.Length);
            if (playerDetected && obstacles.Length == 0)
            {

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
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                velocityBreak = false;
            }
        } else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
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
}
