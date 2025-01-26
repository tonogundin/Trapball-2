using UnityEngine;

public class Soga : MonoBehaviour
{
    public Vector3 fuerzaInicial; // Fuerza inicial para comenzar el balanceo
    public float fuerzaImpulso = 10f; // Fuerza del impulso
    public float velocidadLimite = 0.5f; // Velocidad m�nima para cambiar de direcci�n
    public float intervaloImpulso = 2f; // Tiempo entre impulsos (en segundos)

    private Rigidbody ultimoEslabon; // Rigidbody del �ltimo eslab�n
    private float tiempoSiguienteImpulso; // Control del tiempo para aplicar el siguiente impulso
    private bool direccionDerecha = true; // Alternar direcci�n del impulso

    void Start()
    {
        // Obtener el Rigidbody del �ltimo eslab�n
        ultimoEslabon = transform.GetChild(transform.childCount - 1).GetComponent<Rigidbody>();

        // Aplicar un impulso inicial
        ultimoEslabon.AddForce(fuerzaInicial, ForceMode.Impulse);

        // Configurar el tiempo del siguiente impulso
        tiempoSiguienteImpulso = Time.time + intervaloImpulso;
    }

    void FixedUpdate()
    {
        // Cambiar de direcci�n si la velocidad es suficientemente baja
        if (ultimoEslabon.linearVelocity.magnitude < velocidadLimite)
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
        ultimoEslabon.AddForce(direccionImpulso * fuerzaImpulso, ForceMode.Impulse);

        // Alternar la direcci�n para el pr�ximo impulso
        direccionDerecha = !direccionDerecha;
    }
}
