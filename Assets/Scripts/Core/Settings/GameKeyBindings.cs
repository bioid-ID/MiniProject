using UnityEngine;
using UnityEngine.InputSystem;

public enum GameAction
{
    Interact,
    Inventory,
    Stats,
    Potion,
    Pause,
    Dash
}

public static class GameKeyBindings
{
    private const string Prefix = "MiniProject_Key_";

    public static Key Interact { get => Get(GameAction.Interact, Key.Space); set => Set(GameAction.Interact, value); }
    public static Key Inventory { get => Get(GameAction.Inventory, Key.I); set => Set(GameAction.Inventory, value); }
    public static Key Stats { get => Get(GameAction.Stats, Key.C); set => Set(GameAction.Stats, value); }
    public static Key Potion { get => Get(GameAction.Potion, Key.U); set => Set(GameAction.Potion, value); }
    public static Key Pause { get => Get(GameAction.Pause, Key.Escape); set => Set(GameAction.Pause, value); }
    public static Key Dash { get => Get(GameAction.Dash, Key.LeftShift); set => Set(GameAction.Dash, value); }

    public static Key Get(GameAction action, Key defaultKey)
    {
        int stored = PlayerPrefs.GetInt(Prefix + action, (int)defaultKey);
        return (Key)stored;
    }

    public static void Set(GameAction action, Key key)
    {
        PlayerPrefs.SetInt(Prefix + action, (int)key);
        PlayerPrefs.Save();
    }

    public static void ResetDefaults()
    {
        Interact = Key.Space;
        Inventory = Key.I;
        Stats = Key.C;
        Potion = Key.U;
        Pause = Key.Escape;
        Dash = Key.LeftShift;
    }

    public static string GetLabel(GameAction action)
    {
        return action switch
        {
            GameAction.Interact => "Interact",
            GameAction.Inventory => "Inventory",
            GameAction.Stats => "Character Stats",
            GameAction.Potion => "Use Potion",
            GameAction.Pause => "Pause / Menu",
            GameAction.Dash => "Dash",
            _ => action.ToString()
        };
    }

    public static string GetDisplayName(Key key)
    {
        if (key == Key.None)
            return "None";

        string name = key.ToString();
        if (name.StartsWith("Digit"))
            return name.Substring(5);

        return name;
    }
}

public static class GameInput
{
    public static bool WasPressed(GameAction action)
    {
        if (Keyboard.current == null)
            return false;

        Key key = GetKey(action);
        if (key == Key.None)
            return false;

        return Keyboard.current[key].wasPressedThisFrame;
    }

    public static bool IsPressed(GameAction action)
    {
        if (Keyboard.current == null)
            return false;

        Key key = GetKey(action);
        if (key == Key.None)
            return false;

        return Keyboard.current[key].isPressed;
    }

    public static Key GetKey(GameAction action)
    {
        return action switch
        {
            GameAction.Interact => GameKeyBindings.Interact,
            GameAction.Inventory => GameKeyBindings.Inventory,
            GameAction.Stats => GameKeyBindings.Stats,
            GameAction.Potion => GameKeyBindings.Potion,
            GameAction.Pause => GameKeyBindings.Pause,
            GameAction.Dash => GameKeyBindings.Dash,
            _ => Key.None
        };
    }
}
