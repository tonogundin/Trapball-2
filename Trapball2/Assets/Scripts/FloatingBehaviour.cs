using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBehaviour : MonoBehaviour
{
    [SerializeField] float depthBeforeSumerged;
    [SerializeField] float displacementAmount;
    bool floating;
    Rigidbody rb;
    float waterYPos;
    [SerializeField] float torque;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float zRotation = transform.eulerAngles.z;
        int turnDirection;
        if (floating)
        {
            //Si la caja está por encima del agua, entonces la variable quedará como negativa y positiva al contrario
            float displacementMultiplier = Mathf.Clamp01((waterYPos - transform.position.y) / depthBeforeSumerged) * displacementAmount;
            //Así, se va aplicando una fuerza en ambas direcciones (arriba y abajo) en función de la posición de la caja respecto al agua.
            //Finalmente, se acabará cancelando la fuerza aplicada ya que las posiciones se igualarán.
            rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0), ForceMode.Acceleration);

            if (zRotation > 0.2f && zRotation < 359.8f)
            {
                turnDirection = zRotation > 0.2f && zRotation < 180 ? -1 : 1;
                rb.AddTorque(transform.forward * torque * turnDirection, ForceMode.Acceleration);
            }
            

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Water"))
        {
            rb.mass = 10; //Para mejorar comportamiento cuando la bola se pone encima de una caja en el agua.
            waterYPos = other.transform.position.y;
            //rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
            floating = true;
        }
    }
}
