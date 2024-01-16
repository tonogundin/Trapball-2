using UnityEngine;

public class ParticlesExplosion : MonoBehaviour, IResettable
{
    GameObject player;
    private void Awake()
    {

    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Explode()
    {
        transform.SetParent(null);
        GetComponent<ParticleSystem>().Play();
    }

    public void resetObject()
    {
        transform.SetParent(player.transform);
        GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
    }
}
