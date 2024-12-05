using UnityEngine;

public class ColliderWaterExit : MonoBehaviour
{

    private ParticlesWaterController particlesWaterController;
    private Player player;
    public float bottomOffset = 0.27f;

    void Start()
    {
        particlesWaterController = GetComponentInParent<ParticlesWaterController>();
        player = GetComponentInParent<Player>();
        //transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + bottomOffset, player.transform.position.z);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water") && player.state != StatePlayer.NONE && player.state != StatePlayer.DEAD && player.state != StatePlayer.FINISH)
        {
            particlesWaterController.launchParticlesWaterExit();
        }
    }
}
