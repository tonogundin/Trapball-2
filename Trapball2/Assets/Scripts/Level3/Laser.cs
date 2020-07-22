using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] GameObject laserBeam;
    Material lBeamMat;
    CapsuleCollider coll;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        lBeamMat = laserBeam.GetComponent<MeshRenderer>().material;
        coll = laserBeam.GetComponent<CapsuleCollider>();
        StartCoroutine(LaserBeamBehaviour());
    }

    IEnumerator LaserBeamBehaviour()
    {
        while(true)
        {
            coll.enabled = false;
            lBeamMat.color = new Color(1, 0, 0, 0);
            yield return new WaitForSeconds(0.8f);
            lBeamMat.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            coll.enabled = true;
            lBeamMat.color = new Color(1, 0, 0, 1);
            yield return new WaitForSeconds(2f);
        }
    }
}
