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
        bool result = false;
        if (collision.contactCount > 0)
        {
            // Obtener el primer punto de contacto
            ContactPoint contact = collision.GetContact(0);

            // Comparar la altura (eje Y) del punto de contacto con la posición del jugador
            if (myYPosition > contact.point.y)
            {
                result = true;
            }
        }
        return result;
    }

    public static bool IsBombCollision(Collision collision, Rigidbody rb, float positionY)
    {
        // Obtener la posición en Y del objeto
        float myYPosition = positionY;

        // Validar que hay puntos de contacto
        if (collision.contactCount > 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (myYPosition > contact.point.y)
                {
                    // Verificar si la velocidad en Y es igual o mayor a 0 (hacia abajo)
                    if (rb.linearVelocity.y <= 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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
}
