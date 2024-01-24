using System.Collections;
using UnityEngine;

public class BreakableWood : MonoBehaviour, IResettable
{
    Rigidbody rb;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    State state = State.NORMAL;
    public float collisionForceActive = 10;
    public float velocityImpactActive = -10;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = new Vector3(rb.position.x, rb.position.y, rb.position.z);
        initialScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        initialRotation = transform.rotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && state == State.NORMAL) {
            // Obtiene la magnitud de la velocidad relativa (fuerza del impacto)
            float collisionForce = collision.relativeVelocity.magnitude;

            // Obtiene la velocidad relativa en el eje y
            float yVelocity = collision.relativeVelocity.y;

            // Si la fuerza del impacto es suficientemente fuerte y la velocidad en y es negativa
            if (collisionForce > collisionForceActive && yVelocity < velocityImpactActive)
            {
                state = State.BREAK;
                rb.isKinematic = false;
                StartCoroutine(Disappear());
                FMODUtils.playOneShot(FMODConstants.OBJECTS.PLATFORM_CRACK, transform.position);
            }
        }
    }


    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(0.5f);
        while(transform.localScale.y > 0.1f)
        {
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.SetActive(false);
    }
    public void resetObject()
    {
        gameObject.SetActive(true);
        rb.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = initialRotation;
        rb.rotation = initialRotation;
        transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
    }
    public enum State
    {
        NORMAL,
        BREAK
    }
}
