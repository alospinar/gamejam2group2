using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableFurniture : MonoBehaviour, IInteractable
{
    [SerializeField] private float pushSpeed = 2f;
    [SerializeField] private Collider2D furnitureCollider;

    private Rigidbody2D furnitureRigidbody;
    private RigidbodyConstraints2D originalConstraints;
    private PlayerInteractor pushingPlayer;
    private PlayerController playerController;
    private Collider2D playerCollider;

    public string InteractionPrompt => "E - Empujar";

    private void Awake()
    {
        furnitureRigidbody = GetComponent<Rigidbody2D>();
        originalConstraints = furnitureRigidbody.constraints;
        if (furnitureCollider == null)
            furnitureCollider = GetComponent<Collider2D>();

        SetHorizontalMovementLocked(true);
    }

    private void FixedUpdate()
    {
        if (pushingPlayer == null || playerController == null)
        {
            playerController?.SetPushingAnimation(false);
            StopHorizontalMovement();
            return;
        }

        if (!IsPlayerTouchingFurniture() || !TryGetPushDirection(out float pushDirection))
        {
            playerController.SetPushingAnimation(false);
            StopHorizontalMovement();
            return;
        }

        playerController.SetPushingAnimation(true);
        furnitureRigidbody.linearVelocity = new Vector2(
            pushDirection * pushSpeed,
            furnitureRigidbody.linearVelocity.y);
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        return interactor != null &&
               interactor.GetComponent<PlayerController>() != null &&
               furnitureRigidbody != null &&
               furnitureCollider != null;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor))
            return;

        pushingPlayer = interactor;
        playerController = interactor.GetComponent<PlayerController>();
        playerCollider = interactor.GetComponent<Collider2D>();
        SetHorizontalMovementLocked(false);
        playerController.SetPushingAnimation(false);
    }

    public void StopInteracting(PlayerInteractor interactor)
    {
        if (pushingPlayer != interactor)
            return;

        playerController?.SetPushingAnimation(false);
        pushingPlayer = null;
        playerController = null;
        playerCollider = null;
        SetHorizontalMovementLocked(true);
        StopHorizontalMovement();
    }

    private bool IsPlayerTouchingFurniture()
    {
        return playerCollider != null &&
               furnitureCollider != null &&
               playerCollider.IsTouching(furnitureCollider);
    }

    private bool TryGetPushDirection(out float pushDirection)
    {
        pushDirection = 0f;
        float inputDirection = Mathf.Sign(playerController.moveInput.x);
        if (Mathf.Abs(playerController.moveInput.x) <= Mathf.Epsilon)
            return false;

        float playerSide = Mathf.Sign(playerController.transform.position.x - transform.position.x);
        if (Mathf.Abs(playerSide) <= Mathf.Epsilon || inputDirection != -playerSide)
            return false;

        pushDirection = inputDirection;
        return true;
    }

    private void StopHorizontalMovement()
    {
        if (furnitureRigidbody == null)
            return;

        furnitureRigidbody.linearVelocity = new Vector2(0f, furnitureRigidbody.linearVelocity.y);
        furnitureRigidbody.angularVelocity = 0f;
    }

    private void SetHorizontalMovementLocked(bool locked)
    {
        if (furnitureRigidbody == null)
            return;

        furnitureRigidbody.constraints = locked
            ? originalConstraints | RigidbodyConstraints2D.FreezePositionX
            : originalConstraints & ~RigidbodyConstraints2D.FreezePositionX;
    }
}
