using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchForBarriers : MonoBehaviour
{
    [SerializeField] GameObject[] barriers;
    Collider[] barrierColls;

    // Start is called before the first frame update
    void Start()
    {
        barrierColls = new Collider[barriers.Length];
        for (int i = 0; i < barrierColls.Length; i++)
        {
            barrierColls[i] = barriers[i].GetComponent<Collider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines(); //Paro la corrutina de inicio de cierre de puertas por si está activa.
            OpenCloseBarriers(false, 45);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(WaitForCloseBarriers());
        }
    }

    IEnumerator WaitForCloseBarriers()
    {
        yield return new WaitForSeconds(1.3f);
        OpenCloseBarriers(true, 0);
    }

    void OpenCloseBarriers(bool collState, float degrees)
    {
        barriers[0].transform.localRotation = Quaternion.Euler(-90f, 0, degrees);
        barriers[1].transform.localRotation = Quaternion.Euler(-90f, 0, -degrees);
        foreach (Collider coll in barrierColls)
        {
            coll.enabled = collState;
        }
    }
}
