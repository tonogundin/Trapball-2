using UnityEngine;

public class Death : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Player.TAG))
        {
            FMODUtils.playOneShot(FMODConstants.DAMAGE.IMPACT_SPIKES, transform.position);
            Player player = other.GetComponent<Player>();
            player.die();
        }
    }
}
