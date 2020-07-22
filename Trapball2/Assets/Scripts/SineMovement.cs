using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMovement : MonoBehaviour
{

    [SerializeField] float amplitude;
    [SerializeField] float freq;
    [SerializeField] float phase;
    Vector3 initPos;
    [SerializeField] bool isHorizontal;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isHorizontal)
            transform.position = initPos + new Vector3(amplitude * Mathf.Sin(freq * Time.time + phase), 0, 0);
        else
            transform.position = initPos + new Vector3(0, amplitude * Mathf.Sin(freq * Time.time + phase), 0);
    }
}
