using UnityEngine;

public class TickectPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (LevelManager.Instance == null)
            {
                Debug.LogError("TickectPickup requires a LevelManager in the scene.", this);
                return;
            }

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayTicketPickup();

            LevelManager.Instance.AddTicket();
            Destroy(gameObject);
        }
    }
}
