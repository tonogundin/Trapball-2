using UnityEngine;
using System.Collections.Generic;

public class Elevator : MonoBehaviour
{
    public int countActive = 3;
    public List<GameObject> objects;
    public List<GameObject> objectsInElevator = new List<GameObject>();
    public DinamicWater water;

    public GameObject piston;
    private HydraulicLift hydraulic;

    // Start is called before the first frame update
    void Start()
    {
        if (objects.Count > 0)
        {
            foreach (GameObject gameObject in objects)
            {
                gameObject.SetActive(false);
            }
        }
        hydraulic = piston.GetComponent<HydraulicLift>();

    }

    // Update is called once per frame
    void Update()
    {
        checkWater();
        checkHydraulic();
    }

    private void OnTriggerEnter(Collider other)
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
            case Player.TAG:
                if (!objectsInElevator.Contains(gameObject))
                {
                    Debug.Log("Añadido: " + tag);
                    objectsInElevator.Add(gameObject);
                }

                break;

            default:
                // Handle other cases or do nothing
                break;
        }
    }

    private void checkWater()
    {
        if (hydraulic.isNearDownHydraulic() && water.state != StateWaterDinamic.Upload)
        {
            water.gameObject.SetActive(true);
            water.state = StateWaterDinamic.Upload;
        }
        if (hydraulic.isMinUpHydraulic() && water.state != StateWaterDinamic.Download)
        {
            water.state = StateWaterDinamic.Download;
        }
    }
    private void checkHydraulic()
    {
        if (objectsInElevator.Count >= countActive && hydraulic.isUp)
        {
            setHydraulic(false);
        }
        if ((objectsInElevator.Count == 0 || objectsInElevator[0].tag == Player.TAG) && hydraulic.isDown)
        {
            setHydraulic(true);
        }
    }

    private void activeFirstDesactive()
    {
        GameObject inactiveObject = objects.Find(gameObject => !gameObject.activeSelf && !objectsInElevator.Contains(gameObject) && gameObject.tag != Player.TAG);

        if (inactiveObject != null)
        {
            inactiveObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
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
            case Player.TAG:
                if (objectsInElevator.Contains(gameObject))
                {
                    Debug.Log("Eliminado: " + tag);
                    objectsInElevator.Remove(gameObject);
                }

                break;
            default:
                // Handle other cases or do nothing
                break;
        }

        switch (tag)
        {
            case Player.TAG:
                activeFirstDesactive();
                break;
        }
    }
    public void setHydraulic(bool value)
    {
        hydraulic.isExpanding = value;
        if (value)
        {
            hydraulic.velocityNormal = (objectsInElevator.Count > 0) && objectsInElevator[0].tag == Player.TAG;
        }
    }
}
