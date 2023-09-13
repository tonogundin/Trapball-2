using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitJump : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(15,5), ForceMode.Impulse);
        }
    }
}
