using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyAudioTrigger : MonoBehaviour
{
   [SerializeField] string targetTag = "Player";  // Etiqueta del objeto a detectar
   [SerializeField] int requiredEntries = 3;      // Número de entradas requeridas para destruir el collider
    private int currentEntries = 0;      // Número de entradas actuales

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra en el collider tiene la etiqueta deseada
        if (other.CompareTag(targetTag))
        {
            currentEntries++;
            // Si se han cumplido el número de entradas requeridas, destruir el collider
            if (currentEntries >= requiredEntries)
            {
                Destroy(gameObject);
            }
        }
    }
}