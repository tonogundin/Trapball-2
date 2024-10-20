using System.Collections;
using UnityEngine;

public class ParticlesWaterController : MonoBehaviour
{


    private ParticlesWater particlesWaterEnter;
    private ParticlesWater particlesWaterExit;
    // Start is called before the first frame update
    void Start()
    {
        // Encuentra los GameObjects por nombre y obtiene el componente ParticlesWater
        particlesWaterEnter = transform.Find("ParticleSWaterEntry").GetComponent<ParticlesWater>();
        particlesWaterExit = transform.Find("ParticleSWaterExit").GetComponent<ParticlesWater>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void launchParticlesWaterEnter(Vector3 position)
    {
        particlesWaterEnter.Explode(position);
        StartCoroutine(delayResetParticlesWaterEnter());
    }

    public void launchParticlesWaterEnter()
    {
        particlesWaterEnter.Explode();
        StartCoroutine(delayResetParticlesWaterEnter());
    }
    public void launchParticlesWaterExit()
    {
        particlesWaterExit.Explode();
        StartCoroutine(delayResetParticlesWaterExit());
    }

    IEnumerator delayResetParticlesWaterEnter()
    {
        yield return new WaitForSeconds(1f);
        particlesWaterEnter.resetObject();
    }
    IEnumerator delayResetParticlesWaterExit()
    {
        yield return new WaitForSeconds(1f);
        particlesWaterExit.resetObject();
    }
}
