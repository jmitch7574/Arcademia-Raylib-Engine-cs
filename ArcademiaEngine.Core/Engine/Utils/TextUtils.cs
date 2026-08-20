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
    }

    public static void DrawTextAligned(string text, Vector2 position, TextSettings settings, WaveSettings? waveSettings = null)
    {
        Raylib.SetTextureFilter(settings.Font.Texture, TextureFilter.Point);

        float spacing = MathF.Floor(settings.FontSize / 10f);
        string[] lines = text.Split('\n');

        Vector2 fullTextSize = Raylib.MeasureTextEx(settings.Font, text, settings.FontSize, spacing);

        float blockStartY = settings.VerticalAlignment switch
        {
            GuiTextAlignment.Start => position.Y,
            GuiTextAlignment.Middle => position.Y - (fullTextSize.Y / 2f),
            GuiTextAlignment.End => position.Y - fullTextSize.Y,
            _ => position.Y
        };

        // Wave time
        double time = 0;

        if (waveSettings is WaveSettings speedSettings)
            time = Raylib.GetTime();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            Vector2 lineSize = Raylib.MeasureTextEx(settings.Font, line, settings.FontSize, spacing);

            float currentX = settings.HorizontalAlignment switch
            {
                GuiTextAlignment.Start => position.X,
                GuiTextAlignment.Middle => position.X - (lineSize.X / 2f),
                GuiTextAlignment.End => position.X - lineSize.X,
                _ => position.X
            };

            float currentY = blockStartY + (i * (settings.FontSize + spacing));

            for (int c = 0; c < line.Length; c++)
            {
                char letter = line[c];
                string singleCharStr = letter.ToString();

                float letterY = currentY;

                if (waveSettings is WaveSettings wave)
                {
                    int alignedC = c - line.Length / 2;
                    float waveOffset = MathF.Sin(((float)time * wave.Speed) + (wave.Frequency * (alignedC * 0.5f)));
                    letterY += (waveOffset * wave.Amplitude);
                }

                Raylib.DrawTextEx(
                    settings.Font,
                    singleCharStr,
                    new Vector2((int)currentX, (int)letterY),
                    settings.FontSize,
                    spacing,
                    settings.Color
                );

                float charWidth = Raylib.MeasureTextEx(settings.Font, singleCharStr, settings.FontSize, spacing).X;
                currentX += charWidth + spacing;
            }
        }
    }
}