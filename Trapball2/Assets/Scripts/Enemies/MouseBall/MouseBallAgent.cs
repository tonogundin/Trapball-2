using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseBallAgent : MonoBehaviour
{
    NavMeshAgent agent;
    bool playerDetected, squashed;
    GameObject player;
    float impactFromAboveOffset = 0.65f;
    float bounceForce = 7.5f;
    float squashedTime = 10;
    Vector3 dirToPlayer;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerDetected && !squashed)
        {
            agent.SetDestination(player.transform.position);
            dirToPlayer = (player.transform.position - transform.position).normalized;
            float a = DirectionToRotation(dirToPlayer);
            a -= 90; //Desfase.
            a = Mathf.Clamp(a, -22, 22);
            Debug.Log(a);
            transform.eulerAngles = new Vector3(a, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else if(squashed)
        {
            squashedTime -= Time.deltaTime;
            if(squashedTime <= 0)
            {
                squashed = false;
                squashedTime = 10;
                rb.isKinematic = true;
                agent.enabled = true;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Player.TAG))
        {
            playerDetected = true;
            player = other.gameObject;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Player.TAG))
        {
            Rigidbody rbPlayer = collision.gameObject.GetComponent<Rigidbody>();
            //La normal es calculada del objeto que colisiona conmigo hacía mi, por eso negativo.
            Vector3 impactDir = -collision.GetContact(0).normal.normalized;
            if (impactDir.y > impactFromAboveOffset)
            {
                squashed = true;
                rb.isKinematic = false;
                agent.enabled = false;
            }
            else if(!squashed)
                rbPlayer.AddForce(impactDir * bounceForce, ForceMode.Impulse);

        }
    }
    float DirectionToRotation(Vector3 direction)
    {
        float angle = Mathf.Atan2(Mathf.Sign(direction.x) * direction.x, direction.y) * Mathf.Rad2Deg;
        return angle;
    }
}
