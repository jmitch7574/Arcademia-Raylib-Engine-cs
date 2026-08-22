using System.ComponentModel;
using System.Data.Common;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions;
using ArcademiaEngine.Core;
using ArcademiaEngine.Core.Engine.Text;
using Raylib_cs;


/// <summary>
/// Class for drawing Input Icons
/// </summary>
public static class InputGraphics
{
    static Texture2D KeyTex;
    static NPatchInfo KeyInfo;
    static Shader KeyShader;
    static TextSettings textSettings = new()
    {
        FontSize = 0,
        Color = Color.White,
        HorizontalAlignment = Alignment.Middle,
        VerticalAlignment = Alignment.Middle
    };

    public struct GlobalOptions
    {
        public int Height;
        public bool Reactive;
        public int? ControllerId;
        public Color OutlineColor;      // Outlines
        public Color FillColor;         // Fills
        public Color IconColor;         // Text, Icons
        public Color SpecialColor;      // Active Presses for Face buttons and Dpad
    }

    public struct KeyboardKeyOptions
    {
        public GlobalOptions Base;
    };

    public struct GamepadFaceOptions
    {
        public GlobalOptions Base;
        public AxisDirections Directions;
    };

    public struct GamepadStickOptions
    {
        public GlobalOptions Base;
        public AxisDirections Directions;
        public GamepadAxis? HorizontalAxis;
        public GamepadAxis? VerticalAxis;
        public GamepadButton? StickPressButton;
        public char StickLabel;
        public bool StickPress;
    };

    public struct GamepadShoulderOptions
    {
        public GlobalOptions Base;
        public GamepadButton Button;
    };

    public struct GamepadTriggerOptions
    {
        public GlobalOptions Base;
        public GamepadButton? Button;
        public GamepadAxis? Axis;
    };

    public struct GamepadMenuOptions
    {
        public GlobalOptions Base;
        public GamepadButton Button;
    }

    public struct ArcademiaStickOptions
    {
        public GlobalOptions Base;
        public AxisDirections Directions;
    }

    public struct ArcademiaButtonOptions
    {
        public GlobalOptions Base;
        public ArcademiaKeybind Button;
    }

    [Flags]
    public enum AxisDirections
    {
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
    }

    public static AxisDirections GamepadToAxisDirections(GamepadButton btn) => btn switch
    {
        GamepadButton.LeftFaceUp or GamepadButton.RightFaceUp => AxisDirections.UP,
        GamepadButton.LeftFaceDown or GamepadButton.RightFaceDown => AxisDirections.DOWN,
        GamepadButton.LeftFaceLeft or GamepadButton.RightFaceLeft => AxisDirections.LEFT,
        GamepadButton.LeftFaceRight or GamepadButton.RightFaceRight => AxisDirections.RIGHT,
        _ => throw new NotImplementedException()
    };

    public static void Init()
    {
        KeyTex = Raylib.LoadTexture("Resources/engine/control-icons/key_background.png");

        // KeyInfo (16x16 texture)
        KeyInfo = new NPatchInfo
        {
            Source = new Rectangle(0, 0, 16, 16),
            Left = 3,
            Right = 3,
            Top = 3,
            Bottom = 5,
            Layout = NPatchLayout.NinePatch
        };

        if (Launcher.config.IsWeb) KeyShader = Raylib.LoadShader(null, "Resources/engine/shaders/KeyShaderWeb.fs");
        else KeyShader = Raylib.LoadShader(null, "Resources/engine/shaders/KeyShaderDesktop.fs");
    }

    public static int CalculateKeyWidth(KeyboardKey key, KeyboardKeyOptions options)
    {
        string text = GetKeyDisplayName(key);
        int fontSize = options.Base.Height;

        Vector2 size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, MathF.Floor(fontSize / 10));


        int width = (int)size.X + fontSize;
        int boxHeight = (int)size.Y + fontSize;

        width = Math.Max(width, boxHeight);

        return width;
    }

    public static void DrawKeyboardKey(Vector2 center, KeyboardKey key, out int width, KeyboardKeyOptions options)
    {
        if (options.Base.Reactive && Raylib.IsKeyDown(key)) options.Base.OutlineColor = Raylib.ColorBrightness(options.Base.OutlineColor, 0.25f);

        // Update Shader Info
        int replaceLoc0 = Raylib.GetShaderLocation(KeyShader, "replaceColor0");
        int replaceLoc1 = Raylib.GetShaderLocation(KeyShader, "replaceColor1");
        int replaceLoc2 = Raylib.GetShaderLocation(KeyShader, "replaceColor2");
        int tolLoc = Raylib.GetShaderLocation(KeyShader, "tolerance");

        Vector4 repColor0 = Raylib.ColorNormalize(Raylib.ColorBrightness(options.Base.FillColor, -0.5f));
        Vector4 repColor1 = Raylib.ColorNormalize(Raylib.ColorBrightness(options.Base.FillColor, -0.2f));
        Vector4 repColor2 = Raylib.ColorNormalize(options.Base.FillColor);

        Raylib.SetShaderValue(KeyShader, replaceLoc0, repColor0, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(KeyShader, replaceLoc1, repColor1, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(KeyShader, replaceLoc2, repColor2, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(KeyShader, tolLoc, 0.1f, ShaderUniformDataType.Float);

        string text = GetKeyDisplayName(key);
        int fontSize = options.Base.Height;

        Vector2 size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, MathF.Floor(fontSize / 10));

        width = (int)size.X + fontSize;
        int boxHeight = (int)size.Y + fontSize;

        width = Math.Max(width, boxHeight);

        int topLeftX = (int)(center.X - width / 2);
        int topLeftY = (int)(center.Y - fontSize / 2);

        int additionalHeight = boxHeight - fontSize;


        Raylib.BeginShaderMode(KeyShader);
        Raylib.DrawTextureNPatch(KeyTex, KeyInfo, new Rectangle(topLeftX, topLeftY - additionalHeight / 2, width, boxHeight), new Vector2(0, 0), 0, Color.White);
        Raylib.EndShaderMode();

        Text.DrawText(text, new Vector2(center.X, center.Y), textSettings with { FontSize = fontSize, Color = options.Base.IconColor });
    }

    public static string GetKeyDisplayName(KeyboardKey key) => key switch
    {
        KeyboardKey.Null => "",
        KeyboardKey.Space => "Space",
        KeyboardKey.KpEnter => "Enter",
        KeyboardKey.Backspace => "Back Space",
        KeyboardKey.Escape => "Esc",
        KeyboardKey.CapsLock => "Caps",
        KeyboardKey.PrintScreen => "PrtScn",
        KeyboardKey.ScrollLock => "ScrLck",
        KeyboardKey.NumLock => "NumLck",
        KeyboardKey.PageUp => "Page Up",
        KeyboardKey.PageDown => "Page Down",
        KeyboardKey.KeyboardMenu => "Menu",

        // Modifiers & Navigation
        KeyboardKey.LeftShift => "L Shift",
        KeyboardKey.RightShift => "R Shift",
        KeyboardKey.LeftControl => "L Ctrl",
        KeyboardKey.RightControl => "R Ctrl",
        KeyboardKey.LeftAlt => "L Alt",
        KeyboardKey.RightAlt => "R Alt",
        KeyboardKey.LeftSuper => "L Super",
        KeyboardKey.RightSuper => "R Super",
        KeyboardKey.Up => "Up Arrow",
        KeyboardKey.Down => "Down Arrow",
        KeyboardKey.Left => "Left Arrow",
        KeyboardKey.Right => "Right Arrow",

        // Punctuation & Symbols
        KeyboardKey.Apostrophe => "'",
        KeyboardKey.Comma => ",",
        KeyboardKey.Minus => "-",
        KeyboardKey.Period => ".",
        KeyboardKey.Slash => "/",
        KeyboardKey.Semicolon => ";",
        KeyboardKey.Equal => "=",
        KeyboardKey.LeftBracket => "[",
        KeyboardKey.Backslash => "\\",
        KeyboardKey.RightBracket => "]",
        KeyboardKey.Grave => "`",

        // Numpad Keys
        KeyboardKey.Kp0 => "Npd 0",
        KeyboardKey.Kp1 => "Npd 1",
        KeyboardKey.Kp2 => "Npd 2",
        KeyboardKey.Kp3 => "Npd 3",
        KeyboardKey.Kp4 => "Npd 4",
        KeyboardKey.Kp5 => "Npd 5",
        KeyboardKey.Kp6 => "Npd 6",
        KeyboardKey.Kp7 => "Npd 7",
        KeyboardKey.Kp8 => "Npd 8",
        KeyboardKey.Kp9 => "Npd 9",
        KeyboardKey.KpDecimal => "Npd .",
        KeyboardKey.KpDivide => "Npd /",
        KeyboardKey.KpMultiply => "Npd *",
        KeyboardKey.KpSubtract => "Npd -",
        KeyboardKey.KpAdd => "Npd +",
        KeyboardKey.KpEqual => "Npd =",

        // Digits (Zero -> "0", One -> "1", etc.)
        KeyboardKey.Zero => "0",
        KeyboardKey.One => "1",
        KeyboardKey.Two => "2",
        KeyboardKey.Three => "3",
        KeyboardKey.Four => "4",
        KeyboardKey.Five => "5",
        KeyboardKey.Six => "6",
        KeyboardKey.Seven => "7",
        KeyboardKey.Eight => "8",
        KeyboardKey.Nine => "9",

        // Fallback for A-Z, F1-F12, and Android keys (e.g., KeyboardKey.A -> "A")
        _ => key.ToString()
    };

    private static bool GetButtonDown(GamepadButton button, int? controllerId)
    {
        if (controllerId != null) return Raylib.IsGamepadButtonDown((int)controllerId, button);
        else return InputManager.GetGlobalGamepadButtonDown(button);
    }

    private static float GetAxis(GamepadAxis axis, int? controllerId)
    {
        if (controllerId != null) return Raylib.GetGamepadAxisMovement((int)controllerId, axis);
        else return InputManager.GetGlobalAxis(axis);
    }


    public static void DrawFaceButtons(Vector2 center, GamepadFaceOptions options)
    {
        int radius = (int)(options.Base.Height / 3);

        int cX = (int)center.X;
        int cY = (int)center.Y;

        var directions = new (AxisDirections Flag, GamepadButton button, int OffsetX, int OffsetY)[]
        {
            (AxisDirections.UP, GamepadButton.RightFaceUp, 0, -2 * radius),
            (AxisDirections.DOWN, GamepadButton.RightFaceDown, 0,  2 * radius),
            (AxisDirections.LEFT, GamepadButton.RightFaceLeft, -2 * radius, 0),
            (AxisDirections.RIGHT, GamepadButton.RightFaceRight, 2 * radius, 0)
        };

        foreach (var (flag, button, offsetX, offsetY) in directions)
        {
            var color = options.Directions.HasFlag(flag) ? options.Base.IconColor : options.Base.FillColor;

            if (options.Base.Reactive && GetButtonDown(button, options.Base.ControllerId)) color = options.Base.SpecialColor;

            Raylib.DrawCircle(cX + offsetX, cY + offsetY, radius, color);
        }

        // Outlines
        Raylib.DrawCircleLines(cX, cY - (2 * radius), radius, options.Base.OutlineColor); // North
        Raylib.DrawCircleLines(cX, cY + (2 * radius), radius, options.Base.OutlineColor); // South
        Raylib.DrawCircleLines(cX - (2 * radius), cY, radius, options.Base.OutlineColor); // West
        Raylib.DrawCircleLines(cX + (2 * radius), cY, radius, options.Base.OutlineColor); // East
    }

    public static void DrawDirectionalPad(Vector2 center, GamepadFaceOptions options)
    {
        float thickness = 2;

        int cX = (int)center.X;
        int cY = (int)center.Y;

        int borderDist = options.Base.Height / 3; // Distance from center to inner edge
        int edgeDist = options.Base.Height;       // Distance from center to outer edge
        int halfDist = options.Base.Height / 2;       // For triangles

        // Backgrounds
        Raylib.DrawRectangle(cX - edgeDist, cY - borderDist, edgeDist * 2, borderDist * 2, options.Base.FillColor);
        Raylib.DrawRectangle(cX - borderDist, cY - edgeDist, borderDist * 2, edgeDist * 2, options.Base.FillColor);

        var triangles = new (AxisDirections Flag, GamepadButton button, Vector2 V1, Vector2 V2, Vector2 V3)[]
        {
            (
                AxisDirections.UP,
                GamepadButton.LeftFaceUp,
                new Vector2(cX - borderDist, cY - halfDist),
                new Vector2(cX + borderDist, cY - halfDist),
                new Vector2(cX, cY - edgeDist)
            ),
            (
                AxisDirections.DOWN,
                GamepadButton.LeftFaceDown,
                new Vector2(cX, cY + edgeDist),
                new Vector2(cX + borderDist, cY + halfDist),
                new Vector2(cX - borderDist, cY + halfDist)
            ),
            (
                AxisDirections.LEFT,
                GamepadButton.LeftFaceLeft,
                new Vector2(cX - edgeDist, cY),
                new Vector2(cX - halfDist, cY + borderDist),
                new Vector2(cX - halfDist, cY - borderDist)
            ),
            (
                AxisDirections.RIGHT,
                GamepadButton.LeftFaceRight,
                new Vector2(cX + halfDist, cY - borderDist),
                new Vector2(cX + halfDist, cY + borderDist),
                new Vector2(cX + edgeDist, cY)
            )
        };

        foreach (var (flag, button, v1, v2, v3) in triangles)
        {
            if (options.Directions.HasFlag(flag))
                Raylib.DrawTriangle(v1, v2, v3, options.Base.IconColor);

            if (options.Base.Reactive && GetButtonDown(button, options.Base.ControllerId))
                Raylib.DrawTriangle(v1, v2, v3, options.Base.SpecialColor);
        }


        // Outlines

        // Top Direction
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY - borderDist), new Vector2(cX - borderDist, cY - edgeDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY - edgeDist), new Vector2(cX + borderDist, cY - edgeDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + borderDist, cY - edgeDist), new Vector2(cX + borderDist, cY - borderDist), thickness, options.Base.OutlineColor);

        // Right Direction
        Raylib.DrawLineEx(new Vector2(cX + borderDist, cY - borderDist), new Vector2(cX + edgeDist, cY - borderDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + edgeDist, cY - borderDist), new Vector2(cX + edgeDist, cY + borderDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + edgeDist, cY + borderDist), new Vector2(cX + borderDist, cY + borderDist), thickness, options.Base.OutlineColor);


        // Bottom Direction
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY + borderDist), new Vector2(cX - borderDist, cY + edgeDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY + edgeDist), new Vector2(cX + borderDist, cY + edgeDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + borderDist, cY + edgeDist), new Vector2(cX + borderDist, cY + borderDist), thickness, options.Base.OutlineColor);


        // Left Direction
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY - borderDist), new Vector2(cX - edgeDist, cY - borderDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - edgeDist, cY - borderDist), new Vector2(cX - edgeDist, cY + borderDist), thickness, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - edgeDist, cY + borderDist), new Vector2(cX - borderDist, cY + borderDist), thickness, options.Base.OutlineColor);
    }

    public static void DrawStick(Vector2 center, GamepadStickOptions options)
    {
        int cX = (int)center.X;
        int cY = (int)center.Y;

        int mX = 0;
        int mY = 0;

        if (!options.StickPress)
        {
            if (options.HorizontalAxis != null)
            {
                mX = (int)(GetAxis((GamepadAxis)options.HorizontalAxis, options.Base.ControllerId) * options.Base.Height / 2);
            }

            if (options.VerticalAxis != null)
            {
                mY = (int)(GetAxis((GamepadAxis)options.VerticalAxis, options.Base.ControllerId) * options.Base.Height / 2);
            }
        }

        int stickOutlineEnd = (int)(options.Base.Height / 1.25f); // Distance from center to inner edge
        int stickOutlineStart = (int)(options.Base.Height / 1.5f); // Distance from center to inner edge

        int triangleStart = stickOutlineEnd + 2; // Distance from center to inner edge
        int triangleHalfWidth = (int)(options.Base.Height / 3f);       // Distance from center to outer edge
        int edgeDist = (int)(options.Base.Height * 1.25f);       // Distance from center to outer edge

        int fontSize = ((int)(options.Base.Height / 10) * 10);

        if (!options.StickPress)
        {
            if (options.Directions.HasFlag(AxisDirections.UP))
                Raylib.DrawTriangle(new Vector2(cX - triangleHalfWidth, cY - triangleStart), new Vector2(cX + triangleHalfWidth, cY - triangleStart), new Vector2(cX, cY - edgeDist), options.Base.IconColor);

            if (options.Directions.HasFlag(AxisDirections.DOWN))
                Raylib.DrawTriangle(new Vector2(cX, cY + edgeDist), new Vector2(cX + triangleHalfWidth, cY + triangleStart), new Vector2(cX - triangleHalfWidth, cY + triangleStart), options.Base.IconColor);

            if (options.Directions.HasFlag(AxisDirections.LEFT))
                Raylib.DrawTriangle(new Vector2(cX - edgeDist, cY), new Vector2(cX - triangleStart, cY + triangleHalfWidth), new Vector2(cX - triangleStart, cY - triangleHalfWidth), options.Base.IconColor);

            if (options.Directions.HasFlag(AxisDirections.RIGHT))
                Raylib.DrawTriangle(new Vector2(cX + triangleStart, cY - triangleHalfWidth), new Vector2(cX + triangleStart, cY + triangleHalfWidth), new Vector2(cX + edgeDist, cY), options.Base.IconColor);
        }

        // Stick BG
        if (options.Base.Reactive && GetButtonDown((GamepadButton)options.StickPressButton, options.Base.ControllerId))
            Raylib.DrawCircle(cX + mX, cY + mY, stickOutlineStart, Raylib.ColorBrightness(options.Base.FillColor, 0.25f));
        else
            Raylib.DrawCircle(cX + mX, cY + mY, stickOutlineStart, options.Base.FillColor);

        // Stick Outline
        Raylib.DrawRing(new Vector2(cX + mX, cY + mY), stickOutlineStart, stickOutlineEnd, 0, 360, 8, options.Base.OutlineColor);

        // Stick Label
        Text.DrawText($"{options.StickLabel}", new Vector2(cX + mX, cY + mY), textSettings with { FontSize = fontSize, Color = options.Base.IconColor });

        if (options.StickPress)
        {
            Raylib.DrawTriangle(new Vector2(cX, cY - options.Base.Height / 3), new Vector2(cX + stickOutlineEnd / 2, cY - stickOutlineEnd), new Vector2(cX - stickOutlineEnd / 2, cY - stickOutlineEnd), options.Base.IconColor);
        }

    }

    public static void DrawShoulderButton(Vector2 center, GamepadShoulderOptions options)
    {
        int cX = (int)center.X;
        int cY = (int)center.Y;

        int fontSize = ((int)(options.Base.Height / 10) * 10);

        int height = (int)(options.Base.Height);
        int width = (int)(height * 2f);

        int topLeftX = cX - width / 2;
        int topLeftY = cY - height / 2;

        string shoulderButtonText = options.Button switch
        {
            GamepadButton.LeftTrigger1 => "L1",
            GamepadButton.RightTrigger1 => "R1",
            _ => "??",
        };

        if (options.Base.Reactive && GetButtonDown((GamepadButton)options.Button, options.Base.ControllerId))
            Raylib.DrawRectangle(topLeftX, topLeftY, width, height, Raylib.ColorBrightness(options.Base.FillColor, 0.25f));
        else
            Raylib.DrawRectangle(topLeftX, topLeftY, width, height, options.Base.FillColor);

        Raylib.DrawRectangleLines(topLeftX, topLeftY, width, height, options.Base.OutlineColor);

        Text.DrawText(shoulderButtonText, new Vector2(cX, cY), textSettings with { FontSize = fontSize, Color = options.Base.IconColor });
    }


    public static void DrawTriggerButton(Vector2 center, GamepadTriggerOptions options)
    {
        int cX = (int)center.X;
        int cY = (int)center.Y;

        int fontSize = ((int)(options.Base.Height / 10) * 10);

        string triggerText = "??";

        if (options.Button == GamepadButton.LeftTrigger2 || options.Axis == GamepadAxis.LeftTrigger) triggerText = "L2";
        if (options.Button == GamepadButton.RightTrigger2 || options.Axis == GamepadAxis.RightTrigger) triggerText = "R2";

        int height = (int)(fontSize * 2.5f);
        int width = (int)(fontSize * 1.5f);

        int topLeftX = cX - width / 2;
        int topLeftY = cY - height / 2;

        Raylib.DrawRectangle(topLeftX, topLeftY, width, height, options.Base.FillColor);

        if (options.Base.Reactive)
        {
            if (options.Axis != null)
            {
                int newHeight = (int)(height * float.Clamp(GetAxis((GamepadAxis)options.Axis, options.Base.ControllerId) + 1, 0.0f, 1.0f));
                Raylib.DrawRectangle(topLeftX, topLeftY, width, newHeight, Raylib.ColorBrightness(options.Base.FillColor, 0.2f));
            }
            if (GetButtonDown((GamepadButton)options.Button, options.Base.ControllerId))
            {
                Raylib.DrawRectangle(topLeftX, topLeftY, width, height, Raylib.ColorBrightness(options.Base.FillColor, 0.4f));
            }
        }

        Raylib.DrawRectangleLines(topLeftX, topLeftY, width, height, options.Base.OutlineColor);

        Text.DrawText(triggerText, new Vector2(cX, cY), textSettings with { FontSize = fontSize, Color = options.Base.IconColor });
    }


    public static void DrawMenuButton(Vector2 center, GamepadMenuOptions options)
    {
        int cX = (int)center.X;
        int cY = (int)center.Y;

        int radiusY = options.Base.Height / 2;
        int radiusX = (int)(radiusY * 1.25f);
        int triangleWidth = (int)(radiusX / 2);

        (Vector2 V1, Vector2 V2, Vector2 V3) triangles = options.Button switch
        {
            GamepadButton.MiddleLeft => (
                new Vector2(cX + triangleWidth, cY - radiusY / 2),
                new Vector2(cX - triangleWidth, cY),
                new Vector2(cX + triangleWidth, cY + radiusY / 2)
            ),
            GamepadButton.MiddleRight => (
                new Vector2(cX - triangleWidth, cY + radiusY / 2),
                new Vector2(cX + triangleWidth, cY),
                new Vector2(cX - triangleWidth, cY - radiusY / 2)
            ),
            _ => throw new NotImplementedException(),
        };


        if (options.Base.Reactive && GetButtonDown((GamepadButton)options.Button, options.Base.ControllerId))
            Raylib.DrawEllipse(cX, cY, radiusX, radiusY, Raylib.ColorBrightness(options.Base.FillColor, 0.25f));
        else
            Raylib.DrawEllipse(cX, cY, radiusX, radiusY, options.Base.FillColor);

        Raylib.DrawTriangle(triangles.V1, triangles.V2, triangles.V3, options.Base.IconColor);

        Raylib.DrawEllipseLines(cX, cY, radiusX, radiusY, options.Base.OutlineColor);
    }

    public static void DrawArcademiaStick(Vector2 center, ArcademiaStickOptions options)
    {
        int joystickCX = (int)(center.X);
        int joystickCY = (int)(center.Y - options.Base.Height / 2);
        int joystickR = (int)(options.Base.Height / 2);

        int baseCX = (int)(center.X);
        int baseCY = (int)(center.Y + options.Base.Height / 2);
        int baseXR = (int)(options.Base.Height / 2);
        int baseYR = (int)(options.Base.Height / 4);

        float triangleStart = joystickR + 2f;
        float triangleHalfWidth = joystickR / 2f;
        float edgeDist = triangleStart + joystickR;

        Raylib.DrawEllipse(baseCX, baseCY, baseXR, baseYR, options.Base.FillColor);
        Raylib.DrawEllipseLines(baseCX, baseCY, baseXR, baseYR, options.Base.OutlineColor);
        Raylib.DrawLineEx(new Vector2(baseCX, baseCY), new Vector2(joystickCX, joystickCY), 2.0f, options.Base.OutlineColor);

        Raylib.DrawCircle(joystickCX, joystickCY, joystickR, options.Base.FillColor);
        Raylib.DrawCircleLines(joystickCX, joystickCY, joystickR, options.Base.OutlineColor);

        if (options.Directions.HasFlag(AxisDirections.UP))
        {
            Raylib.DrawTriangle(
                new Vector2(joystickCX, joystickCY - edgeDist),
                new Vector2(joystickCX - triangleHalfWidth, joystickCY - triangleStart),
                new Vector2(joystickCX + triangleHalfWidth, joystickCY - triangleStart),
                options.Base.IconColor
            );
        }

        if (options.Directions.HasFlag(AxisDirections.DOWN))
        {
            Raylib.DrawTriangle(
                new Vector2(joystickCX - triangleHalfWidth, joystickCY + triangleStart),
                new Vector2(joystickCX, joystickCY + edgeDist),
                new Vector2(joystickCX + triangleHalfWidth, joystickCY + triangleStart),
                options.Base.IconColor
            );
        }

        if (options.Directions.HasFlag(AxisDirections.LEFT))
        {
            Raylib.DrawTriangle(
                new Vector2(joystickCX - edgeDist, joystickCY),
                new Vector2(joystickCX - triangleStart, joystickCY + triangleHalfWidth),
                new Vector2(joystickCX - triangleStart, joystickCY - triangleHalfWidth),
                options.Base.IconColor
            );
        }

        if (options.Directions.HasFlag(AxisDirections.RIGHT))
        {
            Raylib.DrawTriangle(
                new Vector2(joystickCX + edgeDist, joystickCY),
                new Vector2(joystickCX + triangleStart, joystickCY - triangleHalfWidth),
                new Vector2(joystickCX + triangleStart, joystickCY + triangleHalfWidth),
                options.Base.IconColor
            );
        }
    }

    public static void DrawArcademiaButton(Vector2 center, ArcademiaButtonOptions options)
    {
        int cX = (int)center.X;
        int cY = (int)center.Y;

        int offset = options.Base.Height / 4;

        int fontSize = ((int)(options.Base.Height / 10) * 10);

        Raylib.DrawCircle(cX, cY + offset / 2, options.Base.Height / 2, Raylib.ColorBrightness(options.Base.FillColor, -0.25f));
        Raylib.DrawCircleLines(cX, cY + offset / 2, options.Base.Height / 2, Raylib.ColorBrightness(options.Base.OutlineColor, -0.25f));

        Raylib.DrawCircle(cX, cY - offset / 2, options.Base.Height / 2, options.Base.FillColor);
        Raylib.DrawCircleLines(cX, cY - offset / 2, options.Base.Height / 2, options.Base.OutlineColor);

        (bool isCenter, string message) stringInfo = options.Button switch
        {
            ArcademiaKeybind.P1_START or ArcademiaKeybind.P2_START => (false, "START"),
            ArcademiaKeybind.P1_EXIT or ArcademiaKeybind.P2_EXIT => (false, "EXIT"),
            ArcademiaKeybind.P1_A or ArcademiaKeybind.P2_A => (true, "A"),
            ArcademiaKeybind.P1_B or ArcademiaKeybind.P2_B => (true, "B"),
            ArcademiaKeybind.P1_C or ArcademiaKeybind.P2_C => (true, "C"),
            ArcademiaKeybind.P1_D or ArcademiaKeybind.P2_D => (true, "D"),
            ArcademiaKeybind.P1_E or ArcademiaKeybind.P2_E => (true, "E"),
            ArcademiaKeybind.P1_F or ArcademiaKeybind.P2_F => (true, "F"),
            _ => (true, "?")
        };

        if (stringInfo.isCenter) Text.DrawText(stringInfo.message, new Vector2(cX, cY - offset / 2), textSettings with { FontSize = fontSize, Color = options.Base.IconColor });
        else Text.DrawText(stringInfo.message, new Vector2(cX, cY + options.Base.Height), textSettings with { FontSize = fontSize, Color = options.Base.IconColor });
    }
}