using UnityEngine;

public class TickectPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayTicketPickup();

            Destroy(gameObject);
        }
    }
}
