using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Raylib_cs;

public class ButtonAction
{
    public List<KeyboardKey> KeyboardKeys { get; set; } = new();
    public List<ArcademiaKeybind> ArcademiaButtons { get; set; } = new();
    public GamepadButton GamepadButton { get; set; } = new();
}

public class AxisAction
{
    public ButtonAction NegativeAction { get; set; } = new();
    public ButtonAction PositiveAction { get; set; } = new();
    public GamepadAxis ControllerAxis { get; set; } = new();
}

public class VectorAction
{
    public AxisAction AxisX { get; set; } = new();
    public AxisAction AxisY { get; set; } = new();
}

public class ActionMap
{
    public static ActionMap LoadedMap { get; private set; } = new();
    public static string path = "action_map.json";

    public Dictionary<string, ButtonAction> ButtonActions { get; set; } = [];
    public Dictionary<string, AxisAction> AxisActions { get; set; } = [];
    public Dictionary<string, VectorAction> VectorActions { get; set; } = [];

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() } // Automatically handles lists/arrays of enums
    };

    public static void Save()
    {
        string json = JsonSerializer.Serialize(LoadedMap, Options);
        File.WriteAllText(Resources.GetResourcePath(path), json);
    }

    public static void Load()
    {
        if (!File.Exists(Resources.GetResourcePath(path)))
        {
            LoadedMap = new();
            return;
        }

        using StreamReader r = new(Resources.GetResourcePath(path));

        string json = r.ReadToEnd();
        LoadedMap = JsonSerializer.Deserialize<ActionMap>(json, Options);
    }

    public static ButtonAction GetButtonAction(string actionName)
    {
        return LoadedMap.ButtonActions[actionName];
    }

    public static AxisAction GetAxisAction(string actionName)
    {
        return LoadedMap.AxisActions[actionName];
    }

    public static VectorAction GetVectorAction(string actionName)
    {
        return LoadedMap.VectorActions[actionName];
    }

    private static bool CheckActionGlobal(string actionName, bool includeNonPlayers, Func<KeyboardKey, bool> keyCheck, Func<int, GamepadButton, bool> gamepadCheck)
    {
        ButtonAction action = ActionMap.GetButtonAction(actionName);
        if (action == null) return false;

        var activeKeyboardSlots = InputManager.Players.Where(p => p.Input != null && p.IsActive && p.Input.IsKeyboard).Select(p => p.Input.InputIdx).ToHashSet();
        var activeControllerSlots = InputManager.Players.Where(p => p.Input != null && p.IsActive && !p.Input.IsKeyboard).Select(p => p.Input.InputIdx).ToHashSet();

        bool keyboardPress = action.KeyboardKeys
            .Where((key, index) => includeNonPlayers || activeKeyboardSlots.Contains(index))
            .Any(key => keyCheck(key));

        bool controllerPress = activeControllerSlots
            .Any(s => gamepadCheck(s, action.GamepadButton));

#if ARCADEMIA
        return action.ArcademiaButtons.
            Where((key, index) => includeNonPlayers || activeKeyboardSlots.Contains(index))
            .Any(key => keyCheck((KeyboardKey)key));
#endif

        return keyboardPress || controllerPress;
    }

    public static bool IsActionPressedGlobal(string actionName, bool includeNonPlayers)
        => CheckActionGlobal(actionName, includeNonPlayers, k => Raylib.IsKeyPressed(k), (idx, b) => Raylib.IsGamepadButtonPressed(idx, b));

    public static bool IsActionDownGlobal(string actionName, bool includeNonPlayers)
        => CheckActionGlobal(actionName, includeNonPlayers, k => Raylib.IsKeyDown(k), (idx, b) => Raylib.IsGamepadButtonDown(idx, b));

    public static bool IsActionReleasedGlobal(string actionName, bool includeNonPlayers)
        => CheckActionGlobal(actionName, includeNonPlayers, k => Raylib.IsKeyReleased(k), (idx, b) => Raylib.IsGamepadButtonReleased(idx, b));


    private static float GetAxisValueGlobal(AxisAction action)
    {
        if (action == null) return 0f;

        float posValue = 0f;
        float negValue = 0f;
        float axisValue = 0f;

#if ARCADEMIA
        for (int i = 0; i < action.PositiveAction.ArcademiaButtons.Count; i++)
        {
            if (Raylib.IsKeyDown((KeyboardKey)action.PositiveAction.ArcademiaButtons[i])) 
                posValue++;
        }

        for (int i = 0; i < action.NegativeAction.ArcademiaButtons.Count; i++)
        {
            if (Raylib.IsKeyDown((KeyboardKey)action.NegativeAction.ArcademiaButtons[i])) 
                negValue++;
        }
#else
        for (int i = 0; i < action.PositiveAction.KeyboardKeys.Count; i++)
        {
            if (Raylib.IsKeyDown(action.PositiveAction.KeyboardKeys[i]))
                posValue++;
        }

        for (int i = 0; i < action.NegativeAction.KeyboardKeys.Count; i++)
        {
            if (Raylib.IsKeyDown(action.NegativeAction.KeyboardKeys[i]))
                negValue++;
        }

        for (int i = 0; i < InputManager.MAX_CONTROLLER_LISTENING; i++)
        {
            float axis = Raylib.GetGamepadAxisMovement(i, action.ControllerAxis);
            if (float.Abs(axis) > float.Abs(axisValue))
            {
                axisValue = axis;
            }
        }

        if (float.Abs(axisValue) < 0.1f)
        {
            axisValue = 0f;
        }
#endif
        return Math.Clamp(posValue - negValue + axisValue, -1.0f, 1.0f);
    }

    public static float GetActionAxisGlobal(string actionName)
    {
        AxisAction action = ActionMap.GetAxisAction(actionName);
        if (action == null) return 0;

        return GetAxisValueGlobal(action);
    }


    public static Vector2 GetActionVector2Global(string actionName)
    {
        VectorAction action = ActionMap.GetVectorAction(actionName);
        if (action == null) return new(0, 0);

        return new Vector2(GetAxisValueGlobal(action.AxisX), GetAxisValueGlobal(action.AxisY));
    }
}