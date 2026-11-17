using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public enum WorldType
{
    Real,
    Fantasy
}

public class WorldManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference switchAction;

    [Header("World Objects")]
    [SerializeField] private WorldObject[] worldObjects;

    [Header("Player")]
    [SerializeField] private WorldObject playerWorldObject;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap realWorldTilemap;
    [SerializeField] private Tilemap fantasyWorldTilemap;

    private WorldType currentWorld = WorldType.Real;

    public WorldType CurrentWorld => currentWorld;
    public int MushroomCharges => LevelManager.Instance != null
        ? LevelManager.Instance.Mushrooms
        : 0;

    private void OnEnable()
    {
        if (switchAction != null && switchAction.action != null)
        {
            switchAction.action.Enable();
            switchAction.action.performed += OnSwitchWorld;
        }

        UpdateWorld();
    }

    private void OnDisable()
    {
        if (switchAction != null && switchAction.action != null)
        {
            switchAction.action.performed -= OnSwitchWorld;
            switchAction.action.Disable();
        }
    }

    private void OnSwitchWorld(InputAction.CallbackContext context)
    {
        bool enteringFantasyWorld = currentWorld == WorldType.Real;
        if (enteringFantasyWorld &&
            (LevelManager.Instance == null || !LevelManager.Instance.TryConsumeMushroom()))
            return;

        currentWorld = currentWorld == WorldType.Real
            ? WorldType.Fantasy
            : WorldType.Real;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayPortal();

        UpdateWorld();
    }

    private void UpdateWorld()
    {
        bool isFantasyWorld = currentWorld == WorldType.Fantasy;

        if (worldObjects != null)
        {
            foreach (WorldObject obj in worldObjects)
            {
                if (obj != null)
                    obj.ChangeWorld(isFantasyWorld);
            }
        }

        if (playerWorldObject != null)
            playerWorldObject.ChangeWorld(isFantasyWorld);

        SetTilemapActive(realWorldTilemap, !isFantasyWorld);
        SetTilemapActive(fantasyWorldTilemap, isFantasyWorld);

        if (SoundManager.Instance != null)
        {
            if (isFantasyWorld)
                SoundManager.Instance.SwitchToTrippedWorld();
            else
                SoundManager.Instance.SwitchToSoberWorld();
        }

        int availableMushrooms = LevelManager.Instance != null
            ? LevelManager.Instance.Mushrooms
            : 0;
        Debug.Log($"World: {currentWorld}. Mushroom charges: {availableMushrooms}");
    }

    private static void SetTilemapActive(Tilemap tilemap, bool isActive)
    {
        if (tilemap != null)
            tilemap.gameObject.SetActive(isActive);
    }

    public int GetCharges()
    {
        return MushroomCharges;
    }
}
