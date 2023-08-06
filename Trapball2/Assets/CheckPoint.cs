using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CheckPoint : MonoBehaviour
{

    public bool active = false;

    public List<GameObject> objects;

    private List<Vector2> positionObjects = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        if (objects != null && objects.Count > 0)
        {
            positionObjects.Clear();
            positionObjects = new List<Vector2>();
            foreach (GameObject obj in objects)
            {
                positionObjects.Add(obj.GetComponent<Rigidbody>().position);
            }
        }
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
        if (positionObjects != null && positionObjects.Count > 0 && objects != null && objects.Count > 0)
        {
            foreach (var (gameObject, position) in objects.Zip(positionObjects, (o, p) => (o, p)))
            {
                gameObject.GetComponent<Rigidbody>().position = new Vector3(position.x, position.y, gameObject.GetComponent<Rigidbody>().position.z);
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
