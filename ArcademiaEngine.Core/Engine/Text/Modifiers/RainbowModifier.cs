using Raylib_cs;

namespace ArcademiaEngine.Core.Engine.Text.Modifiers;

[TextTag("rainbow")]
public class RainbowModifier : TextEffect
{
    public float Freq { get; set; } = 1.0f;
    public float Sat { get; set; } = 0.8f;
    public float Val { get; set; } = 0.8f;
    public float Speed { get; set; } = 1.0f;

    public override void Modify(ref CharFX fx)
    {
        float internalFreq = Freq * 0.05f;
        float hue = ((fx.Time * Speed) + (fx.GlyphIndexLine * internalFreq)) % 1.0f;

        if (hue < 0) hue += 1.0f;

        Color c = Color.FromHSV(hue * 360, Sat, Val);
        fx.Color = c;
    }
}