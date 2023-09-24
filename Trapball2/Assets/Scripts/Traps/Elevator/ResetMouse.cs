using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetMouse : MonoBehaviour
{
    // Start is called before the first frame update
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
        GameObject gameObject = collision.gameObject;
        if (tag == "Untagged" && collision.transform.parent != null)
        {
            tag = collision.transform.parent.gameObject.tag;
            gameObject = collision.transform.parent.gameObject;
        }
        switch (tag)
        {
            case MouseBall2.TAG:
                gameObject.GetComponent<MouseBall2>().resetObject();
                StartCoroutine(setMouseFalse(gameObject));
                break;

            default:
                // Handle other cases or do nothing
                break;
        }

    }
    IEnumerator setMouseFalse(GameObject gameObject)
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
