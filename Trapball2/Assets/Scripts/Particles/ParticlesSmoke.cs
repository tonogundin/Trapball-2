using UnityEngine;

public class ParticlesSmoke : MonoBehaviour, IResettable
{
    GameObject player;
    public float bottomOffset = 0.5f;


    private void Start()
    {
        player = transform.parent.gameObject;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        /*
        transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y - bottomOffset,
            player.transform.position.z
        );

        */
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
