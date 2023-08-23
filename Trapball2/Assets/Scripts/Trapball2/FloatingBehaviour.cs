using UnityEngine;

public class FloatingBehaviour : MonoBehaviour, IResettable
{
    [SerializeField] float depthBeforeSumerged;
    [SerializeField] float displacementAmount;
    bool floating = false;
    Rigidbody rb;
    float waterYPos;
    [SerializeField] float torque;
    float offset = 0.4f;
    float initDisplacement;
    FMOD.Studio.EventInstance BoxSplash;
    // Start is called before the first frame update
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float initialMass;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = new Vector3(rb.position.x, rb.position.y, rb.position.z);
        initialRotation = transform.rotation;
        initialMass = rb.mass;
        BoxSplash = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/ObjectWaterDrop");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float zRotation = transform.eulerAngles.z;
        //Debug.Log(zRotation);
        int turnDirection;
        if (floating)
        {
            //Si la caja está por encima del agua, entonces la variable quedará como negativa y positiva al contrario
            float displacementMultiplier = Mathf.Clamp01((waterYPos + offset - transform.position.y) / depthBeforeSumerged) * displacementAmount;
            //Así, se va aplicando una fuerza en ambas direcciones (arriba y abajo) en función de la posición de la caja respecto al agua.
            //Finalmente, se acabará cancelando la fuerza aplicada ya que las posiciones se igualarán.
            rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0), ForceMode.Acceleration);

            //1er cuadrante. Caja entra recta.
            //if(initDisplacement <= 45 || initDisplacement > 315)
            //{
            if (zRotation > 0.5f || zRotation < 359.5f)
            {
                turnDirection = zRotation > 0.5f && zRotation < 180 ? -1 : 1;
                rb.AddTorque(transform.forward * torque * turnDirection, ForceMode.Acceleration);
            }

            //}
            ////2o cuadrante
            //else if(initDisplacement > 45 && initDisplacement < 135)
            //{
            //    if (zRotation > 90.5f || zRotation < 89.5f)
            //    {
            //        Debug.Log("eee");
            //        turnDirection = zRotation > 90.5f ? 1 : -1;
            //        rb.AddTorque(transform.forward * torque * turnDirection, ForceMode.Acceleration);
            //    }
            //}

            

        }
    }

    public void resetObject()
    {
        rb.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = initialRotation;
        rb.rotation = initialRotation;
        rb.mass = initialMass;
        floating = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("WaterSurface"))
        {
            BoxSplash.start();
            initDisplacement = transform.eulerAngles.z;
            Debug.Log(initDisplacement);
            rb.mass = 10; //Para mejorar comportamiento cuando la bola se pone encima de una caja en el agua.
            waterYPos = other.bounds.center.y;
            //rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
            floating = true;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        {
            if (other.CompareTag("WaterSurface"))

            {
                BoxSplash.release();
            }
        }
    }
}
