using System.Numerics;
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

    public bool IsActionPressed(string actionName)
    {
        ButtonAction action = ActionMap.GetButtonAction(actionName);
#if ARCADEMIA
        return Raylib.IsKeyPressed((KeyboardKey)action.ArcademiaButtons[InputIdx]);
#endif

        if (IsKeyboard) return Raylib.IsKeyPressed(action.KeyboardKeys[InputIdx]);

        return Raylib.IsGamepadButtonPressed(InputIdx, action.GamepadButton);
    }

    public bool IsActionDown(string actionName)
    {
        ButtonAction action = ActionMap.GetButtonAction(actionName);
#if ARCADEMIA
        return Raylib.IsKeyDown((KeyboardKey)action.ArcademiaButtons[InputIdx]);
#endif

        if (IsKeyboard) return Raylib.IsKeyDown(action.KeyboardKeys[InputIdx]);

        return Raylib.IsGamepadButtonDown(InputIdx, action.GamepadButton);
    }

    public bool IsActionReleased(string actionName)
    {
        ButtonAction action = ActionMap.GetButtonAction(actionName);
#if ARCADEMIA
        return Raylib.IsKeyReleased((KeyboardKey)action.ArcademiaButtons[InputIdx]);
#endif

        if (IsKeyboard) return Raylib.IsKeyReleased(action.KeyboardKeys[InputIdx]);

        return Raylib.IsGamepadButtonReleased(InputIdx, action.GamepadButton);
    }

    private float GetAxisValue(AxisAction action)
    {
#if ARCADEMIA
        return Raylib.IsKeyDown((KeyboardKey)action.PositiveAction.ArcademiaButtons[InputIdx]) -
               Raylib.IsKeyDown((KeyboardKey)action.NegativeAction.ArcademiaButtons[InputIdx]);
#endif

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

        return GetAxisValue(action);
    }

    public Vector2 GetActionVector2(string actionName)
    {
        VectorAction action = ActionMap.GetVectorAction(actionName);
        return new Vector2(GetAxisValue(action.AxisX), GetAxisValue(action.AxisY));
    }
}