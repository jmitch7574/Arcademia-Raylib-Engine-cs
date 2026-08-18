using System.Data;
using System.Numerics;
using ArcademiaEngine.Core;
using Raylib_cs;

public class PlayerInput
{
    // Input Info
    public bool IsKeyboard;                         // Whether this device is keyboard or controller
    public int InputIdx;                            // Device specific slot

    // States
    public bool IsConnected;                        // Is this device connected

    // Lifetimes
    public float Lifetime = 0;                      // Since this device was connected
    public float TimeSinceIdentifyingInput = 0;     // Since this device made any input

    public PlayerInput()
    {
        IsKeyboard = false;
        InputIdx = -1;

        IsConnected = false;
    }

    public PlayerInput(bool isKeyboard, int inputIdx)
    {
        IsKeyboard = isKeyboard;
        InputIdx = inputIdx;

        IsConnected = true;
    }

    public PlayerInput(bool isKeyboard, int inputIdx, float lifetime)
    {
        IsKeyboard = isKeyboard;
        InputIdx = inputIdx;

        IsConnected = true;
        Lifetime = lifetime;
    }

    private bool CheckAction(string actionName, Func<KeyboardKey, bool> keyCheck, Func<int, GamepadButton, bool> gamepadCheck)
    {
        ButtonAction action = ActionMap.GetButtonAction(actionName);
        if (action == null) return false;

        if (Launcher.IsArcademia())
            return keyCheck((KeyboardKey)action.ArcademiaButtons[InputIdx]);

        if (IsKeyboard) return keyCheck(action.KeyboardKeys[InputIdx]);

        return gamepadCheck(InputIdx, action.GamepadButton);
    }

    public bool IsActionPressed(string actionName)
        => CheckAction(actionName, k => Raylib.IsKeyPressed(k), (idx, b) => Raylib.IsGamepadButtonPressed(idx, b));

    public bool IsActionDown(string actionName)
        => CheckAction(actionName, k => Raylib.IsKeyDown(k), (idx, b) => Raylib.IsGamepadButtonDown(idx, b));

    public bool IsActionReleased(string actionName)
        => CheckAction(actionName, k => Raylib.IsKeyReleased(k), (idx, b) => Raylib.IsGamepadButtonReleased(idx, b));

    private float GetAxisValue(AxisAction action)
    {
        if (Launcher.IsArcademia())
            return Raylib.IsKeyDown((KeyboardKey)action.PositiveAction.ArcademiaButtons[InputIdx]) -
                   Raylib.IsKeyDown((KeyboardKey)action.NegativeAction.ArcademiaButtons[InputIdx]);

        if (IsKeyboard) return Raylib.IsKeyDown(action.PositiveAction.KeyboardKeys[InputIdx]) -
                               Raylib.IsKeyDown(action.NegativeAction.KeyboardKeys[InputIdx]);


        float buttonValue = Raylib.IsGamepadButtonDown(InputIdx, action.PositiveAction.GamepadButton) -
                            Raylib.IsGamepadButtonDown(InputIdx, action.NegativeAction.GamepadButton);
        float axisValue = Raylib.GetGamepadAxisMovement(InputIdx, action.ControllerAxis);

        if (Math.Abs(buttonValue + axisValue) < 0.1) return 0;

        return Math.Clamp(buttonValue + axisValue, -1.0f, 1.0f);
    }

    public float GetActionAxis(string actionName)
    {
        AxisAction action = ActionMap.GetAxisAction(actionName);
        if (action == null) return 0;

        return GetAxisValue(action);
    }

    public Vector2 GetActionVector2(string actionName)
    {
        VectorAction action = ActionMap.GetVectorAction(actionName);
        if (action == null) return new(0, 0);

        return new Vector2(GetAxisValue(action.AxisX), GetAxisValue(action.AxisY));
    }
}