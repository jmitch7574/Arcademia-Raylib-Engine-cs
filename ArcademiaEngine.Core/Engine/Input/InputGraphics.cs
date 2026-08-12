using System.Numerics;
using System.Transactions;
using Raylib_cs;


/// <summary>
/// Class for drawing Input Icons
/// </summary>
public static class InputGraphics
{
    [Flags]
    public enum AxisDirections
    {
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
    }

    public static void DrawKeyboardKey(Vector2 topLeftPosition, KeyboardKey key, int height, out int width, bool lightUp = false)
    {
        string text = GetKeyDisplayName(key);
        bool multilineText = false;
        int fontSize = height;

        if (text.Contains(' '))
        {
            multilineText = true;
            text = text.Replace(" ", "\n");
        }

        if (multilineText) fontSize /= 2;

        Vector2 size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, MathF.Floor(height / 10));

        width = (int)size.X + 5;
        int boxHeight = (int)size.Y + 5;

        width = Math.Max(width, boxHeight);

        int centerX = (int)(topLeftPosition.X + width / 2);
        int centerY = (int)(topLeftPosition.Y + height / 2);

        int additionalHeight = boxHeight - height;

        if (lightUp && Raylib.IsKeyDown(key)) Raylib.DrawRectangle((int)topLeftPosition.X, (int)topLeftPosition.Y - additionalHeight / 2, width, boxHeight, Color.Red);
        Raylib.DrawRectangleLines((int)topLeftPosition.X, (int)topLeftPosition.Y - additionalHeight / 2, width, boxHeight, Color.White);

        TextUtils.DrawTextAligned(text, fontSize, new Vector2(centerX, centerY), TextUtils.GuiTextAlignment.Center, TextUtils.GuiTextAlignmentVertical.Middle, Color.White);
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
        KeyboardKey.ScrollLock => "Scroll Lock",
        KeyboardKey.NumLock => "Num Lock",
        KeyboardKey.PageUp => "Page Up",
        KeyboardKey.PageDown => "Page Down",
        KeyboardKey.KeyboardMenu => "Menu",

        // Modifiers & Navigation
        KeyboardKey.LeftShift or KeyboardKey.RightShift => "Shift",
        KeyboardKey.LeftControl or KeyboardKey.RightControl => "Ctrl",
        KeyboardKey.LeftAlt or KeyboardKey.RightAlt => "Alt",
        KeyboardKey.LeftSuper or KeyboardKey.RightSuper => "Win",
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
}