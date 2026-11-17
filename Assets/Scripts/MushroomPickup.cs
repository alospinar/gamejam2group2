using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
    [SerializeField] private WorldManager worldManager;
    [SerializeField] private int chargesToRestore = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (worldManager == null)
        {
            Debug.LogError("MushroomPickup requires a WorldManager reference.", this);
            return;
        }

        worldManager.RestoreMushroomCharges(chargesToRestore);
        Destroy(gameObject);
    }
}
