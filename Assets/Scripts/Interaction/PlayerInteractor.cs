using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    [Header("Indicator")]
    [SerializeField] private GameObject interactionIndicator;

    private readonly HashSet<IInteractable> nearbyInteractables = new();
    private InputSystem_Actions inputActions;
    private InputAction interactAction;
    private CircleCollider2D interactionTrigger;
    private IInteractable currentInteractable;
    private IInteractable activeInteractable;
    private bool interactionLocked;

    public bool IsInteracting { get; private set; }
    public event Action InteractPressed;

    private void Awake()
    {
        interactionTrigger = GetComponent<CircleCollider2D>();
        if (interactionTrigger == null)
            interactionTrigger = gameObject.AddComponent<CircleCollider2D>();

        interactionTrigger.isTrigger = true;
        interactionTrigger.radius = interactionRadius;

        inputActions = new InputSystem_Actions();
        interactAction = inputActions.Player.Interact;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        interactAction.started += OnInteractStarted;
        interactAction.canceled += OnInteractCanceled;
    }

    private void OnDisable()
    {
        interactAction.started -= OnInteractStarted;
        interactAction.canceled -= OnInteractCanceled;
        inputActions.Disable();
        StopActiveInteraction();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void Update()
    {
        UpdateCurrentInteractable();
        UpdateInteractionIndicator();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInteractableLayer(other.gameObject.layer))
            return;

        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
            nearbyInteractables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
            nearbyInteractables.Remove(interactable);
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (interactionLocked)
        {
            InteractPressed?.Invoke();
            return;
        }

        if (currentInteractable == null || !currentInteractable.CanInteract(this))
            return;

        activeInteractable = currentInteractable;
        IsInteracting = true;
        activeInteractable.Interact(this);
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        StopActiveInteraction();
    }

    private void StopActiveInteraction()
    {
        if (activeInteractable != null)
            activeInteractable.StopInteracting(this);

        activeInteractable = null;
        IsInteracting = false;
    }

    public void SetInteractionLock(bool locked)
    {
        interactionLocked = locked;

        if (locked)
            StopActiveInteraction();
    }

    private void UpdateCurrentInteractable()
    {
        IInteractable closestInteractable = null;
        float closestDistanceSqr = float.PositiveInfinity;

        nearbyInteractables.RemoveWhere(IsInvalidInteractable);

        foreach (IInteractable interactable in nearbyInteractables)
        {
            Component component = interactable as Component;
            if (component == null)
                continue;

            float distanceSqr = (component.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestInteractable = interactable;
            }
        }

        if (currentInteractable != closestInteractable && activeInteractable != null)
            StopActiveInteraction();

        currentInteractable = closestInteractable;
    }

    private bool IsInvalidInteractable(IInteractable interactable)
    {
        Component component = interactable as Component;
        return component == null || !interactable.CanInteract(this);
    }

    private void UpdateInteractionIndicator()
    {
        bool shouldShow = currentInteractable != null && !IsInteracting && !interactionLocked;

        if (interactionIndicator != null)
            interactionIndicator.SetActive(shouldShow);
    }

    private bool IsInteractableLayer(int layer)
    {
        return (interactableLayers.value & (1 << layer)) != 0;
    }
}
