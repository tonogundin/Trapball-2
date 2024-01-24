using System.Collections.Generic;
using UnityEngine;

public class CheckPointActive : MonoBehaviour
{

    public List<CheckPoint> checkPoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 getPositionLastCheckPoint()
    {
        int highestTrueIndex = checkPoints.FindLastIndex(cp => cp != null && cp.active);
        return checkPoints[highestTrueIndex].getPosition();
    }

    public void setResetCheckpointsObjects()
    {
        if (checkPoints != null)
        {
            foreach (CheckPoint checkpoint in checkPoints)
            {
                checkpoint.setResetObjects();
            }
        }
    }

    public void setActiveCheckpointsObjects(bool active)
    {
        if (checkPoints != null)
        {
            foreach (CheckPoint checkpoint in checkPoints)
            {
                checkpoint.setActiveObjects(active);
            }
        }
    }
}
