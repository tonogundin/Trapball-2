using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Transform player;

    private void Awake()
    {
        GameManager.NewPlayer += FollowNewPlayer;

    }

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position;
            float minusX = player.gameObject.GetComponent<Player>().especialStage ? 15 : 0;
            float minusY = player.gameObject.GetComponent<Player>().especialStage ? 1 : 0;
            transform.position = new Vector3(transform.position.x - minusX, transform.position.y + 3 + minusY, GameManager.gM.zCamOffset);
        }
    }
    void FollowNewPlayer()
    {
        player = GameObject.FindGameObjectWithTag(Player.TAG).transform;
    }

    private void OnDisable()
    {
        GameManager.NewPlayer -= FollowNewPlayer;
    }


}
