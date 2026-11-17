using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Root & Screens")]
    [SerializeField] private GameObject uiCanvasRoot;
    public GameObject titleScreen;
    public GameObject HUDScreen;
    public GameObject winerScreen;
    public GameObject gameOverContainer;
    public GameObject jailScreen;
    public GameObject hospitalScreen;
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public GameObject creditsScreen;

    [Header("Audio")]
    public SoundManager soundManager;

    [Header("HUD Elements")]
    public Image barraAnsiedad;
    public Image barraMalViaje;
    public TMP_Text ticketsText;
    [SerializeField, Min(1f)] private float maxMushroomsForBar = 3f;

    [Header("Legacy State (Synced with LevelManager)")]
    public bool isGameActive;
    public float barraCurrentData;
    public float barraTotalData;
    public bool policeColision;
    public bool isTripped;

    [Header("Scene Names")]
    [SerializeField] private string startSceneName = "Pantalla Inicial";
    [SerializeField] private string firstLevelSceneName = "Level 1";
    [SerializeField] private string[] levelSceneNames = { "Level 1", "Level 2", "Level 3" };
    [SerializeField] private string anxietyDefeatSceneName = "Derrota 1";
    [SerializeField] private string policeDefeatSceneName = "Derrota 2";
    [SerializeField] private string victorySceneName = "Victoria";

    private LevelManager subscribedLevelManager;
    private bool isPaused;
    private bool gameOverTriggered;
    private float lastObservedBarraCurrentData;

    private SoundManager ActiveSoundManager => SoundManager.Instance != null ? SoundManager.Instance : soundManager;

    private void Awake()
    {
        ResolveUIReferences();

        if (Instance != null && Instance != this)
        {
            DestroyOwnedRoots();
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        PersistOwnedRoots();
        EnsureCanvasSortingOrder();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeFromLevelManager();
    }

    private void Start()
    {
        if (Instance != this)
            return;

        if (barraTotalData <= 0f)
            barraTotalData = 100f;
        barraCurrentData = barraTotalData;
        lastObservedBarraCurrentData = barraCurrentData;
        policeColision = false;
        isTripped = false;
        gameOverTriggered = false;

        ApplySceneUIState(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (Instance != this || !isGameActive || gameOverTriggered)
            return;

        if (policeColision)
        {
            ShowInLevelGameOver(GameOverReason.PoliceDetected);
            return;
        }

        if (barraCurrentData <= 0f)
        {
            ShowInLevelGameOver(GameOverReason.AnxietyLimit);
            return;
        }

        if (!Mathf.Approximately(barraCurrentData, lastObservedBarraCurrentData))
        {
            lastObservedBarraCurrentData = barraCurrentData;
            if (barraAnsiedad != null && barraTotalData > 0f)
                barraAnsiedad.fillAmount = Mathf.Clamp01(barraCurrentData / barraTotalData);
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this)
            return;

        ApplySceneUIState(scene.name);
    }

    private void ApplySceneUIState(string sceneName)
    {
        Time.timeScale = 1f;
        isPaused = false;
        ResolveUIReferences();
        BindSettingsSliders();

        if (IsStartScene(sceneName))
        {
            isGameActive = false;
            policeColision = false;
            SetScreenActive(titleScreen, true);
            SetScreenActive(HUDScreen, false);
            SetScreenActive(settingsScreen, false);
            SetScreenActive(creditsScreen, false);
            SetScreenActive(pauseScreen, false);
            SetScreenActive(winerScreen, false);
            SetGameOverScreens(false, false);
            ActiveSoundManager?.PlayMenuMusic();
            return;
        }

        if (IsLevelScene(sceneName))
        {
            isGameActive = true;
            gameOverTriggered = false;
            policeColision = false;
            if (barraTotalData <= 0f)
                barraTotalData = 100f;
            barraCurrentData = barraTotalData;
            lastObservedBarraCurrentData = barraCurrentData;
            SetScreenActive(titleScreen, false);
            SetScreenActive(settingsScreen, false);
            SetScreenActive(creditsScreen, false);
            SetScreenActive(pauseScreen, false);
            SetScreenActive(winerScreen, false);
            SetGameOverScreens(false, false);
            SetScreenActive(HUDScreen, true);

            SubscribeToLevelManager();
            if (LevelManager.Instance != null)
            {
                if (LevelManager.Instance.CurrentState == GameState.GameOver ||
                    LevelManager.Instance.CurrentState == GameState.Victory)
                {
                    LevelManager.Instance.ResetSession();
                }
                else
                {
                    LevelManager.Instance.SetGameState(GameState.Playing);
                }
            }

            RefreshHUD();
            ActiveSoundManager?.PlaySoberMusic();
            return;
        }

        if (MatchesScene(sceneName, anxietyDefeatSceneName))
        {
            isGameActive = false;
            SetScreenActive(titleScreen, false);
            SetScreenActive(HUDScreen, false);
            SetScreenActive(settingsScreen, false);
            SetScreenActive(creditsScreen, false);
            SetScreenActive(pauseScreen, false);
            SetScreenActive(winerScreen, false);
            SetGameOverScreens(showHospital: true, showJail: false);
            return;
        }

        if (MatchesScene(sceneName, policeDefeatSceneName))
        {
            isGameActive = false;
            policeColision = true;
            SetScreenActive(titleScreen, false);
            SetScreenActive(HUDScreen, false);
            SetScreenActive(settingsScreen, false);
            SetScreenActive(creditsScreen, false);
            SetScreenActive(pauseScreen, false);
            SetScreenActive(winerScreen, false);
            SetGameOverScreens(showHospital: false, showJail: true);
            return;
        }

        if (MatchesScene(sceneName, victorySceneName))
        {
            isGameActive = false;
            SetScreenActive(titleScreen, false);
            SetScreenActive(HUDScreen, false);
            SetScreenActive(settingsScreen, false);
            SetScreenActive(creditsScreen, false);
            SetScreenActive(pauseScreen, false);
            SetGameOverScreens(false, false);
            SetScreenActive(winerScreen, true);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        gameOverTriggered = false;
        isGameActive = true;
        policeColision = false;
        if (barraTotalData <= 0f)
            barraTotalData = 100f;
        barraCurrentData = barraTotalData;
        lastObservedBarraCurrentData = barraCurrentData;

        if (LevelManager.Instance != null)
            LevelManager.Instance.ResetSession();

        SetScreenActive(titleScreen, false);
        SetScreenActive(settingsScreen, false);
        SetScreenActive(creditsScreen, false);
        SetScreenActive(pauseScreen, false);
        SetScreenActive(winerScreen, false);
        SetGameOverScreens(false, false);

        ActiveSoundManager?.PlaySoberMusic();
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        gameOverTriggered = false;
        isGameActive = true;
        policeColision = false;
        if (barraTotalData <= 0f)
            barraTotalData = 100f;
        barraCurrentData = barraTotalData;
        lastObservedBarraCurrentData = barraCurrentData;

        if (LevelManager.Instance != null)
            LevelManager.Instance.ResetSession();

        SetGameOverScreens(false, false);
        SetScreenActive(pauseScreen, false);
        SetScreenActive(winerScreen, false);

        ActiveSoundManager?.PlaySoberMusic();

        string activeScene = SceneManager.GetActiveScene().name;
        string sceneToLoad = IsLevelScene(activeScene) ? activeScene : firstLevelSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameActive = false;

        if (LevelManager.Instance != null)
            LevelManager.Instance.ResetSession();

        SceneManager.LoadScene(startSceneName);
    }

    public void TogglePause()
    {
        if (!isGameActive)
            return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        SetScreenActive(pauseScreen, isPaused);

        if (LevelManager.Instance != null)
            LevelManager.Instance.SetGameState(isPaused ? GameState.Paused : GameState.Playing);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        TogglePause();
    }

    public void SwitchWorld()
    {
        SoundManager activeSound = ActiveSoundManager;
        if (activeSound == null)
            return;

        if (isTripped)
            activeSound.SwitchToTrippedWorld();
        else
            activeSound.SwitchToSoberWorld();
    }

    public void SettingScreen()
    {
        SetScreenActive(titleScreen, false);
        SetScreenActive(settingsScreen, true);
        BindSettingsSliders();
    }

    public void ReturnFromSettings()
    {
        SetScreenActive(settingsScreen, false);
        SetScreenActive(titleScreen, !isGameActive);
    }

    public void CreditsScreen()
    {
        SetScreenActive(titleScreen, false);
        SetScreenActive(creditsScreen, true);
    }

    public void ReturnFromCredits()
    {
        SetScreenActive(titleScreen, true);
        SetScreenActive(creditsScreen, false);
    }

    private void SubscribeToLevelManager()
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null || subscribedLevelManager == levelManager)
            return;

        UnsubscribeFromLevelManager();

        subscribedLevelManager = levelManager;
        subscribedLevelManager.OnAnxietyChanged += HandleAnxietyChanged;
        subscribedLevelManager.OnMushroomsChanged += HandleMushroomsChanged;
        subscribedLevelManager.OnTicketsChanged += HandleTicketsChanged;
        subscribedLevelManager.OnGameOver += HandleLevelGameOver;
    }

    private void UnsubscribeFromLevelManager()
    {
        if (subscribedLevelManager == null)
            return;

        subscribedLevelManager.OnAnxietyChanged -= HandleAnxietyChanged;
        subscribedLevelManager.OnMushroomsChanged -= HandleMushroomsChanged;
        subscribedLevelManager.OnTicketsChanged -= HandleTicketsChanged;
        subscribedLevelManager.OnGameOver -= HandleLevelGameOver;
        subscribedLevelManager = null;
    }

    private void HandleLevelGameOver(GameOverReason reason)
    {
        ShowInLevelGameOver(reason);
    }

    private void ShowInLevelGameOver(GameOverReason reason)
    {
        if (gameOverTriggered)
            return;

        gameOverTriggered = true;
        isGameActive = false;
        isPaused = false;
        Time.timeScale = 0f;

        if (PlayerController.Instance != null)
            PlayerController.Instance.SetMovementLocked(true);

        SetScreenActive(HUDScreen, false);
        SetScreenActive(pauseScreen, false);
        SetScreenActive(titleScreen, false);
        SetScreenActive(settingsScreen, false);
        SetScreenActive(creditsScreen, false);
        SetScreenActive(winerScreen, false);

        if (reason == GameOverReason.PoliceDetected)
        {
            policeColision = true;
            SetGameOverScreens(showHospital: false, showJail: true);
        }
        else
        {
            barraCurrentData = 0f;
            lastObservedBarraCurrentData = 0f;
            SetGameOverScreens(showHospital: true, showJail: false);
        }

        if (ActiveSoundManager != null &&
            ActiveSoundManager.susto != null &&
            !ActiveSoundManager.susto.isPlaying)
        {
            ActiveSoundManager.susto.Play();
        }

        if (LevelManager.Instance != null && LevelManager.Instance.CurrentState != GameState.GameOver)
            LevelManager.Instance.SetGameState(GameState.GameOver);
    }

    private void HandleAnxietyChanged(int anxiety)
    {
        if (barraAnsiedad != null)
            barraAnsiedad.fillAmount = Mathf.Clamp01(anxiety / 100f);
    }

    private void HandleMushroomsChanged(int mushrooms)
    {
        if (barraMalViaje != null)
            barraMalViaje.fillAmount = Mathf.Clamp01(mushrooms / Mathf.Max(1f, maxMushroomsForBar));
    }

    private void HandleTicketsChanged(int tickets)
    {
        if (ticketsText != null)
            ticketsText.text = tickets.ToString();
    }

    private void RefreshHUD()
    {
        if (LevelManager.Instance != null)
        {
            HandleAnxietyChanged(LevelManager.Instance.Anxiety);
            HandleMushroomsChanged(LevelManager.Instance.Mushrooms);
            HandleTicketsChanged(LevelManager.Instance.Tickets);
            return;
        }

        if (barraAnsiedad != null && barraTotalData > 0f)
            barraAnsiedad.fillAmount = Mathf.Clamp01(barraCurrentData / barraTotalData);

        if (barraMalViaje != null)
            barraMalViaje.fillAmount = 0f;

        if (ticketsText != null)
            ticketsText.text = "0";
    }

    private void ResolveUIReferences()
    {
        if (uiCanvasRoot == null)
        {
            if (titleScreen != null)
                uiCanvasRoot = titleScreen.transform.root.gameObject;
            else if (HUDScreen != null)
                uiCanvasRoot = HUDScreen.transform.root.gameObject;
        }

        if (gameOverContainer == null)
        {
            Transform parent = hospitalScreen != null
                ? hospitalScreen.transform.parent
                : (jailScreen != null ? jailScreen.transform.parent : null);

            if (parent != null && string.Equals(parent.name, "GameOver", StringComparison.OrdinalIgnoreCase))
                gameOverContainer = parent.gameObject;
        }

        if (ticketsText == null && HUDScreen != null)
        {
            Transform ticketsTransform = HUDScreen.transform.Find("Tickets/Tickets text");
            if (ticketsTransform != null)
                ticketsText = ticketsTransform.GetComponent<TMP_Text>();
        }
    }

    private void EnsureCanvasSortingOrder()
    {
        ConfigureScreenCanvas(uiCanvasRoot, 5);
        ConfigureScreenCanvas(titleScreen, 10);
        ConfigureScreenCanvas(HUDScreen, 10);
        ConfigureScreenCanvas(pauseScreen, 20);
        ConfigureScreenCanvas(settingsScreen, 25);
        ConfigureScreenCanvas(creditsScreen, 25);
        ConfigureScreenCanvas(hospitalScreen, 30);
        ConfigureScreenCanvas(jailScreen, 30);
        ConfigureScreenCanvas(winerScreen, 30);
    }

    private static void ConfigureScreenCanvas(GameObject screen, int minimumSortingOrder)
    {
        if (screen == null)
            return;

        Canvas canvas = screen.GetComponent<Canvas>();
        if (canvas == null)
            return;

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (canvas.isRootCanvas && canvas.sortingOrder < minimumSortingOrder)
            canvas.sortingOrder = minimumSortingOrder;

        CanvasScaler scaler = screen.GetComponent<CanvasScaler>();
        RectTransform rt = screen.GetComponent<RectTransform>();
        if (canvas.isRootCanvas &&
            scaler != null &&
            scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize &&
            rt != null &&
            rt.sizeDelta.x >= 1900f)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }
    }

    private void PersistOwnedRoots()
    {
        foreach (GameObject screen in GetManagedScreens())
        {
            if (screen == null)
                continue;

            GameObject root = screen.transform.root.gameObject;
            if (root != gameObject)
                DontDestroyOnLoad(root);
        }
    }

    private void DestroyOwnedRoots()
    {
        var instanceRoots = new System.Collections.Generic.HashSet<GameObject>();
        if (Instance != null)
        {
            instanceRoots.Add(Instance.gameObject);
            foreach (GameObject screen in Instance.GetManagedScreens())
            {
                if (screen != null)
                    instanceRoots.Add(screen.transform.root.gameObject);
            }
        }

        foreach (GameObject screen in GetManagedScreens())
        {
            if (screen == null)
                continue;

            GameObject root = screen.transform.root.gameObject;
            if (root != gameObject && !instanceRoots.Contains(root))
                Destroy(root);
        }
    }

    private GameObject[] GetManagedScreens()
    {
        return new[]
        {
            uiCanvasRoot,
            titleScreen,
            HUDScreen,
            winerScreen,
            gameOverContainer,
            jailScreen,
            hospitalScreen,
            pauseScreen,
            settingsScreen,
            creditsScreen
        };
    }

    private void BindSettingsSliders()
    {
        SoundManager activeSound = ActiveSoundManager;
        if (activeSound == null || settingsScreen == null)
            return;

        Transform bgmTransform = settingsScreen.transform.Find("Background Music Slider");
        Transform sfxTransform = settingsScreen.transform.Find("Efectos Music Slider");

        Slider bgm = bgmTransform != null ? bgmTransform.GetComponent<Slider>() : null;
        Slider sfx = sfxTransform != null ? sfxTransform.GetComponent<Slider>() : null;

        if (bgm != null || sfx != null)
            activeSound.BindSliders(bgm, sfx);
    }

    private void SetGameOverScreens(bool showHospital, bool showJail)
    {
        bool anyGameOver = showHospital || showJail;
        SetScreenActive(gameOverContainer, anyGameOver);
        SetScreenActive(hospitalScreen, showHospital);
        SetScreenActive(jailScreen, showJail);
    }

    private static void SetScreenActive(GameObject screen, bool active)
    {
        if (screen != null && screen.activeSelf != active)
            screen.SetActive(active);
    }

    private bool IsStartScene(string sceneName)
    {
        return MatchesScene(sceneName, startSceneName);
    }

    private bool IsLevelScene(string sceneName)
    {
        if (levelSceneNames == null)
            return false;

        foreach (string levelName in levelSceneNames)
        {
            if (MatchesScene(sceneName, levelName))
                return true;
        }

        return false;
    }

    private static bool MatchesScene(string a, string b)
    {
        return !string.IsNullOrWhiteSpace(a) &&
               !string.IsNullOrWhiteSpace(b) &&
               string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
