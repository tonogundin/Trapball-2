using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CheckPoint : MonoBehaviour
{

    public bool active = false;

    public List<GameObject> objects;


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
        if (other.gameObject.CompareTag("Player"))
        {
            active = true;
        }

    }

    public Vector2 getPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void setResetObjects()
    {
        if (objects != null && objects.Count > 0)
        {
            foreach (var resettable in objects)
            {
                resettable.GetComponent<IResettable>().resetObject();
            }
        }
    }

    public void setActiveObjects(bool active)
    {
        if (objects != null && objects.Count > 0)
        {
            foreach (GameObject gameObject in objects)
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = !active;
            }
        }
    }
}
