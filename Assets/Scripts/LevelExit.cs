using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    private Collider2D exitCollider;

    private void Awake()
    {
        exitCollider = GetComponent<Collider2D>();
        exitCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelExit requires a LevelManager in the scene.", this);
            return;
        }

        LevelManager.Instance.CompleteCurrentLevel();
    }
}
