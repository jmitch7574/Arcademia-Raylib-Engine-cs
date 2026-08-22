using System.Globalization;
using Raylib_cs;

namespace ArcademiaEngine.Core.Engine.Text.Modifiers;

[TextTag("color")]
public class ColorModifier : TextEffect
{
    private Color Color = Color.White;

    public string Value
    {
        get;
        set
        {
            field = value;
            Color = ParseColor(value);
        }
    }

    public override void Modify(ref CharFX fx)
    {
        fx.Color = Color;
    }

    public static Color ParseColor(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Color.White;

        input = input.Trim();
        input = input.TrimStart('#');

        // Expand short hex #rgb -> #rrggbb
        if (input.Length == 3)
        {
            input = $"{input[0]}{input[0]}{input[1]}{input[1]}{input[2]}{input[2]}";
        }

        if (input.Length >= 6)
        {
            try
            {
                byte r = byte.Parse(input.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                byte g = byte.Parse(input.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                byte b = byte.Parse(input.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                return new Color(r, g, b, (byte)255);
            }
            catch
            {
                Inspector.Error($"[TEXT] Failed to parse color: {input}");
            }
        }

        return new Color(255, 255, 255);
    }
}