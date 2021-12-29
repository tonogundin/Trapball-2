using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBall2 : MonoBehaviour
{
    Rigidbody rb;
    GameObject player;
    bool playerDetected;
    float distToPlayer;
    Vector3 dirToPlayer;
    float extraGravityFactor = 100;
    float movementForce = 5;
    [SerializeField] LayerMask enemObstacleLayer;
    [SerializeField] Mesh meshGizmos;
    Vector3 checkerOffset;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
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
                ManageRotation();
            }
        }


    }
    private void FixedUpdate()
    {
        AddExtraGravityForce();
        checkerOffset = new Vector3(Mathf.Sign(distToPlayer), -0.1f, 0);
         Collider[] obstacles = Physics.OverlapSphere(transform.position + checkerOffset, 0.25f, enemObstacleLayer.value);
        Debug.Log(obstacles.Length);
        if (playerDetected && obstacles.Length == 0)
        {
            rb.AddForce(new Vector3(Mathf.Sign(distToPlayer), 0, 0) * movementForce, ForceMode.Acceleration);
        }
        else if (obstacles.Length > 0)
        {
            //Reinicio de físicas.
            rb.isKinematic = true;
            rb.isKinematic = false;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, rotVelocity * Time.deltaTime);
    }
    void AddExtraGravityForce()
    {
        Vector3 vel = rb.velocity;
        vel.y -= extraGravityFactor * Time.fixedDeltaTime;
        rb.velocity = vel;
    }
}
