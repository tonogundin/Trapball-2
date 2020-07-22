using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSpikes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeRotation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ChangeRotation()
    {
        while (true)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            yield return new WaitForSeconds(1.875f);
            transform.rotation = Quaternion.Euler(180, 0, 0);
            yield return new WaitForSeconds(1.875f);
        }
    }
}
