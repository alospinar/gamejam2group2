using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PoliceVision : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField, Min(0.1f)] private float viewDistance = 8f;
    [SerializeField, Range(0f, 360f)] private float viewAngle = 90f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField, Min(0.01f)] private float detectionInterval = 0.1f;

    [Header("Direction")]
    [SerializeField] private int startingDirection = 1;
    [SerializeField, Min(0f)] private float directionChangeInterval = 3f;
    [SerializeField] private Transform visualRoot;

    private Collider2D visionCollider;
    private float nextDetectionTime;
    private float nextDirectionChangeTime;
    private int currentDirection;

    private void Awake()
    {
        visionCollider = GetComponent<Collider2D>();
        visionCollider.isTrigger = true;
        currentDirection = startingDirection < 0 ? -1 : 1;
        nextDirectionChangeTime = Time.time + directionChangeInterval;
        ApplyVisualDirection();
    }

    private void Update()
    {
        DetectPlayersInRange();

        if (directionChangeInterval <= 0f || Time.time < nextDirectionChangeTime)
            return;

        currentDirection *= -1;
        nextDirectionChangeTime = Time.time + directionChangeInterval;
        ApplyVisualDirection();
    }

    private void DetectPlayersInRange()
    {
        if (Time.time < nextDetectionTime || !CanDetectPlayer())
            return;

        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(
            transform.position,
            viewDistance);

        foreach (Collider2D player in playersInRange)
            TryDetect(player);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDetect(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDetect(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDetect(collision.collider);
    }

    private void TryDetect(Collider2D other)
    {
        if (Time.time < nextDetectionTime ||
            (!IsInLayerMask(other.gameObject.layer, playerLayer) &&
             !other.CompareTag("Player")) ||
            !CanDetectPlayer())
            return;

        nextDetectionTime = Time.time + detectionInterval;

        Vector2 directionToPlayer = (Vector2)other.bounds.center - (Vector2)transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer > viewDistance || distanceToPlayer <= Mathf.Epsilon)
            return;

        Vector2 viewDirection = Vector2.right * currentDirection;
        float angle = Vector2.Angle(viewDirection, directionToPlayer / distanceToPlayer);
        if (angle > viewAngle * 0.5f)
            return;

        if (GameManager.Instance != null)
            GameManager.Instance.policeColision = true;

        if (LevelManager.Instance != null)
            LevelManager.Instance.TriggerGameOver(GameOverReason.PoliceDetected);
    }

    private static bool CanDetectPlayer()
    {
        if (LevelManager.Instance != null)
            return LevelManager.Instance.CurrentState == GameState.Playing;

        return GameManager.Instance != null && GameManager.Instance.isGameActive;
    }

    private void ApplyVisualDirection()
    {
        Transform target = visualRoot != null ? visualRoot : transform;
        Vector3 scale = target.localScale;
        scale.x = Mathf.Abs(scale.x) * currentDirection;
        target.localScale = scale;
    }

    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        Vector3 forward = Vector3.right * (startingDirection < 0 ? -1f : 1f);
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
