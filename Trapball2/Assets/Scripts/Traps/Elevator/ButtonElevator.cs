using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonElevator : MonoBehaviour
{
    // Start is called before the first frame update
    public Elevator elevator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        switch (tag)
        {
            case Player.TAG:
                elevator.setHydraulic(false);
                break;
            default:
                // Handle other cases or do nothing
                break;
        }

    }
}
