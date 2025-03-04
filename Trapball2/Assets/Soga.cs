using UnityEngine;
using System.Collections;

public class Soga : MonoBehaviour
{
    public Vector3 fuerzaInicial; // Fuerza inicial para comenzar el balanceo
    public float fuerzaImpulso = 10f; // Fuerza del impulso
    public float velocidadLimite = 0.5f; // Velocidad mónima para cambiar de dirección
    public float intervaloImpulso = 2f; // Tiempo entre impulsos (en segundos)

    private Transform lastEslabomTransform; // Rigidbody del último eslabón
    private Rigidbody lastEslabonRB; // Rigidbody del último eslabón
    private float tiempoSiguienteImpulso; // Control del tiempo para aplicar el siguiente impulso
    private bool direccionDerecha = true; // Alternar direcciónn del impulso

    private FMOD.Studio.EventInstance soundElectric;

    void Start()
    {
        soundElectric = FMODUtils.createInstance(FMODConstants.OBJECTS.ELECTRIC_CABLE);
        // Obtener el Rigidbody del último eslabón
        lastEslabomTransform = transform.GetChild(transform.childCount - 1);
        lastEslabonRB = lastEslabomTransform.GetComponent<Rigidbody>();

        // Aplicar un impulso inicial
        lastEslabonRB.AddForce(fuerzaInicial, ForceMode.Impulse);

        // Configurar el tiempo del siguiente impulso
        tiempoSiguienteImpulso = Time.time + intervaloImpulso;
        StartCoroutine(playElectricSound());
    }

    void FixedUpdate()
    {
        // Cambiar de dirección si la velocidad es suficientemente baja
        if (lastEslabonRB.linearVelocity.magnitude < velocidadLimite)
        {
            AplicarImpulso();
        }

        // Alternar el impulso basado en el tiempo
        if (Time.time >= tiempoSiguienteImpulso)
        {
            AplicarImpulso();
            tiempoSiguienteImpulso = Time.time + intervaloImpulso;
        }
    }

    void AplicarImpulso()
    {
        // Determinar la direcci�n del impulso
        Vector3 direccionImpulso = direccionDerecha ? Vector3.right : Vector3.left;

        // Aplicar el impulso
        lastEslabonRB.AddForce(direccionImpulso * fuerzaImpulso, ForceMode.Impulse);

        // Alternar la dirección para el próximo impulso
        direccionDerecha = !direccionDerecha;
    }
    private void OnDestroy()
    {
        soundElectric.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    private IEnumerator playElectricSound()
    {
        float randomSeconds = Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(randomSeconds);
        FMODUtils.play3DSound(soundElectric, lastEslabomTransform);
    }
}
