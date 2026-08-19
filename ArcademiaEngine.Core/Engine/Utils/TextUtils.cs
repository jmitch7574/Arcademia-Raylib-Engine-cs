using System;
using System.Numerics;
using Raylib_cs;

public static class TextUtils
{
    public enum GuiTextAlignment
    {
        Start,
        Middle,
        End
    }

    public struct TextSettings
    {
        public int FontSize;
        public Font Font = Raylib.GetFontDefault(); // Currently Unused
        public Color Color;
        public GuiTextAlignment HorizontalAlignment;
        public GuiTextAlignment VerticalAlignment;

        public TextSettings()
        {
        }
    }

    public struct WaveSettings
    {
        public float Amplitude;
        public float Frequency;
        public float Speed;
        public float Offset;
    }

    public static void DrawTextAligned(string text, Vector2 position, TextSettings settings)
    {
        float spacing = MathF.Floor(settings.FontSize / 10f);

        Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, settings.FontSize, spacing);

        float targetX = settings.HorizontalAlignment switch
        {
            GuiTextAlignment.Start => position.X,
            GuiTextAlignment.Middle => position.X - (textSize.X / 2f),
            GuiTextAlignment.End => position.X - textSize.X,
            _ => position.X
        };

        float targetY = settings.VerticalAlignment switch
        {
            GuiTextAlignment.Start => position.Y,
            GuiTextAlignment.Middle => position.Y - (textSize.Y / 2f),
            GuiTextAlignment.End => position.Y - textSize.Y,
            _ => position.Y
        };

        Vector2 targetPosition = new Vector2(targetX, targetY);

        Raylib.DrawTextEx(Raylib.GetFontDefault(), text, targetPosition, settings.FontSize, spacing, settings.Color);
    }

    public static void DrawTextWave(string text, Vector2 position, TextSettings settings, WaveSettings waveSettings)
    {

    }
}