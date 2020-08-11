using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCamera : MonoBehaviour
{
    Transform player;
    Camera cam;
    [SerializeField] Text pruebaText;
    [SerializeField] float xMin, xMax;
    [SerializeField] float yMin, yMax;

    private void OnEnable()
    {
        GameManager.NewPlayer += FollowNewPlayer;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position;
            float xClamped = Mathf.Clamp(transform.position.x, GameManager.gM.xLimits[0], GameManager.gM.xLimits[1]);
            float yClamped = Mathf.Clamp(transform.position.y, GameManager.gM.yLimits[0], GameManager.gM.yLimits[1]);
            //transform.position = new Vector3(xClamped, yClamped, GameManager.gM.zCamOffset);
            transform.position = new Vector3(transform.position.x, transform.position.y + 3, GameManager.gM.zCamOffset);
        }
    }
    void FollowNewPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnDisable()
    {
        GameManager.NewPlayer -= FollowNewPlayer;
    }
    
}
