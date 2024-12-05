using UnityEngine;

public class ParticlesSmokeWaterController : MonoBehaviour
{


    private ParticlesSmoke particles;
    // Start is called before the first frame update
    void Start()
    {
        // Encuentra los GameObjects por nombre y obtiene el componente ParticlesWater
        particles = transform.Find("ParticlesSmokeWater").GetComponent<ParticlesSmoke>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void launchParticles()
    {
        particles.Explode();
    }

    public void stopParticles()
    {
        //particles.resetObject();
    }
}