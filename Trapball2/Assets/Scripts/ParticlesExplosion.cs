using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesExplosion : MonoBehaviour
{
    Light redLight;
    GameObject lightgO;
    private void Awake()
    {
        lightgO = transform.GetChild(0).gameObject;
        redLight = lightgO.GetComponent<Light>();
    }
    public void Explode()
    {
        transform.SetParent(null);
        //Destroy(gameObject, 3);
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(LightingExplosion());
    }
    IEnumerator LightingExplosion()
    {
        //int randomFlashing = Random.Range(0, 10);
        for (int i = 0; i < 10; i++)
        {
            lightgO.SetActive(!lightgO.activeSelf);
            if(lightgO.activeSelf)
            {
                redLight.range = Random.Range(2f, 4f); //Change light range once is active.
            }
            yield return new WaitForSeconds(0.02f);
        }
        lightgO.SetActive(false);
    }
}
