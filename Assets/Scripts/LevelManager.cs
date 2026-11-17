using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
    Victory
}

public enum GameOverReason
{
    AnxietyLimit,
    PoliceDetected
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Initial State")]
    [SerializeField, Min(1)] private int initialAnxiety = 1;
    [SerializeField, Min(0)] private int initialMushrooms;
    [SerializeField, Min(0)] private int requiredTickets = 3;

    [Header("Game Over Scenes")]
    [SerializeField] private string anxietyGameOverScene = "Derrota 1";
    [SerializeField] private string policeGameOverScene = "Derrota 2";

    [Header("Level Scenes")]
    [SerializeField] private string firstLevelScene = "Level 1";
    [SerializeField] private string secondLevelScene = "Level 2";
    [SerializeField] private string thirdLevelScene = "Level 3";
    [SerializeField] private string victoryScene = "Victoria";

    private int anxiety;
    private int mushrooms;
    private int tickets;
    private bool transitionInProgress;

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public int Anxiety => anxiety;
    public int Mushrooms => mushrooms;
    public int Tickets => tickets;
    public int RequiredTickets => requiredTickets;

    public event Action<int> OnAnxietyChanged;
    public event Action<int> OnMushroomsChanged;
    public event Action<int> OnTicketsChanged;
    public event Action<GameState> OnGameStateChanged;
    public event Action<GameOverReason> OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetSession();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ResetSession()
    {
        anxiety = Mathf.Clamp(initialAnxiety, 1, 100);
        mushrooms = Mathf.Max(0, initialMushrooms);
        tickets = 0;
        transitionInProgress = false;
        SetGameState(GameState.Playing);

        OnAnxietyChanged?.Invoke(anxiety);
        OnMushroomsChanged?.Invoke(mushrooms);
        OnTicketsChanged?.Invoke(tickets);
    }

    public void AddAnxiety(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Anxiety cannot be increased with a negative amount.", this);
            return;
        }

        SetAnxiety(anxiety + amount);
    }

    public void ReduceAnxiety(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Anxiety cannot be reduced with a negative amount.", this);
            return;
        }

        SetAnxiety(anxiety - amount);
    }

    public void ResetAnxiety()
    {
        SetAnxiety(initialAnxiety);
    }

    public void AddMushroom(int amount = 1)
    {
        if (amount < 0)
        {
            Debug.LogError("Mushroom count cannot be increased with a negative amount.", this);
            return;
        }

        mushrooms += amount;
        OnMushroomsChanged?.Invoke(mushrooms);
    }

    public bool TryConsumeMushroom()
    {
        if (mushrooms <= 0 || CurrentState != GameState.Playing)
            return false;

        mushrooms--;
        OnMushroomsChanged?.Invoke(mushrooms);
        return true;
    }

    public void AddTicket()
    {
        if (tickets >= requiredTickets)
            return;

        tickets++;
        OnTicketsChanged?.Invoke(tickets);
    }

    public void CompleteCurrentLevel()
    {
        if (CurrentState != GameState.Playing || transitionInProgress)
            return;

        string currentScene = SceneManager.GetActiveScene().name;
        string nextScene;

        if (currentScene == firstLevelScene)
        {
            nextScene = secondLevelScene;
        }
        else if (currentScene == secondLevelScene)
        {
            nextScene = thirdLevelScene;
        }
        else if (currentScene == thirdLevelScene)
        {
            if (tickets < requiredTickets)
            {
                Debug.LogWarning(
                    $"Cannot finish the game with {tickets} of {requiredTickets} tickets.",
                    this);
                return;
            }

            SetGameState(GameState.Victory);
            nextScene = victoryScene;
        }
        else
        {
            Debug.LogWarning($"Scene '{currentScene}' is not configured as a game level.", this);
            return;
        }

        LoadScene(nextScene);
    }

    public void SetGameState(GameState state)
    {
        if (CurrentState == state)
            return;

        CurrentState = state;
        OnGameStateChanged?.Invoke(state);
    }

    public void TriggerGameOver(GameOverReason reason)
    {
        if (CurrentState == GameState.GameOver ||
            CurrentState == GameState.Victory ||
            transitionInProgress)
            return;

        transitionInProgress = true;
        SetGameState(GameState.GameOver);
        OnGameOver?.Invoke(reason);

        string sceneName = reason == GameOverReason.AnxietyLimit
            ? anxietyGameOverScene
            : policeGameOverScene;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"No game over scene configured for {reason}.", this);
            transitionInProgress = false;
            return;
        }

        LoadScene(sceneName);
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Cannot load an empty scene name.", this);
            transitionInProgress = false;
            return;
        }

        transitionInProgress = true;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transitionInProgress = false;
    }

    private void SetAnxiety(int value)
    {
        int clampedValue = Mathf.Clamp(value, 1, 100);
        if (anxiety == clampedValue)
            return;

        anxiety = clampedValue;
        OnAnxietyChanged?.Invoke(anxiety);

        if (anxiety >= 100)
            TriggerGameOver(GameOverReason.AnxietyLimit);
    }
}
