using UnityEngine;

public class ParticlesStellaController : MonoBehaviour
{


    private ParticlesStella particles;
    // Start is called before the first frame update
    void Start()
    {
        // Encuentra los GameObjects por nombre y obtiene el componente ParticlesWater
        particles = transform.Find("ParticleStella").GetComponent<ParticlesStella>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void launchParticles()
    {
        //particles.Explode();
    }

    public void stopParticles()
    {
        //particles.resetObject();
    }
}