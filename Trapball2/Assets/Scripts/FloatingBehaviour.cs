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
        if(floating)
        {
            //Si la caja está por encima del agua, entonces la variable quedará como negativa y positiva al contrario
            float displacementMultiplier = Mathf.Clamp01(waterYPos - transform.position.y / depthBeforeSumerged) * displacementAmount;
            //Así, se va aplicando una fuerza en ambas direcciones (arriba y abajo) en función de la posición de la caja respecto al agua.
            //Finalmente, se acabará cancelando la fuerza aplicada ya que las posiciones se igualarán.
            rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0), ForceMode.Acceleration);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Water"))
        {
            waterYPos = other.transform.position.y;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            floating = true;
        }
    }
}
