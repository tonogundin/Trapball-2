using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBall : MonoBehaviour
{
    GameObject player;
    Rigidbody rb;
    bool playerDetected;
    float dirXToPlayer;
    Vector3 dirVectorToPlayer;
    float movementForce = 5;
    float bounceForce = 7.5f;
    float rotationVelocity = 10;
    float impactFromAboveOffset = 0.65f;
    float xRot;
    Vector3 checkerPos;
    [SerializeField] LayerMask enemObstacleLayer;
    [SerializeField] Mesh meshGizmos;
    [SerializeField] GameObject BORRAR;
    bool squashed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.eulerAngles.x);
        Debug.Log(NormalizeAngle(345f));
        if(playerDetected)
        {
            //Quaternion rotFromOriginToPlayer = Quaternion.LookRotation(dirVectorToPlayer, transform.up);
            //Quaternion finalRotation = Quaternion.Slerp(transform.rotation, rotFromOriginToPlayer, rotationVelocity * Time.deltaTime);
            //transform.rotation = finalRotation;
            float a = DirectionToRotation(dirVectorToPlayer);
            a -= 90; //Desfase.
            a = Mathf.Clamp(a, -22, 22);
            Debug.Log(a);
            transform.eulerAngles = new Vector3(a, transform.eulerAngles.y, transform.eulerAngles.z);

        }
    }
    private void FixedUpdate()
    {
        //Un cubo porque como rota a la hora de mirarnos, con sphere puede perder la detecci�n de obst�culos.
        Collider[] obstacles = Physics.OverlapBox(transform.GetChild(0).position, new Vector3(0.25f, 1, 0.25f), Quaternion.identity, enemObstacleLayer.value);
        Debug.Log(obstacles.Length);
        if(playerDetected && obstacles.Length == 0)
        {
            rb.isKinematic = false;
            rb.AddForce(new Vector3(Mathf.Sign(dirXToPlayer), 0, 0) * movementForce, ForceMode.Acceleration);
        }
        else if(obstacles.Length > 0)
        {
            rb.isKinematic = true;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawMesh(meshGizmos, transform.GetChild(0).position, Quaternion.identity, new Vector3(0.5f, 2, 0.5f));
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player!!");
            playerDetected = true;
            dirVectorToPlayer = (other.transform.position - transform.position).normalized;
            dirXToPlayer = dirVectorToPlayer.x;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rbPlayer = collision.gameObject.GetComponent<Rigidbody>();
            //La normal es calculada del objeto que colisiona conmigo hac�a mi, por eso negativo.
            Vector3 impactDir = -collision.GetContact(0).normal.normalized;
            if(impactDir.y > impactFromAboveOffset)
            {
                squashed = true;
            }
            else
                rbPlayer.AddForce(impactDir * bounceForce, ForceMode.Impulse);
           
        }
    }

    float DirectionToRotation(Vector3 direction)
    {
        float angle = Mathf.Atan2(Mathf.Sign(dirXToPlayer) * direction.x, direction.y) * Mathf.Rad2Deg;
        return angle;
    }

    //return angle in range -180 to 180
    float NormalizeAngle(float a)
    {
        return a - 180f * Mathf.Floor((a + 180f) / 180f);
    }
}
