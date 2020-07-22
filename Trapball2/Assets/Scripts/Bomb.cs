using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    float speed = 3;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector3.up * speed * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Platform"))
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    
    
}
