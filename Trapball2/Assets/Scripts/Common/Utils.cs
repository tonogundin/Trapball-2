using UnityEngine;

public class Utils 
{
    public static bool IsCollisionAbove(Collision collision, float positionY)
    {
        // Obtener la posición en Y de tu objeto
        float myYPosition = positionY;

        // Obtener la posición en Y de la caja
        float otherObjectYPosition = collision.gameObject.transform.position.y + 0.35f;

        // Comparar las posiciones en Y
        return myYPosition > otherObjectYPosition;
    }
    public static bool IsCollisionAboveEnemies(Collision collision, float positionY)
    {
        // Obtener la posición en Y de tu objeto
        float myYPosition = positionY;

        // Obtener la posición en Y de la caja
        float otherObjectYPosition = collision.gameObject.transform.position.y;

        // Comparar las posiciones en Y
        return myYPosition > otherObjectYPosition;
    }
    public static bool IsCollisionAboveMe(Collision collision, float positionY)
    {
        // Obtener la posición en Y de tu objeto
        float myYPosition = positionY;

        // Obtener la posición en Y de la caja
        float otherObjectYPosition = collision.gameObject.transform.position.y;

        // Comparar las posiciones en Y
        return myYPosition < otherObjectYPosition;
    }

    public static float limitValue(float value, float maxValue)
    {
        if (value > maxValue)
        {
            value = maxValue;
        }
        return value;
    }

    public static void SanitizeRigidbody(Rigidbody rb)
    {
    }
    /*
        if (rb == null) return;

        // Sanear la velocidad lineal
        Vector3 linearVelocity = rb.linearVelocity;
        linearVelocity.x = float.IsNaN(linearVelocity.x) || float.IsInfinity(linearVelocity.x) ? 0 : linearVelocity.x;
        linearVelocity.y = float.IsNaN(linearVelocity.y) || float.IsInfinity(linearVelocity.y) ? 0 : linearVelocity.y;
        linearVelocity.z = float.IsNaN(linearVelocity.z) || float.IsInfinity(linearVelocity.z) ? 0 : linearVelocity.z;
        rb.linearVelocity = linearVelocity;

        // Sanear la velocidad angular
        Vector3 angularVelocity = rb.angularVelocity;
        angularVelocity.x = float.IsNaN(angularVelocity.x) || float.IsInfinity(angularVelocity.x) ? 0 : angularVelocity.x;
        angularVelocity.y = float.IsNaN(angularVelocity.y) || float.IsInfinity(angularVelocity.y) ? 0 : angularVelocity.y;
        angularVelocity.z = float.IsNaN(angularVelocity.z) || float.IsInfinity(angularVelocity.z) ? 0 : angularVelocity.z;
        rb.angularVelocity = angularVelocity;

        // Sanear la posición
        Vector3 position = rb.position;
        position.x = float.IsNaN(position.x) || float.IsInfinity(position.x) ? 0 : position.x;
        position.y = float.IsNaN(position.y) || float.IsInfinity(position.y) ? 0 : position.y;
        position.z = float.IsNaN(position.z) || float.IsInfinity(position.z) ? 0 : position.z;
        rb.position = position;
    }
    */

}
