using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    float h;
    float speedLimit = 5f;
    float movementForce = 10f;
    [SerializeField] LayerMask jumpable;
    ParticlesExplosion particles;
    SphereCollider coll;
    Vector3 offset;
    float jumpForce;
    float bombForce;
    [SerializeField] PhysicMaterial bouncy;
    [SerializeField] float bombDelta; //Define cuánto de rápido se realizará el golpe bomba.
    [SerializeField] float jumpDelta; //Define cuánto de rápido se alcanza el límite de fuerza de salto.
    [SerializeField] float jumpLimit; //Define la mayor fuerza de salto posible a aplicar.
    [SerializeField] float initGravityFactor;
    float currentGravityFactor; //Añade un extra de gravedad para saltos más fluidos y rápidos. Tener en cuenta: A mayor factor, más nos costará saltar --> Incrementar jumpLimit
    bool shouldJump;
    bool waitDueBounce;
    float waitTimeDueBounce;
    bool jumpEnabled = true;
    bool bombEnabled;
    CameraShake camShakeScript;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<SphereCollider>();
        particles = transform.GetChild(0).GetComponent<ParticlesExplosion>();
        offset = new Vector3(0, -0.35f, 0);
        currentGravityFactor = initGravityFactor;
    }
    // Start is called before the first frame update
    void Start()
    {
        camShakeScript = GameManager.gM.cam.GetComponent<CameraShake>();
    }

    void Update()
    {
        MovementInput();
        if (!waitDueBounce)
        {
            JumpInput();
        }
        else
        {
            waitTimeDueBounce += Time.deltaTime;

            if(Input.GetMouseButtonUp(0))
                jumpEnabled = true;

            if (waitTimeDueBounce >= 0.5f)
            {
                waitDueBounce = false;
                coll.material = null;
                waitTimeDueBounce = 0;
            }
        }
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
        if (Input.GetMouseButton(0))
        {
            //Dos mecánicas diferenciadas al mantener el ratón: Desde el suelo carga de salto. Desde el aire, golpe bomba.
            if (TouchingFloor())
            {
                //Primero hay que comprobar si tenemos salto disponible: 
                //Si se ha ejecutado uno anterior, no podemos hacer otro hasta que no se levante el ratón.
                if (jumpEnabled)
                {
                    //Vamos cargando fuerza ...
                    jumpForce += (jumpDelta * Time.deltaTime);
                    // Y si se alcanza el 30 % del límite, se saltará el máximo (100%) de forma automática. HABRÍA QUE INCREMENTAR EL LÍMITE.
                    if (jumpForce > jumpLimit * 0.30f)
                    {
                        jumpForce = jumpLimit;
                        //Y además se salta la lectura de levantar el ratón en este mismo frame.
                        jumpEnabled = false;
                        shouldJump = true;
                    }
                }
            }
            //Implementación golpe bomba.
            else
            {
                if (bombEnabled)
                {
                    bombForce += (bombDelta * Time.deltaTime);

                    //Creo que con un límite pequeño debería ser más que suficiente....
                    if (bombForce > 2.5f)
                    {
                        coll.material = bouncy; //Le ponemos un material rebotante.
                        waitDueBounce = true;
                        jumpEnabled = false; //Una vez ejecutado el golpe bomba, deshabilitamos el salto --> Sólo se habilita si se suelta el ratón durante el rebote.
                        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //Cambiamos a dinámico por si atraviesa.
                        currentGravityFactor = 200;
                        bombForce = 0;
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
            bombEnabled = true;
            bombForce = 0; //Si no, bombForce empieza a contar desde el último intento de carga.
        }
    }

    bool TouchingFloor()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position + offset, 0.1f, jumpable.value);
        if (colls.Length > 0)
        {
            //Significa que he caido tras un golpe bomba.
            if (currentGravityFactor != initGravityFactor)
            {
                StartCoroutine(camShakeScript.Shake(0.10f, 0.3f));
                currentGravityFactor = initGravityFactor;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //Volvemos a discreto para consumir menos recursos.
            }
            bombEnabled = false;
            return true;
        }
        else
            return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + offset, 0.1f);
    }
    void ManageExtraGravity()
    {
        if (!TouchingFloor())
        {
            Vector3 vel = rb.velocity;
            vel.y -= currentGravityFactor * Time.fixedDeltaTime;
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