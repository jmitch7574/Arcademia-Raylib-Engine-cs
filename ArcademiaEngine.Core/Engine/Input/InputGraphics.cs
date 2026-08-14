using System.Data.Common;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Transactions;
using ArcademiaEngine.Core;
using Raylib_cs;


/// <summary>
/// Class for drawing Input Icons
/// </summary>
public static class InputGraphics
{
    static Texture2D KeyTex;
    static NPatchInfo KeyInfo;
    static Shader KeyShader;

    public struct KeyboardKeyOptions
    {
        public int Height;
        public bool LightUp;
        public Color TextColor;
        public Color KeyColor;
    };

    public struct GamepadFaceOptions
    {
        public int Height;
        public Color OutlineColor;
        public Color InactiveColor;
        public Color ActiveColor;
        public AxisDirections Directions;
    };

    public struct GamepadStickOptions
    {
        public int Height;
        public Color OutlineColor;
        public Color FillColor;
        public Color TextColor;
        public AxisDirections Directions;
        public char StickLabel;
    };

    [Flags]
    public enum AxisDirections
    {
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
    }

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

    public static void DrawKeyboardKey(Vector2 topLeftPosition, KeyboardKey key, out int width, KeyboardKeyOptions options)
    {
        if (options.LightUp && Raylib.IsKeyDown(key)) options.KeyColor = Raylib.ColorBrightness(options.KeyColor, 0.25f);

        // Update Shader Info
        int replaceLoc0 = Raylib.GetShaderLocation(KeyShader, "replaceColor0");
        int replaceLoc1 = Raylib.GetShaderLocation(KeyShader, "replaceColor1");
        int replaceLoc2 = Raylib.GetShaderLocation(KeyShader, "replaceColor2");
        int tolLoc = Raylib.GetShaderLocation(KeyShader, "tolerance");

        Vector4 repColor0 = Raylib.ColorNormalize(Raylib.ColorBrightness(options.KeyColor, -0.5f));
        Vector4 repColor1 = Raylib.ColorNormalize(Raylib.ColorBrightness(options.KeyColor, -0.2f));
        Vector4 repColor2 = Raylib.ColorNormalize(options.KeyColor);

        Raylib.SetShaderValue(KeyShader, replaceLoc0, repColor0, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(KeyShader, replaceLoc1, repColor1, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(KeyShader, replaceLoc2, repColor2, ShaderUniformDataType.Vec4);
        Raylib.SetShaderValue(KeyShader, tolLoc, 0.1f, ShaderUniformDataType.Float);

        string text = GetKeyDisplayName(key);
        int fontSize = options.Height;

        Vector2 size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, MathF.Floor(fontSize / 10));

        width = (int)size.X + fontSize;
        int boxHeight = (int)size.Y + fontSize;

        width = Math.Max(width, boxHeight);

        int centerX = (int)(topLeftPosition.X + width / 2);
        int centerY = (int)(topLeftPosition.Y + fontSize / 2);

        int additionalHeight = boxHeight - fontSize;


        Raylib.BeginShaderMode(KeyShader);
        Raylib.DrawTextureNPatch(KeyTex, KeyInfo, new Rectangle((int)topLeftPosition.X, (int)topLeftPosition.Y - additionalHeight / 2, width, boxHeight), new Vector2(0, 0), 0, Color.White);
        Raylib.EndShaderMode();

        TextUtils.DrawTextAligned(text, fontSize, new Vector2(centerX, centerY), TextUtils.GuiTextAlignment.Center, TextUtils.GuiTextAlignmentVertical.Middle, options.TextColor);
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


    public static void DrawFaceButtons(Vector2 center, GamepadFaceOptions options)
    {
        int radius = (int)(options.Height / 3);

        int cX = (int)center.X;
        int cY = (int)center.Y;


        if (options.Directions.HasFlag(AxisDirections.UP))
            Raylib.DrawCircle(cX, cY - (2 * radius), radius, options.ActiveColor);
        else
            Raylib.DrawCircle(cX, cY - (2 * radius), radius, options.InactiveColor);

        if (options.Directions.HasFlag(AxisDirections.DOWN))
            Raylib.DrawCircle(cX, cY + (2 * radius), radius, options.ActiveColor);
        else
            Raylib.DrawCircle(cX, cY + (2 * radius), radius, options.InactiveColor);

        if (options.Directions.HasFlag(AxisDirections.LEFT))
            Raylib.DrawCircle(cX - (2 * radius), cY, radius, options.ActiveColor);
        else
            Raylib.DrawCircle(cX - (2 * radius), cY, radius, options.InactiveColor);

        if (options.Directions.HasFlag(AxisDirections.RIGHT))
            Raylib.DrawCircle(cX + (2 * radius), cY, radius, options.ActiveColor);
        else
            Raylib.DrawCircle(cX + (2 * radius), cY, radius, options.InactiveColor);

        // Outlines
        Raylib.DrawCircleLines(cX, cY - (2 * radius), radius, options.OutlineColor); // North
        Raylib.DrawCircleLines(cX, cY + (2 * radius), radius, options.OutlineColor); // South
        Raylib.DrawCircleLines(cX - (2 * radius), cY, radius, options.OutlineColor); // West
        Raylib.DrawCircleLines(cX + (2 * radius), cY, radius, options.OutlineColor); // East
    }

    public static void DrawDirectionalPad(Vector2 center, GamepadFaceOptions options)
    {
        float thickness = 2;

        int cX = (int)center.X;
        int cY = (int)center.Y;

        int borderDist = options.Height / 3; // Distance from center to inner edge
        int edgeDist = options.Height;       // Distance from center to outer edge
        int halfDist = options.Height / 2;       // For triangles

        // Backgrounds
        Raylib.DrawRectangle(cX - edgeDist, cY - borderDist, edgeDist * 2, borderDist * 2, options.InactiveColor);
        Raylib.DrawRectangle(cX - borderDist, cY - edgeDist, borderDist * 2, edgeDist * 2, options.InactiveColor);

        if (options.Directions.HasFlag(AxisDirections.UP))
            Raylib.DrawTriangle(new Vector2(cX - borderDist, cY - halfDist), new Vector2(cX + borderDist, cY - halfDist), new Vector2(cX, cY - edgeDist), options.ActiveColor);

        if (options.Directions.HasFlag(AxisDirections.DOWN))
            Raylib.DrawTriangle(new Vector2(cX, cY + edgeDist), new Vector2(cX + borderDist, cY + halfDist), new Vector2(cX - borderDist, cY + halfDist), options.ActiveColor);

        if (options.Directions.HasFlag(AxisDirections.LEFT))
            Raylib.DrawTriangle(new Vector2(cX - edgeDist, cY), new Vector2(cX - halfDist, cY + borderDist), new Vector2(cX - halfDist, cY - borderDist), options.ActiveColor);

        if (options.Directions.HasFlag(AxisDirections.RIGHT))
            Raylib.DrawTriangle(new Vector2(cX + halfDist, cY - borderDist), new Vector2(cX + halfDist, cY + borderDist), new Vector2(cX + edgeDist, cY), options.ActiveColor);


        // Outlines

        // Top Direction
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY - borderDist), new Vector2(cX - borderDist, cY - edgeDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY - edgeDist), new Vector2(cX + borderDist, cY - edgeDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + borderDist, cY - edgeDist), new Vector2(cX + borderDist, cY - borderDist), thickness, options.OutlineColor);

        // Right Direction
        Raylib.DrawLineEx(new Vector2(cX + borderDist, cY - borderDist), new Vector2(cX + edgeDist, cY - borderDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + edgeDist, cY - borderDist), new Vector2(cX + edgeDist, cY + borderDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + edgeDist, cY + borderDist), new Vector2(cX + borderDist, cY + borderDist), thickness, options.OutlineColor);


        // Bottom Direction
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY + borderDist), new Vector2(cX - borderDist, cY + edgeDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY + edgeDist), new Vector2(cX + borderDist, cY + edgeDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX + borderDist, cY + edgeDist), new Vector2(cX + borderDist, cY + borderDist), thickness, options.OutlineColor);


        // Left Direction
        Raylib.DrawLineEx(new Vector2(cX - borderDist, cY - borderDist), new Vector2(cX - edgeDist, cY - borderDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - edgeDist, cY - borderDist), new Vector2(cX - edgeDist, cY + borderDist), thickness, options.OutlineColor);
        Raylib.DrawLineEx(new Vector2(cX - edgeDist, cY + borderDist), new Vector2(cX - borderDist, cY + borderDist), thickness, options.OutlineColor);
    }

    public static void DrawStick(Vector2 center, GamepadStickOptions options)
    {
        int cX = (int)center.X;
        int cY = (int)center.Y;

        int stickOutlineEnd = (int)(options.Height / 1.25f); // Distance from center to inner edge
        int stickOutlineStart = (int)(options.Height / 1.5f); // Distance from center to inner edge

        int triangleStart = stickOutlineEnd + 2; // Distance from center to inner edge
        int triangleHalfWidth = (int)(options.Height / 3f);       // Distance from center to outer edge
        int edgeDist = (int)(options.Height * 1.25f);       // Distance from center to outer edge

        int textHeight = ((int)(options.Height / 10) * 10);

        // Stick BG
        Raylib.DrawCircle(cX, cY, stickOutlineEnd, options.FillColor);

        // Stick Outline
        Raylib.DrawRing(new Vector2(cX, cY), stickOutlineStart, stickOutlineEnd, 0, 360, 8, options.OutlineColor);

        // Stick Lbale
        TextUtils.DrawTextAligned($"{options.StickLabel}", textHeight, new Vector2(cX, cY), TextUtils.GuiTextAlignment.Center, TextUtils.GuiTextAlignmentVertical.Middle, options.TextColor);

        if (options.Directions.HasFlag(AxisDirections.UP))
            Raylib.DrawTriangle(new Vector2(cX - triangleHalfWidth, cY - triangleStart), new Vector2(cX + triangleHalfWidth, cY - triangleStart), new Vector2(cX, cY - edgeDist), options.TextColor);

        if (options.Directions.HasFlag(AxisDirections.DOWN))
            Raylib.DrawTriangle(new Vector2(cX, cY + edgeDist), new Vector2(cX + triangleHalfWidth, cY + triangleStart), new Vector2(cX - triangleHalfWidth, cY + triangleStart), options.TextColor);

        if (options.Directions.HasFlag(AxisDirections.LEFT))
            Raylib.DrawTriangle(new Vector2(cX - edgeDist, cY), new Vector2(cX - triangleStart, cY + triangleHalfWidth), new Vector2(cX - triangleStart, cY - triangleHalfWidth), options.TextColor);

        if (options.Directions.HasFlag(AxisDirections.RIGHT))
            Raylib.DrawTriangle(new Vector2(cX + triangleStart, cY - triangleHalfWidth), new Vector2(cX + triangleStart, cY + triangleHalfWidth), new Vector2(cX + edgeDist, cY), options.TextColor);
    }

}