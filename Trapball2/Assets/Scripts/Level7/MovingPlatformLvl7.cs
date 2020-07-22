using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformLvl7 : MonoBehaviour
{
    Vector3[] spots = new Vector3[2];
    Vector3 currentDest;
    float speed = 0;
    bool shouldStop;

    // Start is called before the first frame update
    void Start()
    {
        spots[0] = transform.position;
        spots[1] = transform.position + new Vector3(0, -2.5f, 0);
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
            ComeBackToInitPos();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            speed = 3;
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("Hikla");
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        ComeBackToInitPos();
    //    }
    //}

    void ComeBackToInitPos()
    {
        speed = 6;
        shouldStop = true;
        currentDest = spots[0];
    }
}
