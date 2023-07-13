using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    bool jumpEnabled = true;
    bool bombEnabled;
    bool touchFloor = false;
    bool oldStateTouchfloor = false;
    bool freeFall;
    CameraShake camShakeScript;
    FMOD.Studio.EventInstance playerSoundroll;
    FMOD.Studio.EventInstance impactFloor;
    FMOD.Studio.EventInstance impactObjetc;
    FMOD.Studio.EventInstance underWater;
    FMOD.Studio.EventInstance impactWater;

    public Vector2 velocityBall;

    public State state = State.NORMAL;

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
        state = State.NORMAL;
        camShakeScript = GameManager.gM.cam.GetComponent<CameraShake>();
        playerSoundroll = FMODUnity.RuntimeManager.CreateInstance("event:/Desplazamiento/SFXPlayerRollMud");
        impactFloor = FMODUnity.RuntimeManager.CreateInstance("event:/Saltos/ImpactoTerreno");
        impactObjetc = FMODUnity.RuntimeManager.CreateInstance("event:/Objetos/ImpactObject");
        underWater = FMODUnity.RuntimeManager.CreateInstance("event:/Ambientes/AmbienteUnderwater");
        impactWater = FMODUnity.RuntimeManager.CreateInstance("event:/Saltos/ImpactWater");
        playerSoundroll.start();
    }

    void Update()
    {
        MovementInput();
        JumpInput();
        checkSoundRoll();
        
    }
    private float GetComponent(float x) 
    { 
        throw new NotImplementedException();
    }
    private void checkSoundRoll()
    {
        if (oldStateTouchfloor != touchFloor)
        {
            if(touchFloor)
            {
                playerSoundroll.setParameterByName("onTheFloor", 1);
            } else
            {
                playerSoundroll.setParameterByName("onTheFloor", 0); 
            }
            oldStateTouchfloor = touchFloor;
        }

    }
    void FixedUpdate()
    {
        rb.AddForce(new Vector3(h, 0, 0) * movementForce, ForceMode.Force); //Para movimiento.
        ManageExtraGravity();
        
        if(!freeFall)
        {
            ManageBallSpeed();
        }
            

        velocityBall = new Vector2(rb.velocity.x, rb.velocity.y);
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(50, 50, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperCenter;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        string text = string.Format("X: {0:0.000} Y:{1:0.000}", velocityBall.x, velocityBall.y);
        GUI.Label(rect, text, style);
    }

    void MovementInput()
    {
#if UNITY_STANDALONE
        h = Input.GetAxisRaw("Horizontal");
#endif
#if UNITY_ANDROID
                    h = Input.acceleration.x * 2;
#endif
        playerSoundroll.setParameterByName("speed", velocityBall.x);
        impactFloor.setParameterByName("speed", velocityBall.y);
        impactObjetc.setParameterByName("speed", velocityBall.y+velocityBall.x);

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
                        SimpleJump();

                    }
                }
            }
            //Implementación golpe bomba.
            else //Si mantengo y no estoy tocando el suelo....
            {
                if (state == State.JUMP)
                {
                    bombForce += (bombDelta * Time.deltaTime);

                    //Creo que con un límite pequeño debería ser más que suficiente....
                    if (bombForce > 2.5f)
                    {
                        BombJump();
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
                SimpleJump();
                
            } else
            {
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/SaltoHigh", GetComponent<Transform>().position);
            }
            jumpEnabled = true;
            bombEnabled = true;
        }
    }

    void SimpleJump()
    {
        state = State.JUMP;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //Para salto.
        jumpForce = 0;
        playerSoundroll.setVolume(0);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/SaltoLow", GetComponent<Transform>().position);
    }
    void BombJump()
    {
        state = State.BOMBJUMP;
        jumpEnabled = false; //Una vez ejecutado el golpe bomba, deshabilitamos el salto --> Sólo se habilita si se suelta el ratón durante el rebote.
        coll.material = bouncy; //Le ponemos un material rebotante.
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //Cambiamos a dinámico por si atraviesa.
        rb.AddForce(Vector3.down * 1.5f, ForceMode.Impulse);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/SaltoBomba", GetComponent<Transform>().position);
        playerSoundroll.setVolume(0);
    }

    IEnumerator StatusNormal()
    {
        yield return new WaitForSeconds(1f);
        state = State.NORMAL;
    }
    void EndBombJump()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/ImpactoTerrenoBomba", GetComponent<Transform>().position);
        StartCoroutine(StatusNormal());
        bombForce = 0; //Si no, bombForce empieza a contar desde el último intento de carga.
        StartCoroutine(camShakeScript.Shake(0.10f, 0.15f));
        coll.material = null;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //Volvemos a discreto para consumir menos recursos.
        playerSoundroll.setVolume(1);
    }
    bool TouchingFloor()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position + offset, 0.1f, jumpable.value);
        if (colls.Length > 0)
        {
            playerSoundroll.setVolume(1);
            //Significa que he caido tras un golpe bomba.
            if (bombForce > 2.5f)  //currentGravityFactor != initGravityFactor
            {
                EndBombJump();
            }

            else if (touchFloor == false)
            {
                //FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/ImpactoTerreno", GetComponent<Transform>().position);
            }
            if (state != State.NORMAL &&  rb.velocity.y >= 0)
            {
                //state = State.NORMAL;
            }
            bombEnabled = false;
            touchFloor = true;
            return true;
        }
        else
        {
            touchFloor = false;
            return false;
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SueloPantanoso"))
        {
            playerSoundroll.setParameterByName("Terrain", 0);
            impactFloor.setParameterByName("Terrain", 0);
            impactFloor.start();
        }

        else if (collision.gameObject.CompareTag("SueloMadera"))
        {
            playerSoundroll.setParameterByName("Terrain", 1);
            impactFloor.setParameterByName("Terrain", 1);
            impactFloor.start();
        }

        else if (collision.gameObject.CompareTag("Box"))
        {
            playerSoundroll.setParameterByName("Terrain", 1);
            impactFloor.setParameterByName("Terrain", 1);
            impactFloor.start();
        }
       else if (collision.gameObject.CompareTag("SueloPiedra"))
        {
            playerSoundroll.setParameterByName("Terrain", 2);
            impactFloor.setParameterByName("Terrain", 2);
            impactFloor.start();
        }

     



            if (collision.gameObject.CompareTag("Box"))

            {
                impactObjetc.start();
                impactObjetc.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }


        }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            rb.velocity = new Vector3(rb.velocity.x, -0.5f, rb.velocity.z);
            impactWater.start();
            underWater.start();
        }
            

        else if (other.CompareTag("TubeEnter"))
            freeFall = true;

        else if (other.CompareTag("TubeExit"))
            freeFall = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water")) 
        {
            currentGravityFactor = -5f;
            rb.angularDrag = 4;
            rb.drag = 2.2f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            currentGravityFactor = initGravityFactor;
            rb.angularDrag = 0.05f;
            rb.drag = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Saltos/ImpactoTerrenoBomba", GetComponent<Transform>().position);
            underWater.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    

}
    public void Die()
    {
        GameManager.gM.ChangeGravityScale(-9.81f); //También cambio la gravedad aquí porque si no se nota más gravedad en las partículas.
        particles.Explode();
        GameManager.gM.InstantiateNewBall(2, GameManager.gM.initPosForPlayer);
        Destroy(gameObject);
        Handheld.Vibrate();
    }

    public bool isTouchFloor()
    {
        return touchFloor;
    }

    public enum State
    {
        NORMAL,
        JUMP,
        BOMBJUMP,
        DEAD
    }
}