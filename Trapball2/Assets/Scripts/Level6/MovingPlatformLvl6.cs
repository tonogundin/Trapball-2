using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformLvl6 : MonoBehaviour
{
    Vector3[] spots = new Vector3[2];
    Vector3 currentDest;
    float speed = 0;
    bool shouldStop;
    // Start is called before the first frame update
    void Start()
    {
        spots[0] = transform.position;
        spots[1] = new Vector3(12.82f, transform.position.y, transform.position.z);
        currentDest = spots[1];
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentDest, speed * Time.deltaTime);
        if(transform.position == spots[0])
        {
            currentDest = spots[1];
            if(shouldStop)
            {
                speed = 0;
                shouldStop = false;
            }
        }
        else if(transform.position == spots[1])
        {
            shouldStop = true;
            currentDest = spots[0];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            speed = 3;
        }
    }
}
