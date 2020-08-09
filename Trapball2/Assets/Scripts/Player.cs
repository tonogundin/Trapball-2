using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    float h;
    float speedLimit = 5f;
    float movementForce = 10f;
    [SerializeField] LayerMask ground;
    ParticlesExplosion particles;
    Vector3 offset;
    float jumpForce;
    [SerializeField] float jumpDelta; //Define cuánto de rápido se alcanza el límite de fuerza de salto.
    [SerializeField] float jumpLimit; //Define la mayor fuerza de salto posible a aplicar.
    [SerializeField] float gravityFactor; //Añade un extra de gravedad para saltos más fluidos y rápidos. Tener en cuenta: A mayor factor, más nos costará saltar --> Incrementar jumpLimit
    bool shouldJump;
    bool jumpEnabled = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        particles = transform.GetChild(0).GetComponent<ParticlesExplosion>();
        offset = new Vector3(0, 0, -0.5f);
        Debug.Log("vuelvo");
        GameManager.gM.ChangeGravityScale(Physics.gravity.y);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        MovementInput();
        JumpInput();
    }
    void FixedUpdate()
    {
        rb.AddForce(new Vector3(h, 0, 0) * movementForce, ForceMode.Force); //Para movimiento.
        if (shouldJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //Para salto.
            jumpForce = 0;
            shouldJump = false;
        }
        ManageExtraGravity();
        ManageBallSpeed();
        //if(TouchingFloor())
        //{
        //    //GameManager.gM.ChangeGravityScale(-29.43f);
        //    ManageBallSpeed();

        //}
        //else
        //{
        //   // GameManager.gM.ChangeGravityScale(-9.81f);
        //}
        //NotJumpWithMovingPlatform();
    }

    void MovementInput()
    {
    #if UNITY_STANDALONE
            h = Input.GetAxisRaw("Horizontal");
    #endif
    #if UNITY_ANDROID
                    h = Input.acceleration.x * 2;
    #endif
    }
    void JumpInput()
    {
        //Primero hay que comprobar si tenemos salto disponible: 
        //Si se ha ejecutado uno anterior, no podemos hacer otro hasta que no se levante el ratón.
        if (jumpEnabled)
        {
            if (Input.GetMouseButton(0))
            {
                //Si se está tocando el suelo y podemos seguir cargando hasta el límite ...
                if (TouchingFloor() && jumpForce <= jumpLimit)
                {
                    jumpForce += (jumpDelta * Time.deltaTime);

                    // Y si se alcanza el 30 % del límite, se saltará el máximo (100%) de forma automática.
                    if (jumpForce > jumpLimit * 0.30f)
                    {
                        jumpForce = jumpLimit;
                        //Y además se salta la lectura de levantar el ratón en este mismo frame.
                        jumpEnabled = false;
                        shouldJump = true;
                    }

                }

            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Si al levantar el ratón no se alcanza ni siquiera el 30%...
            if (jumpForce > 0 && jumpForce <= jumpLimit * 0.30f)
            {
                //Se aplicará el 70% del límite.
                jumpForce = jumpLimit * 0.70f;
                shouldJump = true;
            }
            jumpEnabled = true;
        }
    }

    bool TouchingFloor()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position + offset, 0.5f, ground.value);
        if (colls.Length > 0)
        {
            return true;
        }
        else
            return false;
    }
    void ManageExtraGravity()
    {
        if (!TouchingFloor())
        {
            Vector3 vel = rb.velocity;
            vel.y -= gravityFactor * Time.fixedDeltaTime;
            rb.velocity = vel;
        }
    }
    void ManageBallSpeed()
    {
        //Límite de velocidad
        if (Mathf.Abs(rb.velocity.x) > speedLimit)
        {
            rb.velocity = new Vector3(speedLimit * Mathf.Sign(rb.velocity.x), rb.velocity.y, rb.velocity.z);
        }
        //Ayuda para que no cueste tanto dejar la bola quieta
        //Si la bola a penas se mueve, no hay input de usuario y no está en una rampa (y == 0 + TouchingFloor) se parará por completo.
        else if (Mathf.Abs(rb.velocity.x) < 0.3f && h == 0 && rb.velocity.y == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Spikes") || other.CompareTag("LaserBeam"))
        //{
        //    Die();
        //}
    }
    public void Die()
    {
        GameManager.gM.ChangeGravityScale(-9.81f); //También cambio la gravedad aquí porque si no se nota más gravedad en las partículas.
        particles.Explode();
        GameManager.gM.InstantiateNewBall(2, GameManager.gM.initPosForPlayer);
        Destroy(gameObject);
        Handheld.Vibrate();
    }
    

}
