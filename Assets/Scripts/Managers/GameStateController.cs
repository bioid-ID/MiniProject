using System;
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
    public bool CanControlPlayer => IsPlaying && !IsInventoryOpen;
    public bool CanWorldInteract => IsPlaying && !IsInventoryOpen;
    public bool CanCombat => IsPlaying && !IsInventoryOpen && Context == GameContext.Dungeon;

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
    }

    public void SetContext(GameContext newContext)
    {
        if (Context == newContext)
            return;

        Context = newContext;
        ContextChanged?.Invoke(Context);
    }

    public void SetPlaying()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        SetInventoryOpen(false);
        Cursor.visible = false;
        StateChanged?.Invoke(State);
    }

    public void SetInventoryOpen(bool open)
    {
        if (IsInventoryOpen == open)
            return;

        IsInventoryOpen = open;
        Cursor.visible = open || IsPaused;
    }

    public void SetPaused()
    {
        State = GameState.Paused;
        Time.timeScale = 0f;
        Cursor.visible = true;
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
