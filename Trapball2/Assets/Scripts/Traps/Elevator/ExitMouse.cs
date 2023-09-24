using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;
        GameObject gameObject = other.gameObject;
        if (tag == "Untagged" && other.transform.parent != null)
        {
            tag = other.transform.parent.gameObject.tag;
            gameObject = other.transform.parent.gameObject;
        }
        switch (tag)
        {
            case MouseBall2.TAG:
                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(15,0, 0), ForceMode.Acceleration);
                break;

            default:
                // Handle other cases or do nothing
                break;
        }
    }
}
