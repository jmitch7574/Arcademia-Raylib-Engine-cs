
using System.Numerics;
using Raylib_cs;

namespace ArcademiaEngine.Core.Engine.Text;

public enum Alignment
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
    public Alignment HorizontalAlignment;
    public Alignment VerticalAlignment;
    public float GlowRadius = 1.0f;
    public float GlowStrength = 1f;

    public TextSettings()
    {
    }
}

public static class Text
{
    public static void DrawText(string text, Vector2 position, TextSettings settings)
    {
        Raylib.SetTextureFilter(settings.Font.Texture, TextureFilter.Point);

        float spacing = MathF.Floor(settings.FontSize / 10f);
        string[] lines = text.Split('\n');

        Vector2 fullTextSize = Raylib.MeasureTextEx(settings.Font, text, settings.FontSize, spacing);

        float blockStartY = settings.VerticalAlignment switch
        {
            Alignment.Start => position.Y,
            Alignment.Middle => position.Y - (fullTextSize.Y / 2f),
            Alignment.End => position.Y - fullTextSize.Y,
            _ => position.Y
        };

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            Vector2 lineSize = Raylib.MeasureTextEx(settings.Font, line, settings.FontSize, spacing);

            float currentX = settings.HorizontalAlignment switch
            {
                Alignment.Start => position.X,
                Alignment.Middle => position.X - (lineSize.X / 2f),
                Alignment.End => position.X - lineSize.X,
                _ => position.X
            };

            float currentY = blockStartY + (i * (settings.FontSize + spacing));

            for (int c = 0; c < line.Length; c++)
            {
                char letter = line[c];
                string singleCharStr = letter.ToString();

                float letterY = currentY;

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