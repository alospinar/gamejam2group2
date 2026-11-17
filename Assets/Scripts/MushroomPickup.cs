using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
    [SerializeField] private int chargesToRestore = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (LevelManager.Instance == null)
        {
            Debug.LogError("MushroomPickup requires a LevelManager in the scene.", this);
            return;
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMushroomPickup();

        LevelManager.Instance.AddMushroom(chargesToRestore);
        Destroy(gameObject);
    }
}
