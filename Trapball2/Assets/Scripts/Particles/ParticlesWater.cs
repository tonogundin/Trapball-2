using UnityEngine;

public class ParticlesWater : MonoBehaviour, IResettable
{
    GameObject player;
    public float bottomOffset = 0.5f;  // Distancia desde el centro hasta la parte inferior (ajústala según tu personaje)
    public float extraOffset = 0f;   // Offset extra para mover las partículas más abajo


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Player.TAG);
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //transform.position = new Vector3(player.transform.position.x, player.transform.position.y - bottomOffset, player.transform.position.z);
    }
    public void Explode()
    {
        transform.SetParent(null);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y - bottomOffset, player.transform.position.z);
        GetComponent<ParticleSystem>().Play();
    }

    public void resetObject()
    {
        transform.SetParent(player.transform);
        GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y - bottomOffset, player.transform.position.z);
    }
}
