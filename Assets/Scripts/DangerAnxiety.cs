using System.Collections.Generic;
using UnityEngine;

public class DangerAnxiety : MonoBehaviour
{
    [SerializeField, Min(0f)] private float anxietyPerSecond = 5f;
    [SerializeField] private LayerMask dangerLayer;

    private readonly HashSet<Collider2D> dangerContacts = new HashSet<Collider2D>();
    private float pendingAnxiety;

    private void Update()
    {
        if (dangerContacts.Count == 0 ||
            LevelManager.Instance == null ||
            LevelManager.Instance.CurrentState != GameState.Playing)
            return;

        pendingAnxiety += anxietyPerSecond * Time.deltaTime;
        int anxietyToAdd = Mathf.FloorToInt(pendingAnxiety);
        if (anxietyToAdd <= 0)
            return;

        pendingAnxiety -= anxietyToAdd;
        LevelManager.Instance.AddAnxiety(anxietyToAdd);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RegisterDangerContact(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        RegisterDangerContact(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        dangerContacts.Remove(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RegisterDangerContact(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        RegisterDangerContact(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        dangerContacts.Remove(collision.collider);
    }

    private void RegisterDangerContact(Collider2D other)
    {
        if (IsInLayerMask(other.gameObject.layer, dangerLayer))
            dangerContacts.Add(other);
    }

    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
