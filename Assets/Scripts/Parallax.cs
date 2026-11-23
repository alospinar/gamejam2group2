using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Parallax Settings")]
    [SerializeField, Range(0f, 1f)]
    private float parallaxEffect = 0.1f;

    [SerializeField] private bool horizontalParallax = true;
    [SerializeField] private bool verticalParallax = false;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogError("No se encontró Main Camera");
            enabled = false;
            return;
        }

        startPosition = transform.position;
        cameraStartPosition = targetCamera.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraMovement =
            targetCamera.transform.position - cameraStartPosition;

        Vector3 newPosition = startPosition;

        if (horizontalParallax)
            newPosition.x += cameraMovement.x * parallaxEffect;

        if (verticalParallax)
            newPosition.y += cameraMovement.y * parallaxEffect;

        transform.position = newPosition;
    }
}
