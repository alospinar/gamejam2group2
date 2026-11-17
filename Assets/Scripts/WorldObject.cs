using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D objectCollider;
    [SerializeField] private Rigidbody2D _rigidbody2D;

    [Header("World Sprites")]
    [SerializeField] private Sprite realSprite;
    [SerializeField] private Sprite fantasySprite;

    [Header("World Scales")]
    [SerializeField] private Vector3 realScale = Vector3.one;
    [SerializeField] private Vector3 fantasyScale = Vector3.one;

    [Header("Collider Settings")]
    [SerializeField] private bool colliderInRealWorld = true;
    [SerializeField] private bool colliderInFantasyWorld = false;

    [Header("Rigidbody Settings")]
    [SerializeField] private RigidbodyType2D realBodyType = RigidbodyType2D.Static;
    [SerializeField] private RigidbodyType2D fantasyBodyType = RigidbodyType2D.Dynamic;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (_rigidbody2D == null)
            _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void ChangeWorld(bool fantasyWorld)
    {
        transform.localScale = fantasyWorld ? fantasyScale : realScale;

        if (spriteRenderer != null)
        {
            Sprite currentSprite = fantasyWorld ? fantasySprite : realSprite;
            spriteRenderer.sprite = currentSprite;
            spriteRenderer.enabled = currentSprite != null;
        }

        if (objectCollider != null)
            objectCollider.enabled = fantasyWorld
                ? colliderInFantasyWorld
                : colliderInRealWorld;

        if (_rigidbody2D != null)
        {
            _rigidbody2D.bodyType = fantasyWorld ? fantasyBodyType : realBodyType;
        }
    }
}
