using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Playing;
    public GameContext Context { get; private set; } = GameContext.Title;

    public bool IsPlaying => State == GameState.Playing;
    public bool IsPaused => State == GameState.Paused;
    public bool IsInventoryOpen { get; private set; }
    public bool IsStatOpen { get; private set; }

    public bool CanControlPlayer => IsPlaying;
    public bool CanWorldInteract => IsPlaying;
    public bool CanCombat => IsPlaying && Context == GameContext.Dungeon;

    public event Action<GameState> StateChanged;
    public event Action<GameContext> ContextChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetContext(ResolveContext(scene.name));
        SetPlaying();
        GameSessionReset.ApplyForScene(scene);
        UpdateBgmForContext();

        if (GameSceneNames.IsDungeonScene(scene.name))
            StartCoroutine(RetryDungeonSetupNextFrame());
    }

    private static IEnumerator RetryDungeonSetupNextFrame()
    {
        yield return null;
        DungeonSceneSetupUtility.EnsureGameplay();
    }

    private void UpdateBgmForContext()
    {
        if (SoundManager.Instance == null)
            return;

        switch (Context)
        {
            case GameContext.Hub:
                SoundManager.Instance.PlayBgm("Audio/BGM/hub_loop");
                break;
            case GameContext.Dungeon:
                SoundManager.Instance.PlayBgm("Audio/BGM/dungeon_loop");
                break;
            default:
                SoundManager.Instance.StopBgm();
                break;
        }
    }

    public void SetContext(GameContext newContext)
    {
        Context = newContext;
        ContextChanged?.Invoke(Context);
        UpdateCursorVisibility();
    }

    public void SetPlaying()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        UpdateCursorVisibility();
        StateChanged?.Invoke(State);
    }

    public void SetInventoryOpen(bool open)
    {
        if (IsInventoryOpen == open)
            return;

        IsInventoryOpen = open;
        UpdateCursorVisibility();
    }

    public void SetStatOpen(bool open)
    {
        if (IsStatOpen == open)
            return;

        IsStatOpen = open;
        UpdateCursorVisibility();
    }

    public void NotifySceneTransitionComplete()
    {
        UpdateCursorVisibility();
    }

    private void UpdateCursorVisibility()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetPaused()
    {
        State = GameState.Paused;
        Time.timeScale = 0f;
        UpdateCursorVisibility();
        StateChanged?.Invoke(State);
    }

    public void TogglePause()
    {
        if (IsPaused)
            SetPlaying();
        else
            SetPaused();
    }

    public void PrepareSceneTransition()
    {
        SetPlaying();
    }

    private static GameContext ResolveContext(string sceneName)
    {
        if (GameSceneNames.IsTitleScene(sceneName))
            return GameContext.Title;

        if (GameSceneNames.IsHubScene(sceneName))
            return GameContext.Hub;

        if (GameSceneNames.IsDungeonScene(sceneName))
            return GameContext.Dungeon;

        return GameContext.Title;
    }
}
