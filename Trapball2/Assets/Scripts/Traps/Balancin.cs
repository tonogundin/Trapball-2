using UnityEngine;
using System.Collections;

public class Balancin : MonoBehaviour, IResettable
{
    [SerializeField] float depthBeforeSumerged;
    [SerializeField] float displacementAmount;
    bool floating;
    Rigidbody rb;
    float waterYPos;
    [SerializeField] float torque;
    public float forceX = 0f;
    float offset = 0.4f;
    GameObject player;
    float energyImpactMouse = 0f;
    float energyImpactBall = 0f;
    string mouseBall = "MouseBall";
    float forceXBalancin = 0f;
    private Vector3 oldPosition;
    GameObject mouse;
    private Vector3 initialPosition;
    public bool anclado = true;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = new Vector3(rb.position.x, rb.position.y, rb.position.z);
        setOldPosition(rb.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(Player.TAG);
        }
        
    }
    private void FixedUpdate()
    {


        if (floating)
        {
            // Calcula el desplazamiento de balanceo
            float displacementMultiplier = Mathf.Clamp01((waterYPos + offset - transform.position.y) / depthBeforeSumerged) * displacementAmount;

            if (!float.IsInfinity(displacementMultiplier) && !float.IsNaN(displacementMultiplier))
            {
                rb.AddForce(new Vector3(forceXBalancin, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0), ForceMode.Acceleration);
                if (forceXBalancin > 0)
                {
                    forceXBalancin -= 0.5f;
                }
                repositionMouse(rb.position.x - oldPosition.x);
            }

            // Controla la rotación Z
            // Calcula la rotación en Z, asegurando que 0 grados sea la posición estable
            float zRotation = transform.eulerAngles.z;
            if (!float.IsNaN(zRotation) && !float.IsInfinity(zRotation))
            {
                if (zRotation > 0.5f || zRotation < 359.5f)
                {
                    int turnDirection = zRotation > 0.5f && zRotation < 180 ? -1 : 1;
                    rb.AddTorque(transform.forward * torque * turnDirection, ForceMode.Acceleration);
                }
            }
        }
    }




    public void resetObject()
    {
        rb.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        rb.linearVelocity = new Vector3(0, 0, 0);
        anclado = true;
        setOldPosition(rb.position);
        StartCoroutine(restoreConstraints());
    }
    IEnumerator restoreConstraints()
    {
        yield return new WaitForSeconds(1f);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impact = collision.relativeVelocity.y * -1;
        if (collision.gameObject == player)
        {
            if (anclado)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
                anclado = false;
            }
            if (mouse != null && impact > 3f && mouse.GetComponent<MouseBall2>().isStayonShip())
            {
                energyImpactBall = impact;
                if (energyImpactBall > 8)
                {
                    energyImpactBall = 8;
                }
                moveBalancin(forceX);

                moveMouse(new Vector3(forceX, 0, 0), ForceMode.Acceleration);
                moveMouse(new Vector3(0, energyImpactBall, 0), ForceMode.Impulse);
                energyImpactBall = 0;
            }
        }
        if (collision.gameObject.name.Contains(mouseBall))
        {
            mouse = collision.gameObject;
            energyImpactMouse = impact;
            if (energyImpactMouse > 8)
            {
                energyImpactMouse = 8;
            }
            if (player != null && impact > 3f)
            {
                moveBalancin(forceX);
                moveBall(new Vector3(0, energyImpactMouse, 0), ForceMode.Impulse);
                energyImpactMouse = 0;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == mouseBall)
        {
            mouse = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterSurface"))
        {
            rb.mass = 10; //Para mejorar comportamiento cuando la bola se pone encima de una caja en el agua.
            waterYPos = other.bounds.center.y;
            if (anclado)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

            }
            floating = true;
        }
    }

    private void moveBalancin(float velocityX)
    {
        forceXBalancin = velocityX;
    }
    private void moveMouse(Vector3 force, ForceMode forceMode)
    {
        if (mouse != null)
        {
            Rigidbody mouseRB = mouse.GetComponent<Rigidbody>();
            mouseRB.AddForce(force, forceMode);
        }
    }

    private void moveBall(Vector3 force, ForceMode forceMode)
    {
        if (player != null && player.GetComponent<Player>().isOnBalancin())
        {
            Rigidbody playerRB = player.GetComponent<Rigidbody>();
            if (player.GetComponent<Player>().isTouchFloor() && player.GetComponent<Rigidbody>().linearVelocity.y <= 1 && player.GetComponent<Rigidbody>().linearVelocity.y >= -1)
            {
                //player.GetComponent<Player>().resetJumpForce();
                playerRB.AddForce(force, forceMode);
            }
        }
    }

    private void repositionMouse(float positionX)
    {
        if (mouse != null && mouse.GetComponent<MouseBall2>().isStayonShip() && mouse.GetComponent<MouseBall2>().isSmashProcess())
        {
            Rigidbody mouseRB = mouse.GetComponent<Rigidbody>();
            mouseRB.position = new Vector3(mouseRB.position.x + positionX, mouseRB.position.y, mouseRB.position.z);
            
        }
        setOldPosition(rb.position);
    }

    private void setOldPosition(Vector3 position)
    {
        oldPosition = new Vector3(position.x, position.y, position.z);
    }
}

