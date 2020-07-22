using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCube : MonoBehaviour
{
    [SerializeField] GameObject airPrefab;
    GameObject airCopy;
    Vector3 offset = new Vector3(0, 1.15f, 0);
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnAir());
    }

    // Update is called once per frame
    void Update()
    {
        if(airCopy != null)
        {
            airCopy.transform.Translate(Vector3.up * 2.5f * Time.deltaTime);
        }
    }

    IEnumerator SpawnAir()
    {
        while(true)
        {
            airCopy = Instantiate(airPrefab, transform.position + offset, Quaternion.identity);
            yield return new WaitForSeconds(2);
            Destroy(airCopy);
        }
    }
}
