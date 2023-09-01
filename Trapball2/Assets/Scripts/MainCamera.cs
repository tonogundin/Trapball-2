using UnityEngine;
using UnityEngine.UI;

public class MainCamera : MonoBehaviour
{
    Transform player;
    [SerializeField] Text pruebaText;
    [SerializeField] float xMin, xMax;
    [SerializeField] float yMin, yMax;

    private void Awake()
    {
        GameManager.NewPlayer += FollowNewPlayer;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position;
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
