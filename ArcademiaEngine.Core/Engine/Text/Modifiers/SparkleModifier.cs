using System.Globalization;
using System.Security.Permissions;
using Raylib_cs;

namespace ArcademiaEngine.Core.Engine.Text.Modifiers;

[TextTag("sparkle")]
public class SparkleModifier : TextEffect
{
    public string Colors
    {
        get;
        set
        {
            field = value;
            ColorsList = ParseColors(value);
        }
    } = "";

    private List<Color> ColorsList = [];

    public float Freq { get; set; } = 1.0f;
    public float Speed { get; set; } = 5.0f;

    public override void Modify(ref CharFX fx)
    {
        if (ColorsList.Count < 1) return;

        float lerpValue = (float.Sin(fx.Time * Speed + fx.GlyphIndex * Freq) + 1) / 2 * (ColorsList.Count - 1);

        int indexOne = (int)float.Floor(lerpValue);
        int indexTwo = (int)float.Ceiling(lerpValue);

        fx.Color = Color.Lerp(ColorsList[indexOne], ColorsList[indexTwo], (float)(lerpValue % 1.0));
    }

    private static List<Color> ParseColors(string input)
    {
        string[] hexCodes = input.Split(",");
        List<Color> colors = [];

        foreach (string code in hexCodes)
        {
            Color col = ColorModifier.ParseColor(code);
            colors.Add(col);
        }

        return colors;
    }
}